using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SetAssociativeCache
{
    public class TimeHelper
    {
        public static long NanoTime()
        {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

    }
}
