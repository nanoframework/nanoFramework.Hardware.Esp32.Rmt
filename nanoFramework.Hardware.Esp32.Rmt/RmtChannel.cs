//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Base class for a RMT channel
    /// </summary>
    public abstract class RmtChannel
    {
        #region Fields

        /// <summary>
        /// The <see cref="RmtChannel"/> settings instance.
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected readonly RmtChannelSettings _settings;

        #endregion Fields

        /// <summary>
        /// Gets the current <see cref="ChannelMode"/> for this instance.
        /// </summary>
        public abstract ChannelMode Mode { get; }

        /// <summary>
        /// Gets the channel number for this instance.
        /// </summary>
        public int Channel => _settings.Channel;

        /// <summary>
        /// Gets or sets the GPIO pin used with the current channel.
        /// </summary>
        public int Pin
        {
            get => _settings.PinNumber;
            set
            {
                NativeSetGpioPin(Channel, (byte)Mode, value, invertSignal: false);
                _settings.PinNumber = value;
            }
        }

        /// <summary>
        /// Gets the source clock type. 
        /// This is currently fixed to use APB clock so will always be <see cref="SourceClock.APB"/> in the current implementation.
        /// </summary>
        /// <remarks>
        /// ESP IDF v5.1.4 supports only <see cref="SourceClock.APB"/> for ESP32. This property cannot be changed.
        /// </remarks>
        /// <exception cref="NotSupportedException"></exception>
        public static SourceClock SourceClock
        {
            get => SourceClock.APB;
        }

        /// <summary>
        /// Returns the actual frequency of the source clock used by the hardware when clock source is set to default. 
        /// This is currently 80Mhz for all devices except ESP32_H2 which is 32Mhz. This should be used to calculate timings on rmtChannel.
        /// </summary>
        public static int SourceClockFrequency
        {
            get => NativeGetSourceClockFrequency();
        }

        /// <summary>
        /// The value can be between 1 and 255 and affects all RMT channels.
        /// </summary>
        public byte ClockDivider
        {
            get => _settings.ClockDivider;
            set
            {
                NativeSetClockDivider(value);
                _settings.ClockDivider = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of memory blocks available to the current channel.
        /// </summary>
        /// <remarks>
        /// This function is used to configure the amount of memory blocks allocated to a channel.
        /// The 8 channels share a 512x32-bit RAM block which can be read and written by the processor cores over the APB bus,
        /// as well as read by the transmitters and written by the receivers.
        /// This means that every channel gets a single memory block equaling 64x32-bit.
        /// If memory block number of one channel is set to a value greater than 1, this channel will occupy the memory block of the next channel.
        /// Channel 0 can use at most 8 blocks of memory, accordingly channel 7 can only use one memory block.
        /// </remarks>
        public byte NumberOfMemoryBlocks
        {
            get => _settings.NumberOfMemoryBlocks;
            set
            {
                NativeSetNumberOfMemoryBlocks(value);
                _settings.NumberOfMemoryBlocks = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RmtChannel"/> class.
        /// </summary>
        /// <param name="settings">A <see cref="RmtChannelSettings"/> instance to configure the channel.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> cannot be null.</exception>
        protected RmtChannel(RmtChannelSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException();
        }

        #region native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetGpioPin(int channel, byte mode, int pin, bool invertSignal);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetClockDivider(byte value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetNumberOfMemoryBlocks(byte value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int NativeGetSourceClockFrequency();

        #endregion native calls
    }
}
