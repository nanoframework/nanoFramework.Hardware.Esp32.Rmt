//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// <see cref="ReceiverChannel"/>'s settings class.
    /// </summary>
    /// <remarks>
    /// All changes made to properties of this class are ignored after the <see cref="ReceiverChannel"/> is initialized.
    /// The equivalent properties in the channel instance can be used to make on-the-fly changes to the configurations.
    /// </remarks>
    public sealed class ReceiverChannelSettings : RmtChannelSettings
    {
        private ushort _idleThreshold;
        private bool _enableFilter;
        private byte _filterThreshold;
        private TimeSpan _receiveTimeout;

        /// <summary>
        /// Gets or sets the idle threshold after which the receiver will go into idle mode 
        /// and <see cref="RmtCommand"/>s are copied into the ring buffer and availble to your code. This is measured by number of clock ticks (after applying the clock divider).
        /// </summary>
        /// <remarks>
        /// The receive process finishes(goes idle) when no edges have been detected for the specified <see cref="IdleThreshold"/> clock cycles.
        /// Supported value range between 1 and 65535 (0xFFFF).
        /// The RMT Module's clock ticks at a rate of 80Mhz. If the <see cref="RmtChannelSettings.ClockDivider"/> is set to 80 for example, then a clock tick is equal to 1 microsecond (80Mhz / 80 = 1Mhz = 1us).
        /// So setting this property to a value of 200 means the threshold is 200us.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be set to 0 or less</exception>
        public ushort IdleThreshold
        {
            get => _idleThreshold;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _idleThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the filter state. 
        /// If enabled, the receiver will ignore pulses with widths less than specified in <see cref="FilterThreshold"/>.
        /// </summary>
        public bool EnableFilter 
        { 
            get => _enableFilter; 
            set => _enableFilter = value;
        }

        /// <summary>
        /// Gets or sets the threshold, in clock ticks, of the filter.
        /// when <see cref="EnableFilter"/> is set to <see langword="true"/> It will ignore pulses shorter than the specified threshold.
        /// The acceptable range of values is 0 to 255 clock ticks.
        /// </summary>
        /// <remarks>
        /// Example:
        /// If the <see cref="RmtChannelSettings.ClockDivider"/> is set to 80 then the clock (80Mhz) will tick at a rate of 1Mhz (80Mhz / 80 = 1Mhz) making each clock tick equal to 1 microsecond.
        /// Therefore, setting <see cref="FilterThreshold"/> to a value like 100 will cause the receiver channel to ignore any pulses that are shorter than 100 microseconds.
        /// </remarks>
        public byte FilterThreshold 
        { 
            get => _filterThreshold; 
            set => _filterThreshold = value;
        }

        /// <summary>
        /// Gets or sets the timeout threshold for the <see cref="ReceiverChannel.GetAllItems"/> call. Defaults to 1 second.
        /// </summary>
        public TimeSpan ReceiveTimeout 
        { 
            get => _receiveTimeout; 
            set => _receiveTimeout = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverChannelSettings"/> class.
        /// </summary>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        /// <remarks>This constructor will use the next available RMT channel starting from channel 0 and up to channel 7.</remarks>
        public ReceiverChannelSettings(int pinNumber) : this(channel: -1, pinNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverChannelSettings"/> class.
        /// </summary>
        /// <param name="channel">The channel number to use. Valid value range is 0 to 7 (inclusive).</param>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="channel"/> must be between 0 and 7.</exception>
        public ReceiverChannelSettings(int channel, int pinNumber) : base(channel, pinNumber)
        {
            _idleThreshold = 12_000; //12ms
            _enableFilter = true;
            _filterThreshold = 100;
            _receiveTimeout = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverChannelSettings"/> class by copying values from the other <see cref="ReceiverChannelSettings"/> instance specified.
        /// </summary>
        /// <param name="other">The other <see cref="ReceiverChannelSettings"/> to copy values from.</param>
        internal ReceiverChannelSettings(ReceiverChannelSettings other) : base(other)
        {
            _idleThreshold = other.IdleThreshold;
            _enableFilter = other.EnableFilter;
            _filterThreshold = other.FilterThreshold;
            _receiveTimeout = other.ReceiveTimeout;
        }
    }
}
