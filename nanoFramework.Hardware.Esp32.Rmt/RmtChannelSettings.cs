//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Base class for shared RMT Channel settings.
    /// </summary>
    public abstract class RmtChannelSettings
    {
        private int _channel;
        private int _pinNumber;
        private byte _clockDivider;
        private byte _numberOfMemoryBlocks;
        private int _bufferSize;
        private bool _signalInverterEnabled;

        /// <summary>
        /// Gets or sets the channel number.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot make changes to the property after the RMT channel is initialized.
        /// Use the methods on the channel instance to make configuration changes.</exception>
        public int Channel
        {
            get => _channel;
            set => _channel = value;
        }

        /// <summary>
        /// Gets or sets the GPIO pin number to be used with the specified channel.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot make changes to the property after the RMT channel is initialized.
        /// Use the methods on the channel instance to make configuration changes.</exception>
        public int PinNumber
        {
            get => _pinNumber;
            set => _pinNumber = value;
        }

        /// <summary>
        /// Gets or sets the clock divider value.
        /// </summary>
        /// <remarks>
        /// ESP IDF v4.4.3 only supports the APB bus clock @ 80Mhz.
        /// The default value for the divider is 80 (80Mhz / 80 = 1Mhz = 1us ticks).
        /// </remarks>
        /// <exception cref="InvalidOperationException">Cannot make changes to the property after the RMT channel is initialized.
        /// Use the methods on the channel instance to make configuration changes.</exception>
        public byte ClockDivider
        {
            get => _clockDivider;
            set => _clockDivider = value;
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
        public byte NumberOfMemoryBlocks { get => _numberOfMemoryBlocks; set => _numberOfMemoryBlocks = value; }

        /// <summary>
        /// Gets or sets the RMT Ring Buffer size.
        /// </summary>
        /// <remarks>
        /// The ring buffer is available in transmit and receive channels.
        /// For receive channels, Incoming <see cref="RmtCommand"/>s are moved to the ring buffer after the <see cref="ReceiverChannelSettings.IdleThreshold"/> has lapsed.
        /// For transmit channels, <see cref="RmtCommand"/>s are automatically copied over to the channel's memory block and written to the transmitter.
        /// </remarks>
        public int BufferSize { get => _bufferSize; set => _bufferSize = value; }

        /// <summary>
        /// Gets or sets a value indicating if the RMT module should invert the incoming/outgoing signal.
        /// </summary>
        /// <remarks>This works like an external inverter connected to the GPIO of certain RMT channel.</remarks>
        public bool SignalInverterEnabled { get => _signalInverterEnabled; set => _signalInverterEnabled = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RmtChannelSettings"/> class.
        /// </summary>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        public RmtChannelSettings(int pinNumber)
            : this(channel: -1, pinNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RmtChannelSettings"/> class.
        /// </summary>
        /// <param name="channel">The channel number to use. Valid value range is 0 to 7 (inclusive).</param>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="channel"/> must be between 0 and 7.</exception>
        public RmtChannelSettings(int channel, int pinNumber)
        {
            this.Channel = channel <= 7
                ? channel
                : throw new ArgumentOutOfRangeException(nameof(channel));

            this.PinNumber = pinNumber;

            this.ClockDivider = 80; // 80Mhz (80_000_000) / 80 = 1Mhz (1_000_000) = 1us clock
            this.NumberOfMemoryBlocks = 1; // default as per ESP32 IDF docs
            this.BufferSize = 100; // hold 100 RMT items. Arbitrary value since 64 (amount allocated to the channel by default) didn't work for some reason.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RmtChannelSettings"/> class by copying values from the other <see cref="RmtChannelSettings"/> instance specified.
        /// </summary>
        /// <param name="other">The other <see cref="RmtChannelSettings"/> to copy values from.</param>
        internal RmtChannelSettings(RmtChannelSettings other)
        {
            this.Channel = other.Channel;
            this.PinNumber = other.PinNumber;
            this.ClockDivider = other.ClockDivider;
            this.NumberOfMemoryBlocks = other.NumberOfMemoryBlocks;
            this.BufferSize = other.BufferSize;
            this.SignalInverterEnabled = other.SignalInverterEnabled;
        }
    }
}
