//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt

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
        private UInt32 _idleThreshold;
        private UInt32 _filterThreshold;
        private TimeSpan _receiveTimeout;
        private bool _enableDemodulation;
        private int _carrierWaveFrequency;
        private float _carrierWaveDutyPercentage;
        private bool _carrierLevel;
        private int _bufferSize;

        /// <summary>
        /// <para>Gets or sets the idle threshold after which the receiver will go into idle mode and the receive will complete.</para>
        /// </summary>
        /// <remarks>
        /// <para>The receive process finishes(goes idle) when no edges have been detected for the specified nanoseconds.</para>
        /// <para>So setting this property to a value of 200000 means the threshold is 200us.</para>
        /// 
        /// <para>Value cannot be set to 0 or greater than the maximum value supported by the specific ESP32 target. 
        /// Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets, this value is called signal_range_max_ns in the docs.</para>
        /// <para>Typically the value can not be set higher then 32,767,000,000,000 / channel resolution. Exception given on starting receive operation.</para>
        /// <para>For a resolution of 1Mhz, the maximum value is 32,767,000. (32.767ms)</para>
        /// 
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public UInt32 IdleThreshold
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
        /// <para>Gets or sets the minimum valid pulse duration for either high or low levels, specified in nanoseconds.</para>
        /// <para>It will ignore pulses shorter than the specified value.</para>
        /// </summary>
        /// <remarks>
        /// <para>Setting to a value like 100000 will cause the receiver channel to ignore any pulses that are shorter than 100 microseconds.</para>
        /// <para>The maximum value depends on the hardware capabilities of the specific ESP32 target.
        /// For a clock of 80mhz, the maximum value is 3199. (3.199us). Exception given on starting receive operation.</para>
        /// <para>Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets, this value is called signal_range_min_ns in the docs.</para>
        /// </remarks>
        public UInt32 FilterThreshold
        {
            get => _filterThreshold;
            set => _filterThreshold = value;
        }

        /// <summary>
        /// Gets or sets the timeout threshold for the <see cref="ReceiverChannel.Receive"/> call. Defaults to 1 second.
        /// </summary>
        public TimeSpan ReceiveTimeout
        {
            get => _receiveTimeout;
            set => _receiveTimeout = value;
        }

        /// <summary>
        /// Enables or disables demodulating the received signal.
        /// </summary>
        /// <remarks>
        /// This configuration is not available on the base ESP32 target and will be ignored. Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets.
        /// </remarks>
        public bool EnableDemodulation
        {
            get => _enableDemodulation;
            set => _enableDemodulation = value;
        }

        /// <summary>
        /// Gets or sets the carrier wave frequency. Only applicable when <see cref="EnableDemodulation"/> is set to <see langword="true" />.
        /// </summary>
        /// <remarks>
        /// This configuration is not available on the base ESP32 target and will be ignored. Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets.
        /// </remarks>
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
        /// Gets or sets the carrier wave duty cycle percentage. Only applicable when <see cref="EnableDemodulation"/> is set to <see langword="true" />.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be 0 or less, greater than 100.</exception>
        /// <remarks>
        /// This configuration is not available on the base ESP32 target and will be ignored. Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets.
        /// </remarks>
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
        /// Gets or sets a value indicating at which level of RMT output is the carrier wave applied. Only applicable when <see cref="EnableDemodulation"/> is set to <see langword="true" />.
        /// <see langword="true" /> = HIGH.
        /// </summary>
        /// <remarks>
        /// This configuration is not available on the base ESP32 target and will be ignored. Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets.
        /// </remarks>
        public bool CarrierLevel
        {
            get => _carrierLevel;
            set => _carrierLevel = value;
        }

        /// <summary>
        /// Gets or sets the RMT Receive Buffer size.
        /// </summary>
        /// <remarks>
        /// Incoming <see cref="RmtSymbol"/>s are saved in the buffer. Any <see cref="RmtSymbol"/>s when full will be ignored.
        /// Receive of <see cref="RmtSymbol"/> completes after the <see cref="ReceiverChannelSettings.IdleThreshold"/> has lapsed.
        /// </remarks>
        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }   
                _bufferSize = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverChannelSettings"/> class using default settings.
        /// </summary>
        /// <remarks>
        /// <para>Default settings:</para>
        /// <list type="bullet">
        /// <item><description>IdleThreshold = 12000000 (12ms)</description></item>
        /// <item><description>FilterThreshold = 1200 (1.2us)</description></item>
        /// <item><description>ReceiveTimeout = 1 second</description></item>
        /// <item><description>EnableDemodulation = true</description></item>
        /// <item><description>CarrierWaveFrequency = 38_000</description></item>
        /// <item><description>CarrierWaveDutyPercentage = 33</description></item>
        /// <item><description>CarrierLevel = true</description></item>
        /// <item><description>BufferSize = 100</description></item>
        /// </list>
        /// </remarks>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        public ReceiverChannelSettings(int pinNumber) : base(pinNumber)
        {
            _idleThreshold = 12000000; //12ms
            _filterThreshold = 1200;   // 1.2us
            _receiveTimeout = TimeSpan.FromSeconds(1);
            _enableDemodulation = true;
            _carrierWaveFrequency = 38_000;
            _carrierWaveDutyPercentage = 33;
            _carrierLevel = true;
            _bufferSize = 100; // hold 100 RMT symbols.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverChannelSettings"/> class by copying values from the other <see cref="ReceiverChannelSettings"/> instance specified.
        /// </summary>
        /// <param name="other">The other <see cref="ReceiverChannelSettings"/> to copy values from.</param>
        internal ReceiverChannelSettings(ReceiverChannelSettings other) : base(other)
        {
            _idleThreshold = other.IdleThreshold;
            _filterThreshold = other.FilterThreshold;
            _receiveTimeout = other.ReceiveTimeout;
            _enableDemodulation = other.EnableDemodulation;
            _carrierWaveFrequency = other.CarrierWaveFrequency;
            _carrierWaveDutyPercentage = other.CarrierWaveDutyPercentage;
            _carrierLevel = other.CarrierLevel;
            _bufferSize = other.BufferSize;
        }
    }
}
