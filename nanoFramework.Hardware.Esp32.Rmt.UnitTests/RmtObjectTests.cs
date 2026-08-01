using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Hardware.Esp32.Rmt.UnitTests
{
    [TestClass]
    public class RmtManagedApiTests
    {
        // ------------------------------------------------------------
        //  RmtSymbol
        // ------------------------------------------------------------

        [TestMethod]
        public void RmtSymbol_ConstructsCorrectly()
        {
            var s = new RmtSymbol(10, true, 20, false);

            Assert.AreEqual((ushort)10, s.Duration0);
            Assert.AreEqual((ushort)20, s.Duration1);
            Assert.IsTrue(s.Level0);
            Assert.IsFalse(s.Level1);
        }

        [TestMethod]
        public void RmtSymbol_ThrowsOnInvalidDuration()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => new RmtSymbol(40000, true, 20, false));
        }

        // ------------------------------------------------------------
        //  RmtSymbols
        // ------------------------------------------------------------

        [TestMethod]
        public void RmtSymbols_AddClearIndexing_Works()
        {
            var symbols = new RmtSymbols();

            var a = new RmtSymbol(5, true, 6, false);
            var b = new RmtSymbol(7, false, 8, true);

            symbols.Add(a);
            symbols.Add(b);

            Assert.AreEqual(2, symbols.Count);
            Assert.AreSame(a, symbols[0]);
            Assert.AreSame(b, symbols[1]);

            symbols.Clear();
            Assert.AreEqual(0, symbols.Count);
        }

        [TestMethod]
        public void RmtSymbols_IndexerThrowsOnInvalidIndex()
        {
            var symbols = new RmtSymbols();
            symbols.Add(new RmtSymbol(1, true, 1, false));

            Assert.ThrowsException(typeof(IndexOutOfRangeException),
                () => { var _ = symbols[5]; });
        }

        // ------------------------------------------------------------
        //  ByteEncoderSettings
        // ------------------------------------------------------------

        [TestMethod]
        public void ByteEncoderSettings_ConstructsCorrectly()
        {
            var zero = new RmtSymbol(1, true, 2, false);
            var one = new RmtSymbol(3, false, 4, true);

            var enc = new ByteEncoderSettings(zero, one, true, 8);

            Assert.AreSame(zero, enc.Bit0);
            Assert.AreSame(one, enc.Bit1);
            Assert.IsTrue(enc.MsbFirst);
        }

        // ------------------------------------------------------------
        //  CopyEncoderSettings
        // ------------------------------------------------------------

        [TestMethod]
        public void CopyEncoderSettings_ConstructsCorrectly()
        {
            var enc = new CopyEncoderSettings();
            Assert.IsNotNull(enc);
        }

        // ------------------------------------------------------------
        //  EncoderData
        // ------------------------------------------------------------

        [TestMethod]
        public void EncoderData_ConstructsCorrectly()
        {
            var data = new byte[] { 1, 2, 3 };
            var ed = new EncoderData(data, 5);

            Assert.AreSame(data, ed.Data);
            Assert.AreEqual(5, ed.Loop);
        }

        // ------------------------------------------------------------
        //  TransmitChannelSettings
        // ------------------------------------------------------------

        [TestMethod]
        public void TransmitChannelSettings_ConstructsCorrectly()
        {
            var s = new TransmitChannelSettings(12);

            Assert.AreEqual(12, s.PinNumber);
            Assert.AreEqual(1_000_000, s.ResolutionHz);
            Assert.AreEqual((byte)1, s.NumberOfMemoryBlocks);
        }

        [TestMethod]
        public void TransmitChannelSettings_ThrowsOnInvalidValues()
        {
            var s = new TransmitChannelSettings(5);

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => s.CarrierWaveFrequency = 0);

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => s.CarrierWaveDutyPercentage = 0);

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => s.LoopCount = 0);
        }

        // ------------------------------------------------------------
        //  ReceiverChannelSettings
        // ------------------------------------------------------------

        [TestMethod]
        public void ReceiverChannelSettings_ConstructsCorrectly()
        {
            var s = new ReceiverChannelSettings(4);

            Assert.AreEqual(4, s.PinNumber);
            Assert.AreEqual(12000000u, s.IdleThreshold);
            Assert.AreEqual(1200u, s.FilterThreshold);
        }

        [TestMethod]
        public void ReceiverChannelSettings_ThrowsOnInvalidValues()
        {
            var s = new ReceiverChannelSettings(4);

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => s.IdleThreshold = 0);

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => s.CarrierWaveFrequency = 0);

            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => s.CarrierWaveDutyPercentage = 0);
        }

        // ------------------------------------------------------------
        //  RmtChannel base class
        // ------------------------------------------------------------

        private sealed class DummyChannel : RmtChannel
        {
            public DummyChannel(RmtChannelSettings s) : base(s) { }
        }

        [TestMethod]
        public void RmtChannel_BasePropertiesWork()
        {
            var settings = new TransmitChannelSettings(10);
            var ch = new DummyChannel(settings);

            Assert.AreEqual(10, ch.Pin);

            ch.Pin = 22;
            Assert.AreEqual(22, ch.Pin);

            ch.ResolutionHz = 2_000_000;
            Assert.AreEqual(2_000_000, ch.ResolutionHz);
        }
    }
}
