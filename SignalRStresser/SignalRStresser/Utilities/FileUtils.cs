using SignalRStresser.Models;

namespace SignalRStresser.Utilities
{
    class FileUtils
    {
        public static string GetFinalReportJsonFilename(BenchmarkContext context)
        {
            var reportPath = System.IO.Path.Combine(context.RunParameters.OutputDirectory, "sload_run_all_report.json");

            return reportPath;
        }
    }
}
