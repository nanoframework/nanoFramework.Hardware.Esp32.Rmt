//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// <see cref="TransmitterChannel"/>'s settings class.
    /// </summary>
    /// <remarks>
    /// All changes made to properties of this class are ignored after the <see cref="TransmitterChannel"/> is initialized.
    /// The equivalent properties in the channel instance can be used to make on-the-fly changes to the configurations.
    /// </remarks>
    public sealed class TransmitChannelSettings : RmtChannelSettings
    {
        private bool enableCarrierWave;
        private bool carrierLevel;
        private int carrierWaveFrequency;
        private byte carrierWaveDutyPercentage;
        private bool enableLooping;
        private bool enableIdleLevelOutput;
        private bool idleLevel;

        /// <summary>
        /// Enables or disables the carrier wave generator in the RMT Hardware.
        /// </summary>
        public bool EnableCarrierWave { get => enableCarrierWave; set => enableCarrierWave = value; }

        /// <summary>
        /// Gets or sets a value indicating at which level of RMT output is the carrier wave applied.
        /// <see langword="true" /> = HIGH.
        /// </summary>
        public bool CarrierLevel { get => carrierLevel; set => carrierLevel = value; }

        /// <summary>
        /// Gets or sets the carrier wave frequency.
        /// </summary>
        public int CarrierWaveFrequency { get => carrierWaveFrequency; set => carrierWaveFrequency = value; }

        /// <summary>
        /// Gets or sets the carrier wave duty cycle percentage.
        /// </summary>
        public byte CarrierWaveDutyPercentage { get => carrierWaveDutyPercentage; set => carrierWaveDutyPercentage = value; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable looping through the ring buffer when transmitting <see cref="RmtCommand"/>s.
        /// </summary>
        public bool EnableLooping { get => enableLooping; set => enableLooping = value; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable the idle level output.
        /// </summary>
        public bool EnableIdleLevelOutput { get => enableIdleLevelOutput; set => enableIdleLevelOutput = value; }

        /// <summary>
        /// Gets or sets a value indicating the RMT idle level.
        /// <see langword="true" /> = HIGH.
        /// </summary>
        public bool IdleLevel { get => idleLevel; set => idleLevel = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitChannelSettings"/> class.
        /// </summary>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        /// <remarks>This constructor will use the next available RMT channel starting from channel 0 and up to channel 7.</remarks>
        public TransmitChannelSettings(int pinNumber) : this(channel: -1, pinNumber)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitChannelSettings"/> class.
        /// </summary>
        /// <param name="channel">The channel number to use. Valid value range is 0 to 7 (inclusive).</param>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="channel"/> must be between 0 and 7.</exception>
        public TransmitChannelSettings(int channel, int pinNumber) : base(channel, pinNumber)
        {
            this.EnableCarrierWave = true;
            this.CarrierLevel = true;
            this.CarrierWaveFrequency = 38_000;
            this.CarrierWaveDutyPercentage = 33;

            this.EnableLooping = false;

            this.EnableIdleLevelOutput = true;
            this.IdleLevel = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitChannelSettings"/> class by copying values from the other <see cref="TransmitChannelSettings"/> instance specified.
        /// </summary>
        /// <param name="other">The other <see cref="TransmitChannelSettings"/> to copy values from.</param>
        internal TransmitChannelSettings(TransmitChannelSettings other) : base(other)
        {
            this.EnableCarrierWave = other.EnableCarrierWave;
            this.CarrierLevel = other.CarrierLevel;
            this.CarrierWaveFrequency = other.CarrierWaveFrequency;
            this.CarrierWaveDutyPercentage = other.CarrierWaveDutyPercentage;

            this.EnableLooping = other.EnableLooping;

            this.EnableIdleLevelOutput = other.EnableIdleLevelOutput;
            this.IdleLevel = other.IdleLevel;
        }
    }
}
