using System.Collections;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;

// ReSharper disable InconsistentNaming
namespace nanoFramework.Hardware.Esp32.Rmt.Benchmarks
{

    [IterationCount(2000)]
    public class SerializeCommandsBenchmark
    {
        public static readonly RmtCommand Sk6812_OnePulse = new(14, true, 12, false);
        public static readonly RmtCommand Sk6812_ZeroPulse = new(7, true, 16, false);
        public static readonly RmtCommand Sk6812_ResetCommand = new(500, false, 520, false);

        public static readonly RmtCommand Ws2808_OnePulse = new(52, true, 52, false);
        public static readonly RmtCommand Ws2808_ZeroPulse = new(14, true, 52, false);
        public static readonly RmtCommand Ws2808_ResetCommand = new(1400, false, 1400, false);

        public static readonly RmtCommand Ws2812b_OnePulse = new(32, true, 18, false);
        public static readonly RmtCommand Ws2812b_ZeroPulse = new(16, true, 34, false);
        public static readonly RmtCommand Ws2812b_ResetCommand = new(2000, false, 2000, false);

        public static readonly RmtCommand Ws2812c_OnePulse = new(52, true, 52, false);
        public static readonly RmtCommand Ws2812c_ZeroPulse = new(14, true, 52, false);
        public static readonly RmtCommand Ws2812c_ResetCommand = new(1400, false, 1400, false);

        public static readonly RmtCommand Ws2815b_OnePulse = new(52, true, 52, false);
        public static readonly RmtCommand Ws2815b_ZeroPulse = new(14, true, 52, false);
        public static readonly RmtCommand Ws2815b_ResetCommand = new(1400, false, 1400, false);

        public static readonly ArrayList RmtCommandsArrayList = new()
        {
            Sk6812_OnePulse,
            Sk6812_ZeroPulse,
            Sk6812_ResetCommand,

            Ws2808_OnePulse,
            Ws2808_ZeroPulse,
            Ws2808_ResetCommand,

            Ws2812b_OnePulse,
            Ws2812b_ZeroPulse,
            Ws2812b_ResetCommand,

            Ws2812c_OnePulse,
            Ws2812c_ZeroPulse,
            Ws2812c_ResetCommand,

            Ws2815b_OnePulse,
            Ws2815b_ZeroPulse,
            Ws2815b_ResetCommand
        };

        [Benchmark]
        public void SerializeCommands_Original()
        {
            int i = 0;
            int remaining;
            byte[] binaryCommands = new byte[RmtCommandsArrayList.Count * 4];
            foreach (var cmd in RmtCommandsArrayList)
            {
                // First pair
                if ((cmd as RmtCommand).Duration0 <= 255)
                {
                    binaryCommands[0 + i] = (byte)(cmd as RmtCommand).Duration0;
                    binaryCommands[1 + i] = (byte)((cmd as RmtCommand).Level0 == true ? 128 : 0);
                }
                else
                {
                    remaining = (cmd as RmtCommand).Duration0 % 256;
                    binaryCommands[0 + i] = (byte)(remaining);
                    binaryCommands[1 + i] = (byte)(((cmd as RmtCommand).Level0 ? 128 : 0) + (((cmd as RmtCommand).Duration0 - remaining) / 256));
                }

                // Second pair
                if ((cmd as RmtCommand).Duration1 <= 255)
                {
                    binaryCommands[2 + i] = (byte)(cmd as RmtCommand).Duration1;
                    binaryCommands[3 + i] = (byte)((cmd as RmtCommand).Level1 ? 128 : 0);
                }
                else
                {
                    remaining = (cmd as RmtCommand).Duration1 % 256;
                    binaryCommands[2 + i] = (byte)(remaining);
                    binaryCommands[3 + i] = (byte)(((cmd as RmtCommand).Level1 ? 128 : 0) + (((cmd as RmtCommand).Duration1 - remaining) / 256));
                }
                i += 4;
            }
        }

        [Benchmark]
        public void SerializeCommands_Improved_1540()
        {
            var index = 0;
            var serializedCommands = new byte[RmtCommandsArrayList.Count * 4];
            for (var i = 0; i < RmtCommandsArrayList.Count; i++)
            {
                var command = (RmtCommand)RmtCommandsArrayList[i];

                var highByte1 = (byte)(command.Duration0 >> 8);
                var lowByte1 = (byte)(command.Duration0 & 0xFF);
                var highByte2 = (byte)(command.Duration1 >> 8);
                var lowByte2 = (byte)(command.Duration1 & 0xFF);
                var level1 = (byte)(command.Level0 ? 0x80 : 0);
                var level2 = (byte)(command.Level1 ? 0x80 : 0);

                serializedCommands[index++] = lowByte1;
                serializedCommands[index++] = (byte)(highByte1 | level1);
                serializedCommands[index++] = lowByte2;
                serializedCommands[index++] = (byte)(highByte2 | level2);
            }
        }
    }
}
