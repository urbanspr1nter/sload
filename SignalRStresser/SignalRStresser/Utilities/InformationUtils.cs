using System;
using SignalRStresser.Models;

namespace SignalRStresser.Utilities
{
    class InformationUtils
    {
        public static void ShowRunParameters(RunParameters runParameters)
        {
            Console.WriteLine();
            Console.WriteLine("Run parameters: ");
            Console.WriteLine($"\t{Newtonsoft.Json.JsonConvert.SerializeObject(runParameters)}");
            Console.WriteLine();
        }

        public static void ShowConnectionsCurrentlyEstablished(int connections)
        {
            Console.Write($"... Connections currently established: {connections}\r");
        }

        public static void ShowAllConnectionsBuilt(int connections)
        {
            Console.WriteLine();
            Console.WriteLine($"Connections established: {connections}");
            Console.WriteLine("Built all hub connections.");
        }
        public static void ShowPreparingResultsNotification()
        {
            Console.WriteLine();

            string dots = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    dots += ".";
                }

                Console.Write("Preparing results from tests" + dots + "\r");

                if (dots.Length == 5)
                {
                    dots = "";
                }

                TimeUtils.Delay(200);
            }
        }

        public static void ShowResultsProcessed()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Results have been processed for this agent.");
            Console.WriteLine();
        }
    }
}
