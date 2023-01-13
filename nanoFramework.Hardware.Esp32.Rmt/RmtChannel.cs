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
        /// The source clock. Only the 80MHz APB clock is currently supported.
        /// </summary>
        /// <remarks>
        /// ESP IDF v4.4.3 supports only <see cref="SourceClock.APB"/>. This property cannot be changed.
        /// </remarks>
        /// <exception cref="NotSupportedException"></exception>
        public SourceClock SourceClock
        {
            get => SourceClock.APB;
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// The value can be between 1 and 255.
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
        /// <exception cref="ArgumentNullException"></exception>
        protected RmtChannel(RmtChannelSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #region native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetGpioPin(int channel, byte mode, int pin, bool invertSignal);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetClockDivider(byte value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetNumberOfMemoryBlocks(byte value);

        #endregion native calls
    }
}
