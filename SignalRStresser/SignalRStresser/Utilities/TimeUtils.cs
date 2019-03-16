using System;
using System.Diagnostics;

namespace SignalRStresser.Utilities
{
    class TimeUtils
    {
        public static void Delay(long milliseconds)
        {
            long totalTicks = TimeSpan.FromMilliseconds(milliseconds).Ticks;

            long ticks = 0;
            var stopwatch = Stopwatch.StartNew();
            while (ticks < totalTicks)
            {
                ticks += (stopwatch.ElapsedTicks - ticks);
            }
            stopwatch.Stop();
        }
    }
}
