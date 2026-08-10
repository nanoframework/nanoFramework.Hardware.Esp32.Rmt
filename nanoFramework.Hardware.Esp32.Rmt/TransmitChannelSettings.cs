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
        private bool _enableCarrierWave;
        private bool _carrierLevel;
        private int _carrierWaveFrequency;
        private float _carrierWaveDutyPercentage;
        private bool _carrierAlwaysOn;
        private bool _enableLooping;
        private int _loopCount;
        private bool _enableIdleLevelOutput;
        private bool _idleLevel;

        /// <summary>
        /// Enables or disables the carrier wave generator in the RMT Hardware.
        /// </summary>
        public bool EnableCarrierWave
        {
            get => _enableCarrierWave;
            set => _enableCarrierWave = value;
        }

        /// <summary>
        /// Gets or sets a value indicating at which level of RMT output is the carrier wave applied.
        /// <see langword="true" /> = HIGH.
        /// </summary>
        public bool CarrierLevel
        {
            get => _carrierLevel;
            set => _carrierLevel = value;
        }

        /// <summary>
        /// Gets or sets the carrier wave frequency.
        /// </summary>
        public int CarrierWaveFrequency
        {
            get => _carrierWaveFrequency;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _carrierWaveFrequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the carrier wave duty cycle percentage.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot 0 or less or greater than 100.</exception>
        public float CarrierWaveDutyPercentage
        {
            get => _carrierWaveDutyPercentage;
            set
            {
                if (value <= 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _carrierWaveDutyPercentage = value;
            }
        }

        /// <summary>
        /// Gets or Sets the carrier always on flag.
        /// When true the carrier will be output when there is no transmission.
        /// </summary>
        public bool CarrierAlwaysOn
        {
            get => _carrierAlwaysOn;
            set => _carrierAlwaysOn = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable looping through the ring buffer when transmitting <see cref="RmtSymbol"/>s.
        /// </summary>
        public bool EnableLooping
        {
            get => _enableLooping;
            set => _enableLooping = value;
        }

        /// <summary>
        /// Gets or sets the maximum transmission loop count. Only applicable if <see cref="EnableLooping"/> is set to <see langword="true"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be less that 1 or greater than 1023.</exception>
        /// <remarks>
        /// This configuration is not available on the base ESP32 target and will be ignored. Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets.
        /// </remarks>
        public int LoopCount
        {
            get => _loopCount;
            set
            {
                if (value < 1 || value > 1023)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _loopCount = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable the idle level output.
        /// </summary>
        public bool EnableIdleLevelOutput
        {
            get => _enableIdleLevelOutput;
            set => _enableIdleLevelOutput = value;
        }

        /// <summary>
        /// Gets or sets a value indicating the RMT idle level.
        /// <see langword="true" /> = HIGH.
        /// </summary>
        public bool IdleLevel
        {
            get => _idleLevel;
            set => _idleLevel = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitChannelSettings"/> class.
        /// </summary>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        public TransmitChannelSettings(int pinNumber) : base(pinNumber)
        {
            _enableCarrierWave = false;
            _carrierLevel = true;
            _carrierWaveFrequency = 38_000;
            _carrierWaveDutyPercentage = 33;
            _carrierAlwaysOn = false;

            _enableLooping = false;
            _loopCount = 1;

            _enableIdleLevelOutput = true;
            _idleLevel = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitChannelSettings"/> class by copying values from the other <see cref="TransmitChannelSettings"/> instance specified.
        /// </summary>
        /// <param name="other">The other <see cref="TransmitChannelSettings"/> to copy values from.</param>
        internal TransmitChannelSettings(TransmitChannelSettings other) : base(other)
        {
            _enableCarrierWave = other.EnableCarrierWave;
            _carrierLevel = other.CarrierLevel;
            _carrierWaveFrequency = other.CarrierWaveFrequency;
            _carrierWaveDutyPercentage = other.CarrierWaveDutyPercentage;
            _carrierAlwaysOn = other.CarrierAlwaysOn;

            _enableLooping = other.EnableLooping;
            _loopCount = other.LoopCount;

            _enableIdleLevelOutput = other.EnableIdleLevelOutput;
            _idleLevel = other.IdleLevel;
        }
    }
}
