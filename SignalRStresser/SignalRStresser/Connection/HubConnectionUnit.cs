using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRStresser.Models;
using SignalRStresser.Models.Error;
using SignalRStresser.Enums;
using SignalRStresser.HubCommands;

namespace SignalRStresser.Connection
{
    class HubConnectionUnit
    {
        private IHubCommand _pingCommand;

        private const int _maxRetries = 3;
        private HubConnection _hub;
        private DateTime _dateUtcStart;
        private BenchmarkContext _benchmarkContext;
        private HubConnectionContext _connectionContext;

        private SignalRConnectionHandler _testConnection;

        public HubConnectionErrorType ErrorType { get => this._connectionContext.ErrorType; }
        public string Id { get => this._connectionContext.Id; }
        public bool Resolved { get => _connectionContext.Resolved; }
        public long ResolvedTime { get => _connectionContext.ResolvedTime; }
        public bool Errored { get => _connectionContext.Errored; }
        public ErrorEntry ErrorLog { get; }
        public bool Connected { get => _hub.State == HubConnectionState.Connected; }
        public bool Running { get => _testConnection.Running; }

        public bool ConnectionInitialized { get => _connectionContext.ConnectionInitialized; }

        public HubConnectionUnit(string url, BenchmarkContext context)
        {
            this._benchmarkContext = context;
            this._connectionContext = new HubConnectionContext();
            this._pingCommand = new PingCommand(this._benchmarkContext);

            this._hub = new HubConnectionBuilder().WithUrl(url).Build();
            this._hub.On<string>(_pingCommand.ReceiveMethodName(), _pingCommand.Receive);
            this._hub.On<string>("ReceiveResponse", HandleResponse);

            this._hub.Closed += HubClosed;

            this._hub.KeepAliveInterval = TimeSpan.FromMilliseconds(_benchmarkContext.RunParameters.KeepAliveInterval);
            this._hub.ServerTimeout = TimeSpan.FromMilliseconds(_benchmarkContext.RunParameters.ServerTimeout);
            this._hub.HandshakeTimeout = TimeSpan.FromMilliseconds(_benchmarkContext.RunParameters.HandShakeTimeout);

            this.ErrorLog = new ErrorEntry();
            this.ErrorLog.WorkerId = this._benchmarkContext.WorkerId;
            this.ErrorLog.Id = this._connectionContext.Id;

            _testConnection = new SignalRConnectionHandler(_hub, _connectionContext);
        }

        public async Task<long> Start()
        {
            if(_hub.State == HubConnectionState.Connected)
            {
                return 0;
            }

            try
            {
                _connectionContext.ConnectionInitialized = true;

                Stopwatch connectionTime = Stopwatch.StartNew();

                await _hub.StartAsync();

                if(_hub.State == HubConnectionState.Connected)
                {
                    Interlocked.Increment(ref _benchmarkContext.SuccessfulConnections);
                }

                connectionTime.Stop();

                // Set the default state.   
                _testConnection.Reset();

                return connectionTime.ElapsedMilliseconds;
            }
            catch (Exception e)
            {
                this.ErrorLog.ConnectionErrors.Add(new ConnectionErrorEntry {
                    RetryNumber = this._connectionContext.CurrentRetries,
                    Message = e.Message,
                    InnerMessage = e.InnerException.Message,
                    UtcDate = DateTime.UtcNow
                });

                this.Restart();
            }

            return -1;
        }

        private bool CanRun()
        {
            if (_connectionContext.NextTest.Ticks > DateTime.UtcNow.Ticks)
            {
                return false;
            }

            if (_testConnection.InvocationResult != null && (_testConnection.InvocationResult.IsCanceled || _testConnection.InvocationResult.IsFaulted))
            {
                _connectionContext.Errored = true;
                _connectionContext.Resolved = true;
                _connectionContext.ErrorType = HubConnectionErrorType.ConnectionError;
                ErrorLog.ConnectionErrors.Add(new ConnectionErrorEntry
                {
                    RetryNumber = _connectionContext.CurrentRetries,
                    Message = "The attempt to make a call to the hub failed.",
                    InnerMessage = "",
                    UtcDate = DateTime.UtcNow
                });

                Interlocked.Decrement(ref _benchmarkContext.SuccessfulConnections);
                Interlocked.Increment(ref _benchmarkContext.FaultedConnections);

                return false;
            }

            if (_testConnection.InvocationResult != null && (_testConnection.InvocationResult.Status == TaskStatus.Running))
            {
                return false;
            }

            return true;
        }

        public void Invoke()
        {
            if(!CanRun())
            {
                return;
            }

            if (Resolved || _testConnection.Running)
            {
                return; 
            }

            if (!Connected)
            {
                Interlocked.Increment(ref _benchmarkContext.DisconnectedConnections);
                Interlocked.Decrement(ref _benchmarkContext.SuccessfulConnections);

                Restart();
                return;
            }

            this._dateUtcStart = DateTime.UtcNow;

            _testConnection.SendRequest();
        }

        private void SetConnectionError(Exception arg = null)
        {
            Interlocked.Increment(ref this._benchmarkContext.DisconnectedConnections);

            var message = "The connection to the hub was closed.";
            var innerMessage = "";
            if(arg != null)
            {
                message = arg.Message;
                innerMessage = arg.InnerException.Message;
            }

            this.ErrorLog.ConnectionErrors.Add(new ConnectionErrorEntry
            {
                RetryNumber = this._connectionContext.CurrentRetries,
                Message = message,
                InnerMessage = innerMessage,
                StartUtcDate = this._dateUtcStart,
                UtcDate = DateTime.UtcNow
            });

            this._connectionContext.Errored = true;
            this._connectionContext.Resolved = true;
        }

        public void Ping()
        {
            bool isPersistingConnections = _benchmarkContext.BenchmarkState == BenchmarkState.PersistingConnections;

            TimeSpan lastPingTime = isPersistingConnections
                ? _connectionContext.LastPingForPersistTime 
                : _connectionContext.LastPingTime;

            int intervalMs = isPersistingConnections
                ? _benchmarkContext.RunParameters.PersistConnectionInterval
                : _benchmarkContext.RunParameters.PingInterval;

            double lastPingDeltaMs = (TimeSpan.FromTicks(DateTime.UtcNow.Ticks).Subtract(lastPingTime)).TotalMilliseconds;
            if (lastPingDeltaMs < intervalMs)
            {
                return;
            }

            if (isPersistingConnections)
            {
                _connectionContext.LastPingForPersistTime = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
            }
            else
            {
                _connectionContext.LastPingTime = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
            }

            _pingCommand.Send(_hub);
        }

        public void Dispose()
        {
            this._hub.StopAsync().Wait();
            this._hub.DisposeAsync().Wait();
        }

        public void HandleResponse(string message)
        {
            _testConnection.HandleResponse(message);

            this._connectionContext.ResolvedTime = _testConnection.ResolvedTImes[0];

            this._connectionContext.NextTest = TimeSpan.FromTicks(DateTime.UtcNow.AddMilliseconds(1500).Ticks);
            if(_testConnection.ResolvedTImes.Count == 3)
            {
                _connectionContext.Resolved = true;
            }
        }

        private void WaitUntilRetry()
        {
            var nextRetryOffsetMs = (int)(1500 * Math.Pow(1.5, this._connectionContext.CurrentRetries));
            var nextRetryTime = TimeSpan.FromTicks(DateTime.UtcNow.AddTicks(TimeSpan.FromMilliseconds(nextRetryOffsetMs).Ticks).Ticks);

            _connectionContext.NextRetry = nextRetryTime;
        }

        private void Restart()
        {
            if(this._connectionContext.NextRetry.Ticks > DateTime.UtcNow.Ticks)
            {
                return;
            }

            if (this._connectionContext.CurrentRetries > _maxRetries)
            {
                this._connectionContext.Resolved = true;

                Interlocked.Increment(ref _benchmarkContext.FaultedConnections);

                Console.WriteLine($"Could not establish a connection on {this.Id}");

                this._connectionContext.Errored = true;
                this._connectionContext.ErrorType = HubConnectionErrorType.ConnectionError;

                if (_testConnection.InvocationResult != null)
                {
                    this.ErrorLog.Result = _testConnection.InvocationResult.Exception == null ? "" : _testConnection.InvocationResult.Exception.Message;
                }

                return;
            }

            WaitUntilRetry();
            _connectionContext.CurrentRetries++;

            Task t = Start();
        }

        private Task HubClosed(Exception arg)
        {
            return new TaskFactory().StartNew(() => SetConnectionError(arg));
        }
    }
}
