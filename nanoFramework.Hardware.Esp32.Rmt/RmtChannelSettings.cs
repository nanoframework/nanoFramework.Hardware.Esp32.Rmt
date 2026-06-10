//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Base class for shared RMT Channel settings.
    /// </summary>
    public abstract class RmtChannelSettings
    {
        // The channel handle is used internally to identify the channel and updated by native code
        private int _handle;

        // The following fields are used to store the settings values and are updated by the properties of this class.
        // They are used to provide the current settings values to native code when needed.
        private int _pinNumber;
        private int _resolutionHz;
        private byte _numberOfMemoryBlocks;
        private bool _signalInverterEnabled;

        /// <summary>
        /// Gets or Sets the channel handle.
        /// </summary>
        internal int Handle
        {
            get => _handle; 
            set => _handle = value;
        }

        /// <summary>
        /// Gets or sets the GPIO pin number to be used with the specified channel.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be less than 0.</exception>
        public int PinNumber
        {
            get => _pinNumber;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _pinNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Resolution in Hertz.
        /// </summary>
        /// <remarks>
        /// The default value for resolution is 1Mhz = 1us ticks.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be less than 1.</exception>
        public int ResolutionHz
        {
            get => _resolutionHz;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _resolutionHz = value;
            }
        }

       /// <summary>
        /// Gets or sets the number of memory blocks available to the current channel.
        /// </summary>
        /// <remarks>
        /// <para>This function is used to configure the number of memory blocks allocated to a channel.
        /// The size of the memory block depends on the target ESP32 and is normally 64x32-bit or 48x32-bit, and the maximum 
        /// number of blocks that can be allocated is the number of channels available on target. </para>
        /// <para>List of memory block sizes for various ESP32 targets:</para>
        /// <list type="bullet">
        /// <item><description>ESP32: 8 Channels of 64x32-bit = 512x32-bit RAM block total</description></item>
        /// <item><description>ESP32_S2: 8 Channels of 64x32-bit = 512x32-bit RAM</description></item>
        /// <item><description>ESP32_S3: 8 Channels of 64x32-bit = 512x32-bit RAM</description></item>
        /// <item><description>ESP32_C3: 4 Channels of 48x32-bit = 192x32-bit RAM</description></item>
        /// <item><description>ESP32_C5: 4 Channels of 48x32-bit = 192x32-bit RAM</description></item>
        /// <item><description>ESP32_C6: 4 Channels of 48x32-bit = 192x32-bit RAM</description></item>
        /// <item><description>ESP32_P4: 8 Channels of 48x32-bit = 384x32-bit RAM</description></item>
        /// <item><description>ESP32_H2: 4 Channels of 48x32-bit = 192x32-bit RAM</description></item>
        /// </list>
        /// <para>This means that every channel gets a single memory block equaling 64x32-bit or 48x32-bit.
        /// If the number of memory blocks of one channel is set to a value greater than 1, this channel will occupy the memory block of the next channel.
        /// The first Channel can use at most 8/4 blocks of memory, accordingly last channel can only use one memory block.</para>
        /// </remarks>
        public byte NumberOfMemoryBlocks 
        { 
            get => _numberOfMemoryBlocks; 
            set
            {
                if (value < 1 || value > 8)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _numberOfMemoryBlocks = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the RMT module should invert the incoming/outgoing signal.
        /// </summary>
        /// <remarks>
        /// This works like an external inverter connected to the GPIO of certain RMT channel.
        ///  For RX channels this inverts the input signal and TX channels it inverts the output signal.
        /// </remarks>
        public bool SignalInverterEnabled 
        { 
            get => _signalInverterEnabled; 
            set => _signalInverterEnabled = value; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RmtChannelSettings"/> class.
        /// </summary>
        /// <param name="pinNumber">The GPIO Pin number to use with the channel.</param>
        protected RmtChannelSettings(int pinNumber)
        {
            _pinNumber = pinNumber;

            _resolutionHz = 1 * 1000 * 1000; //  1Mhz (1_000_000) = 1us clock
            _numberOfMemoryBlocks = 1; // default as per ESP32 IDF docs ( 64 symbols)
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RmtChannelSettings"/> class by copying values from the other <see cref="RmtChannelSettings"/> instance specified.
        /// </summary>
        /// <param name="other">The other <see cref="RmtChannelSettings"/> to copy values from.</param>
        internal RmtChannelSettings(RmtChannelSettings other)
        {
            _pinNumber = other.PinNumber;
            _resolutionHz = other.ResolutionHz;
            _numberOfMemoryBlocks = other.NumberOfMemoryBlocks;
            _signalInverterEnabled = other.SignalInverterEnabled;
        }
    }
}
