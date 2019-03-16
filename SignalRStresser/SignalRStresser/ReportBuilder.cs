using System;
using System.Linq;
using System.Collections.Generic;
using SignalRStresser.Models;
using SignalRStresser.Models.Report;
using SignalRStresser.Enums;
using SignalRStresser.Utilities;

namespace SignalRStresser
{
    class ReportBuilder
    {
        public void BuildReport(List<AgentContext> workerContexts, BenchmarkContext context)
        {
            AggregatedResultsReport aggResultsReport = new AggregatedResultsReport();
            List<long> allTimes = new List<long>();
            List<long> reconnectTimes = new List<long>();

            System.IO.StreamWriter reportWriter = new System.IO.StreamWriter(FileUtils.GetFinalReportJsonFilename(context));

            foreach (var workerContext in workerContexts)
            {
                if (workerContext.Status == AgentStatus.Dead)
                {
                    aggResultsReport.DeadWorkers.Add(workerContext);
                    continue;
                }

                var rawReportData = workerContext.ResultsReport;

                aggResultsReport.RunParameters.Add(rawReportData.Context);
                aggResultsReport.ErrorLog.Add(rawReportData.ErrorLog);
                aggResultsReport.CallTimes.Add(rawReportData.Times);
                aggResultsReport.ReconnectionResult.AddRange(rawReportData.ReconnectResults);

                allTimes.AddRange(rawReportData.Times.Times);

                aggResultsReport.CallStatistics.SuccessfulConnections += rawReportData.Context.SuccessfulConnections;
                aggResultsReport.CallStatistics.DisconnectedConnections += rawReportData.Context.DisconnectedConnections;
                aggResultsReport.CallStatistics.FaultedConnections += rawReportData.Context.FaultedConnections;

                rawReportData.ReconnectResults.ForEach(r => reconnectTimes.Add(r.Time));
            }

            allTimes.Sort();

            // Calculate some statistics
            if(allTimes.Count > 0)
            {
                aggResultsReport.CallStatistics.CallerLatencyStats.Average = allTimes.Sum() / allTimes.Count;
                aggResultsReport.CallStatistics.CallerLatencyStats.P50 = allTimes[0 + (int)((allTimes.Count - 1) * 0.5)];
                aggResultsReport.CallStatistics.CallerLatencyStats.P90 = allTimes[0 + (int)((allTimes.Count - 1) * 0.9)];
                aggResultsReport.CallStatistics.CallerLatencyStats.P95 = allTimes[0 + (int)((allTimes.Count - 1) * 0.95)];
                aggResultsReport.CallStatistics.CallerLatencyStats.P99 = allTimes[0 + (int)((allTimes.Count - 1) * 0.99)];
            }


            int fastCount = 0; // 0 -> 99
            int acceptableCount = 0; // 100 -> 199
            int slowCount = 0; // 200 -> 999
            int xslowCount = 0; // 1000+

            allTimes.ForEach(time => {
                if (time >= 1000)
                {
                    xslowCount++;
                }
                else if (time >= 200)
                {
                    slowCount++;
                }
                else if (time >= 100)
                {
                    acceptableCount++;
                }
                else
                {
                    fastCount++;
                }
            });

            aggResultsReport.CallStatistics.CallerLatencyStats.Time10 = (1.0 * fastCount) / allTimes.Count;
            aggResultsReport.CallStatistics.CallerLatencyStats.Time100 = (1.0 * acceptableCount) / allTimes.Count;
            aggResultsReport.CallStatistics.CallerLatencyStats.Time200 = (1.0 * slowCount) / allTimes.Count;
            aggResultsReport.CallStatistics.CallerLatencyStats.Time1000 = (1.0 * xslowCount) / allTimes.Count;

            reconnectTimes.Sort();

            if (reconnectTimes.Count > 0)
            {
                aggResultsReport.CallStatistics.ReconnectStats.Average = reconnectTimes.Sum() / reconnectTimes.Count;
                aggResultsReport.CallStatistics.ReconnectStats.P50 = reconnectTimes[0 + (int)((reconnectTimes.Count - 1) * 0.5)];
                aggResultsReport.CallStatistics.ReconnectStats.P90 = reconnectTimes[0 + (int)((reconnectTimes.Count - 1) * 0.90)];
                aggResultsReport.CallStatistics.ReconnectStats.P95 = reconnectTimes[0 + (int)((reconnectTimes.Count - 1) * 0.95)];
                aggResultsReport.CallStatistics.ReconnectStats.P99 = reconnectTimes[0 + (int)((reconnectTimes.Count - 1) * 0.99)];
                aggResultsReport.CallStatistics.ReconnectStats.TotalReconnectsDuringPersistence = reconnectTimes.Count;
            }

            reportWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(aggResultsReport));
            reportWriter.Close();
        }
    }
}
