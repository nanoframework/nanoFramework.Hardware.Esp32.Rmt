//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Enumeration of Led strip types.
    /// </summary>
    public enum LedType
    {
        /// <summary>
        /// WS2811 led type.
        /// </summary>
        WS2811,
        /// <summary>
        /// WS2812 led type.
        /// </summary>
        WS2812,
        /// <summary>
        /// WS2813 led type.
        /// </summary>
        WS2813,
        /// <summary>
        /// WS2815 led type.
        /// </summary>
        WS2815,
        /// <summary>
        /// SK2812 led type.
        /// </summary>
        SK2812,
    };

    /// <summary>
    /// Class to create RMT TransmitterChannel for led strips.
    /// </summary>
    public class LedTransmitChannel : IDisposable
    {
        private TransmitterEncodedChannel _transmitChannel;
        private bool disposedValue;

        /// <summary>
        /// Create a LedTransmitChannel for use with a led strip with the specified timings.
        /// </summary>
        /// <remarks>
        /// Used for creating led strip channels with different timings to standard led strip types.
        /// For standard types use <see cref="LedTransmitChannel(int, LedType)"/>
        /// </remarks>
        /// <param name="pinNumber">GPIO output pin number.</param>
        /// <param name="T0L">Data transfer time in ticks for bit 0 low part (number of 100ns ticks).</param>
        /// <param name="T0H">Data transfer time in ticks for bit 0 high part (number of 100ns ticks).</param>
        /// <param name="T1L">Data transfer time in ticks for bit 1 low part (number of 100ns ticks).</param>
        /// <param name="T1H">Data transfer time in ticks for bit 1 high part (number of 100ns ticks).</param>
        /// <param name="ResetTime">Data transfer time in ticks for reset (number of 100ns ticks).</param>
        /// <returns>TransmitterChannel object.</returns>
        public LedTransmitChannel(int pinNumber, ushort T0L, ushort T0H, ushort T1L, ushort T1H, ushort ResetTime)
        {
            Initialize(pinNumber, T0L, T0H, T1L, T1H, ResetTime);
        }

        /// <summary>
        /// Create a TransmitterChannel for use with specified WS28xx/SK28xx led.
        /// </summary>
        /// <param name="pinNumber">GPIO output pin number.</param>
        /// <param name="wsType">Type of WS28xx/SK28xx led strip.</param>
        /// <returns>TransmitterChannel instance.</returns>
        public LedTransmitChannel(int pinNumber, LedType wsType)
        {
            ushort T0L;
            ushort T0H;
            ushort T1L;
            ushort T1H;
            ushort ResetTime;

            switch (wsType)
            {
                // T0L= 2.0µs, T0H=0.5µs, T1L=1.3µs, T1H=1.2µs, Reset=>50µs
                case LedType.WS2811:
                    T0L = 20;
                    T0H = 5;
                    T1L = 13;
                    T1H = 12;
                    ResetTime = 500;
                    break;

                // T0L=900ns, T0H=300ns, T1L=300ns, T1H=900ns, Reset=50µs
                case LedType.WS2812:
                    T0L = 9;
                    T0H = 3;
                    T1L = 3;
                    T1H = 9;
                    ResetTime = 500;
                    break;

                // T0L= 600ns, T0H=300ns, T1L=300ns, T1H=600ns, Reset=>280µs
                case LedType.WS2813:
                    T0L = 6;
                    T0H = 3;
                    T1L = 3;
                    T1H = 6;
                    ResetTime = 3000;
                    break;

                // T0L= 600ns, T0H=300ns, T1L=300ns, T1H=600ns, Reset=>280µs
                case LedType.WS2815:
                    T0L = 6;
                    T0H = 3;
                    T1L = 3;
                    T1H = 6;
                    ResetTime = 3000;
                    break;

                // T0L= 900ns, T0H=300ns, T1L=600ns, T1H=600ns, Reset= 80µs
                case LedType.SK2812:
                    T0L = 9;
                    T0H = 3;
                    T1L = 6;
                    T1H = 6;
                    ResetTime = 800;
                    break;

                default:
                    throw new ArgumentException();
            }

            Initialize(pinNumber, T0L, T0H, T1L, T1H, ResetTime);
        }

        // Initialization logic
        private void Initialize(int pinNumber, ushort T0L, ushort T0H, ushort T1L, ushort T1H, ushort ResetTime)
        {
            // The resolution must be high enough to support the timing requirements of the led strip.
            // With 100ns ticks, 10MHz resolution allows for 100ns increments, which is sufficient for WS28xx/SK28xx timings.
            const int resolution = 10_000_000;

            TransmitChannelSettings settings = new(pinNumber)
            {
                ResolutionHz = resolution,
                NumberOfMemoryBlocks = 1
            };

            // Setup timing symbols for bit 0, bit 1, and reset
            RmtSymbol bit0 = new(T0H, true, T0L, false);
            RmtSymbol bit1 = new(T1H, true, T1L, false);
            ushort resetFirst = (ushort)(ResetTime / 2);
            ushort resetSecond = (ushort)(ResetTime - resetFirst);
            RmtSymbol[] resetSymbols = new[] { new RmtSymbol(resetFirst, false, resetSecond, false) };

            // Configure the encoders: ByteEncoder for the led data, and CopyEncoder for the reset symbol
            ByteEncoderSettings byteEnc = new ByteEncoderSettings(bit0, bit1, true, 0);
            CopyEncoderSettings copyEnc = new CopyEncoderSettings(resetSymbols);

            // Create the TransmitterEncodedChannel with the specified settings and encoders
            _transmitChannel = new TransmitterEncodedChannel(settings, new EncoderSettings[] { byteEnc, copyEnc });
        }

        /// <summary>
        /// Return underlying rmt channel.
        /// Can be used to pass to TransmitSyncManager to synchronize led channels.
        /// </summary>
        public TransmitterEncodedChannel Channel { get { return _transmitChannel; } }
        
        /// <summary>
        /// Send Led data to the strip. Data should be in correct color format, 3 bytes per led, and in the order of LEDS on the strip.
        /// </summary>
        /// <param name="ledData">Array of bytes representing the LED data.</param>
        /// <param name="RepeatDataCount">Number of times to output data. 
        /// This can be used to set each LED with same info or repeat pattern along whole string.
        /// </param>
        /// <param name="waitToComplete">Wait for the transmission to complete before returning.</param>
        public void SendLedData(byte[] ledData, int RepeatDataCount = 1, bool waitToComplete = true)
        {
            _transmitChannel.SendWithEncoders(new EncoderData[] { new EncoderData(ledData, RepeatDataCount) }, waitToComplete);
        }


        /// <summary>
        /// Dispose pattern implementation to clean up the TransmitterChannel when done.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _transmitChannel.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose 
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
