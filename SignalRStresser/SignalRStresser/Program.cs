using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using SignalRStresser.Connection;
using SignalRStresser.Enums;
using SignalRStresser.Models;
using SignalRStresser.Models.Report;
using SignalRStresser.Remote;
using SignalRStresser.Utilities;
using SignalRStresser.Network;
using Microsoft.Extensions.CommandLineUtils;
using System.Threading.Tasks;

namespace SignalRStresser
{
    class Program
    {
        static BenchmarkContext context = new BenchmarkContext();
        static AgentProvider workerProvider;

        static void Initialize(CommandOptions options)
        {
            context.InitializeBenchmarkContext(options);
            workerProvider = new AgentProvider(context);
        }

        static void Main(string[] args)
        {
            CommandLineApplication cmdApp = new CommandLineApplication();
            CommandAppBuilder cmdAppBuilder = new CommandAppBuilder();
            CommandOptions options = cmdAppBuilder.Build(context, cmdApp);

            cmdApp.Execute(args);

            Initialize(options);

            if (context.MasterNode)
            {
                var messageManager = new MessageManager(workerProvider, context);

                messageManager.Listen(context.RunParameters.MasterNodeHostname, context.RunParameters.MasterNodeListeningPort);

                // take all the slave host names and build.
                foreach(string slaveHostname in context.RunParameters.AgentNodeHostnames)
                {
                    StartWorkerMessage swMessage = workerProvider.StartWorker(slaveHostname);
                    messageManager.SendMessageToNode(new RemoteMessage {
                        Type = Remote.Enums.RemoteMessageType.StartWorker,
                        WorkerId = swMessage.RunParameters.WorkerId,
                        Body = Newtonsoft.Json.JsonConvert.SerializeObject(swMessage)
                    }, slaveHostname, context.RunParameters.AgentNodeListeningPort);
                }

                // once that happens we just need to wait...
                while (!workerProvider.WorkersComplete())
                {
                    List<AgentContext> unfinished = workerProvider.WorkerContexts.Where(x => x.Status != AgentStatus.Completed && x.Status != AgentStatus.Dead).ToList();
                    List<string> unfinishedWorkerIds = new List<string>();

                    foreach(var worker in unfinished)
                    {
                        unfinishedWorkerIds.Add(worker.WorkerId);

                        long ticksPassed = DateTime.UtcNow.Ticks - worker.LastPingTime.Ticks;
                        long workerStatusInterval = context.RunParameters.AgentNodePingInterval;
                        if (ticksPassed > TimeSpan.FromMilliseconds(workerStatusInterval).Ticks)
                        {
                            if(ticksPassed > TimeSpan.FromMilliseconds(workerStatusInterval * 4).Ticks)
                            {
                                Console.WriteLine($"No response from worker {worker.WorkerId}. Declared dead.");
                                worker.Status = AgentStatus.Dead;
                            }
                            else
                            {
                                try
                                {
                                    Task t = messageManager.Ping(worker.WorkerId, worker.Hostname, worker.Port);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine($"Could not get response from: {worker.WorkerId}.");
                                    worker.Status = AgentStatus.Dead;
                                }
                            }
                        }
                    }

                    Console.WriteLine($"Waiting for workers to finish... {StringUtils.CollectionToString(unfinishedWorkerIds, ",")}");
                    Thread.Sleep(2000);
                }

                var reportBuilder = new ReportBuilder();
                reportBuilder.BuildReport(workerProvider.WorkerContexts, context);
            }
            else if(context.MasterNode == false)
            {
                var messageManager = new MessageManager(workerProvider, context);

                messageManager.Listen(context.RunParameters.AgentNodeHostnames[0], context.RunParameters.AgentNodeListeningPort);

                while (true)
                {
                    context.MoveToBenchmarkState(BenchmarkState.Listening);

                    // wait until the start signal is received.
                    SpinListen();

                    context.WorkerId = context.RunParameters.WorkerId;
                    Console.WriteLine($"WorkerID: {context.WorkerId}");

                    InformationUtils.ShowRunParameters(context.RunParameters);

                    context.MoveToBenchmarkState(BenchmarkState.Initialized);

                    var hubManager = new HubConnectionManager(context);
                    hubManager = new HubConnectionManager(context);

                    context.MoveToBenchmarkState(BenchmarkState.BuildingConnections);
                    Task t = hubManager.SetupAndStart();

                    context.MoveToBenchmarkState(BenchmarkState.TestsPending);
                    TimeUtils.Delay(3000);

                    context.MoveToBenchmarkState(BenchmarkState.TestsRunning);
                    bool allResolved = hubManager.HubConnections.Where(x => !x.Resolved).Count() == 0 ? true : false;
                    while (!allResolved)
                    {
                        hubManager.PerformTests();

                        var numberUnresolved = hubManager.HubConnections.Where(x => !x.Resolved).Count();

                        allResolved = numberUnresolved == 0 ? true : false;
                    }
                    context.MoveToBenchmarkState(BenchmarkState.TestsCompleted);

                    InformationUtils.ShowPreparingResultsNotification();
                    hubManager.ProcessTestResults();
                    InformationUtils.ShowResultsProcessed();

                    context.MoveToBenchmarkState(BenchmarkState.PersistingConnections);
                    hubManager.RunIdle();

                    var resultsMessage = new RemoteMessage
                    {
                        WorkerId = context.RunParameters.WorkerId,
                        Type = Remote.Enums.RemoteMessageType.Results,
                        Body = Newtonsoft.Json.JsonConvert.SerializeObject(hubManager.ResultsReport)
                    };

                    messageManager.SendMessageToNode(resultsMessage, context.RunParameters.MasterNodeHostname, context.RunParameters.MasterNodeListeningPort);

                    hubManager.Dispose();

                    context.MoveToBenchmarkState(BenchmarkState.Completed);

                    messageManager.SendMessageToNode(new RemoteMessage
                    {
                        WorkerId = context.RunParameters.WorkerId,
                        Type = Remote.Enums.RemoteMessageType.WorkComplete,
                        Body = Newtonsoft.Json.JsonConvert.SerializeObject(new WorkCompleteMessage
                        {
                            WorkerId = context.RunParameters.WorkerId,
                            IpAddress = context.RunParameters.MasterNodeHostname,
                            Port = context.RunParameters.MasterNodeListeningPort
                        })
                    }, context.RunParameters.MasterNodeHostname, context.RunParameters.MasterNodeListeningPort);

                    Console.WriteLine();

                    // Reinitialize everything
                    context.MoveToBenchmarkState(BenchmarkState.Listening);
                }
            }
        }

        public static void SpinListen()
        {
            while (context.BenchmarkState == BenchmarkState.Listening)
            {
                Console.Write("Listening for any requests...\r");
            }
            Console.WriteLine();
        }
    }
}
