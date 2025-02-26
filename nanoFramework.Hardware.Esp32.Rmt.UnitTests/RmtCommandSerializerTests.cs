using System.Collections;
using nanoFramework.TestFramework;
// ReSharper disable InconsistentNaming

namespace nanoFramework.Hardware.Esp32.Rmt.UnitTests
{
    [TestClass]
    internal class RmtCommandSerializerTests
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

        [TestMethod]
        public void SerializeCommands_correctly_serializes_commands()
        {
            var commands = new ArrayList
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

            var result = RmtCommandSerializer.SerializeCommands(commands);

            Assert.AreEqual(60, result.Length);
            Assert.AreEqual((byte)14, result[0]);
            Assert.AreEqual((byte)128, result[1]);
            Assert.AreEqual((byte)12, result[2]);
            Assert.AreEqual((byte)0, result[3]);
            Assert.AreEqual((byte)7, result[4]);
            Assert.AreEqual((byte)128, result[5]);
            Assert.AreEqual((byte)16, result[6]);
            Assert.AreEqual((byte)0, result[7]);
            Assert.AreEqual((byte)244, result[8]);
            Assert.AreEqual((byte)1, result[9]);
            Assert.AreEqual((byte)8, result[10]);
            Assert.AreEqual((byte)2, result[11]);
            Assert.AreEqual((byte)52, result[12]);
            Assert.AreEqual((byte)128, result[13]);
            Assert.AreEqual((byte)52, result[14]);
            Assert.AreEqual((byte)0, result[15]);
            Assert.AreEqual((byte)14, result[16]);
            Assert.AreEqual((byte)128, result[17]);
            Assert.AreEqual((byte)52, result[18]);
            Assert.AreEqual((byte)0, result[19]);
            Assert.AreEqual((byte)120, result[20]);
            Assert.AreEqual((byte)5, result[21]);
            Assert.AreEqual((byte)120, result[22]);
            Assert.AreEqual((byte)5, result[23]);
            Assert.AreEqual((byte)32, result[24]);
            Assert.AreEqual((byte)128, result[25]);
            Assert.AreEqual((byte)18, result[26]);
            Assert.AreEqual((byte)0, result[27]);
            Assert.AreEqual((byte)16, result[28]);
            Assert.AreEqual((byte)128, result[29]);
            Assert.AreEqual((byte)34, result[30]);
            Assert.AreEqual((byte)0, result[31]);
            Assert.AreEqual((byte)208, result[32]);
            Assert.AreEqual((byte)7, result[33]);
            Assert.AreEqual((byte)208, result[34]);
            Assert.AreEqual((byte)7, result[35]);
            Assert.AreEqual((byte)52, result[36]);
            Assert.AreEqual((byte)128, result[37]);
            Assert.AreEqual((byte)52, result[38]);
            Assert.AreEqual((byte)0, result[39]);
            Assert.AreEqual((byte)14, result[40]);
            Assert.AreEqual((byte)128, result[41]);
            Assert.AreEqual((byte)52, result[42]);
            Assert.AreEqual((byte)0, result[43]);
            Assert.AreEqual((byte)120, result[44]);
            Assert.AreEqual((byte)5, result[45]);
            Assert.AreEqual((byte)120, result[46]);
            Assert.AreEqual((byte)5, result[47]);
            Assert.AreEqual((byte)52, result[48]);
            Assert.AreEqual((byte)128, result[49]);
            Assert.AreEqual((byte)52, result[50]);
            Assert.AreEqual((byte)0, result[51]);
            Assert.AreEqual((byte)14, result[52]);
            Assert.AreEqual((byte)128, result[53]);
            Assert.AreEqual((byte)52, result[54]);
            Assert.AreEqual((byte)0, result[55]);
            Assert.AreEqual((byte)120, result[56]);
            Assert.AreEqual((byte)5, result[57]);
            Assert.AreEqual((byte)120, result[58]);
            Assert.AreEqual((byte)5, result[59]);
        }
    }
}
