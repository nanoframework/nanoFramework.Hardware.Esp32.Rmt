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
        /// and <see cref="RmtCommand"/>s are copied into the ring buffer and availble to your code.
        /// </summary>
        /// <remarks>
        /// The receive process finishes(goes idle) when no edges have been detected for Idle Threshold clock cycles.
        /// Supported value range between 1 and 65535 (0xFFFF)
        /// </remarks>
        public ushort IdleThreshold
        {
            get => _idleThreshold;
            set
            {
                if (value == 0)
                    throw new ArgumentOutOfRangeException();

                _idleThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the filter state. 
        /// If enabled, the receiver will ignore pulses with widths less than specified in <see cref="FilterThreshold"/>.
        /// </summary>
        public bool EnableFilter { get => _enableFilter; set => _enableFilter = value; }

        /// <summary>
        /// Gets or sets the threshold for pulse widths to ignore when <see cref="EnableFilter"/> is set to <see langword="true"/>.
        /// </summary>
        public byte FilterThreshold { get => _filterThreshold; set => _filterThreshold = value; }

        /// <summary>
        /// Gets or sets the timeout threshold for the <see cref="ReceiverChannel.GetAllItems"/> call. Defaults to 1 second.
        /// </summary>
        public TimeSpan ReceiveTimeout { get => _receiveTimeout; set => _receiveTimeout = value; }

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
            this.IdleThreshold = 12_000; //12ms
            this.EnableFilter = true;
            this.FilterThreshold = 100;
            this.ReceiveTimeout = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverChannelSettings"/> class by copying values from the other <see cref="ReceiverChannelSettings"/> instance specified.
        /// </summary>
        /// <param name="other">The other <see cref="ReceiverChannelSettings"/> to copy values from.</param>
        internal ReceiverChannelSettings(ReceiverChannelSettings other) : base(other)
        {
            this.IdleThreshold = other.IdleThreshold;
            this.EnableFilter = other.EnableFilter;
            this.FilterThreshold = other.FilterThreshold;
            this.ReceiveTimeout = other.ReceiveTimeout;
        }
    }
}
