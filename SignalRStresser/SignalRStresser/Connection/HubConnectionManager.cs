using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SignalRStresser.Enums;
using SignalRStresser.Models;
using SignalRStresser.Models.Report;
using SignalRStresser.Utilities;
using System.Diagnostics;

namespace SignalRStresser.Connection
{
    class HubConnectionManager : IDisposable
    {
        private Collector _collector;
        private BenchmarkContext _context;
        private ResultsReport _report;

        public ResultsReport ResultsReport { get => _report; }
        public string TestResults { get; set; }

        public List<HubConnectionUnit> HubConnections { get; }

        public HubConnectionManager(BenchmarkContext context)
        {
            this.HubConnections = new List<HubConnectionUnit>();
            this._collector = new Collector();
            this._context = context;
            _report = new ResultsReport();
        }

        public void Dispose()
        {
            foreach (var hu in this.HubConnections)
            {
                hu.Dispose();
            }
        }

        public async Task KeepConnectionsAlive()
        {
            foreach (HubConnectionUnit hu in this.HubConnections)
            {
                // Keep the existing connections alive. Do this one at a time.
                if (hu.Connected)
                {
                    hu.Ping();
                }
                else if(!hu.Connected && hu.ConnectionInitialized)
                {
                    // Reconnect
                    long reconnectTime = await hu.Start();

                    _collector.SendConnectionResult(hu.Id, reconnectTime);
                }
            }
        }

        public async Task SetupAndStart()
        {
            for (int i = 0; i < _context.RunParameters.MaxConnections; i++)
            {
                HubConnections.Add(new HubConnectionUnit(_context.RunParameters.HubUrl, _context));
            }

            int batchSize = _context.RunParameters.NumberOfCpus * 4;

            List<Task> tasks = new List<Task>();
            for(int i = 0; i < _context.RunParameters.MaxConnections; i++)
            {
                if(i % batchSize == 0)
                {
                    InformationUtils.ShowConnectionsCurrentlyEstablished(_context.SuccessfulConnections);

                    Task k = KeepConnectionsAlive();

                    Thread.Sleep(_context.RunParameters.RampUpInterval);
                }

                tasks.Add(HubConnections[i].Start());
            }

            await Task.WhenAll(tasks.ToArray());

            InformationUtils.ShowAllConnectionsBuilt(_context.SuccessfulConnections);
        }

        public void PerformTests()
        {
            Console.Write("Performing data transfer test...\r");

            List<HubConnectionUnit> unresolvedHubConnections = HubConnections.Where(x => !x.Resolved).ToList();

            if (unresolvedHubConnections.Count == 0)
            {
                Console.WriteLine("Completed data transfer test.");
                return;
            }

            Parallel.ForEach(unresolvedHubConnections, connection => { connection.Invoke(); });
        }

        public void ProcessTestResults()
        {
            List<Models.Error.ErrorEntry> errorLog = new List<Models.Error.ErrorEntry>();

            foreach (HubConnectionUnit hu in HubConnections)
            {
                if (hu.Errored && hu.ErrorType == Enums.HubConnectionErrorType.ConnectionError)
                {
                    errorLog.Add(hu.ErrorLog);
                    continue;
                }

                _collector.SendResult(hu.ResolvedTime, CollectorResultType.CallerLatency);
            }

            List<long> callTimeResults = _collector.GetResults(CollectorResultType.CallerLatency);

            System.IO.Directory.CreateDirectory($"{_context.RunParameters.OutputDirectory}");

            _report.Context = _context;
            _report.Times = new CallTimes { WorkerId = _context.WorkerId, Times = callTimeResults };
            _report.ErrorLog = errorLog;
        }

        public void RunIdle()
        {
            int seconds = _context.RunParameters.PersistConnectionTime;
            long ticks = 0;
            long totalTime = TimeSpan.FromSeconds(seconds).Ticks;

            Stopwatch watch = Stopwatch.StartNew();
            while (ticks < totalTime)
            {
                ticks += (watch.ElapsedTicks - ticks);

                Console.Write($"Persisting connection for {_context.RunParameters.PersistConnectionTime} seconds. Elapsed: {TimeSpan.FromTicks(ticks).TotalSeconds}.          \r");

                Task t = this.KeepConnectionsAlive();

                // We want to sleep here just to allow other processes in the system to actually do stuff.
                Thread.Sleep(1000);
            }

            // Process the reconnects
            _report.ReconnectResults = _collector.GetConnectionResults();

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
