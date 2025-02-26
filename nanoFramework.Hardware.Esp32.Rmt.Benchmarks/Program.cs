using nanoFramework.Benchmark;
using System;
using System.Diagnostics;
using System.Threading;

namespace nanoFramework.Hardware.Esp32.Rmt.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
#if DEBUG
            Console.WriteLine("Benchmarks should be run in a release build.");
            Debugger.Break();
            return;
#endif

            BenchmarkRunner.RunClass(typeof(SerializeCommandsBenchmark));
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
