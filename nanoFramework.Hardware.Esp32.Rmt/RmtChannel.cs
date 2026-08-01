//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt µs

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Base class for a RMT channel.
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
        /// Gets or sets the GPIO pin used with the current channel.
        /// </summary>
        public int Pin
        {
            get => _settings.PinNumber;
            set
            {
                _settings.PinNumber = value;
            }
        }

        /// <summary>
        /// Gets or Sets the resolution of the internal tick counter. This effects all RMT channels.
        /// The default value is 1000000 (1Mhz) giving a tick period of 1 µs.
        /// </summary>
        public int ResolutionHz
        {
            get => _settings.ResolutionHz;
            set
            {
                _settings.ResolutionHz = value;
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
            get => _settings.NumberOfMemoryBlocks;
            set
            {
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
    }
}
