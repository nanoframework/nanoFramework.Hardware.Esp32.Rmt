//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// A class that can be used to create and transmit RMT symbols on ESP32
    /// </summary>
    /// <remarks>
    /// For detailed explanation of ESP32 RMT Module, please check the 
    /// Espressif official documentation here: https://docs.espressif.com/projects/esp-idf/en/v5.4.4/esp32/api-reference/peripherals/rmt.html
    /// </remarks>
    public class TransmitterEncodedChannel : RmtChannel, IDisposable
    {
        #region Fields

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly TransmitChannelSettings _transmitterChannelSettings;
        private readonly EncoderSettings[] _encoderSettings;
        private EncoderData[] _encoderData;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the channel is in idle mode.
        /// </summary>
        public bool IsChannelIdle
        {
            get => NativeTxGetIsChannelIdle();
        }

        internal Int32 Handle
        {
            get => _transmitterChannelSettings.Handle;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable looping through the ring buffer when transmitting <see cref="RmtSymbol"/>s.
        /// </summary>
        public bool EnableLooping
        {
            get => _transmitterChannelSettings.EnableLooping;
            set
            {
                _transmitterChannelSettings.EnableLooping = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum transmission loop count. Only applicable if <see cref="EnableLooping"/> is set to <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// This configuration is not available on the base ESP32 target and will be ignored. Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets.
        /// </remarks>
        public int LoopCount
        {
            get => _transmitterChannelSettings.LoopCount;
            set
            {
                _transmitterChannelSettings.LoopCount = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating at which level of RMT output is the carrier wave applied.
        /// <see langword="true" /> = HIGH.
        /// </summary>
        public bool CarrierLevel
        {
            get => _transmitterChannelSettings.CarrierLevel;
            set
            {
                NativeTxSetCarrierMode();
                _transmitterChannelSettings.CarrierLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the RMT idle level.
        /// <see langword="true" /> = HIGH.
        /// </summary>
        public bool IdleLevel
        {
            get => _transmitterChannelSettings.IdleLevel;
            set
            {
                _transmitterChannelSettings.IdleLevel = value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitterChannel"/> class with a multi encoders.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="encoderSettings"></param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> cannot be null.</exception>
        public TransmitterEncodedChannel(TransmitChannelSettings settings, EncoderSettings[] encoderSettings) : base(settings)
        {
            _transmitterChannelSettings = settings ?? throw new ArgumentNullException();

            _encoderData = null;

            if (encoderSettings == null)
            {
                // If no encoderSettings provided then default to a single CopyEncoder.
                _encoderSettings = new EncoderSettings[] { new CopyEncoderSettings() };
            }
            else
            {
                _encoderSettings = encoderSettings;
            }

            _settings.Handle = NativeTxInit();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Send a data to RMT module via encoders.
        /// </summary>
        /// <param name="encoderData">
        /// An array of EncoderData used to provide data and parameters to encoders which require data. 
        /// If encoder step already has data then it will be skipped.
        /// </param>
        /// <param name="waitTxDone">Wait for TX to complete before returning.</param>
        /// <exception cref="ArgumentException" >The number of objects in array must equal number of encoders.</exception>
        public void SendWithEncoders(EncoderData[] encoderData, bool waitTxDone = true)
        {
            // Save to object, we want it to be available to native after this call ( waitTxDone = false )
            _encoderData = encoderData;

            NativeTxWriteEncoder(waitTxDone);
        }

        #endregion Methods

        #region Destructor

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~TransmitterEncodedChannel() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable S4200 // Native methods should be wrapped
        protected virtual void Dispose(bool disposing) => NativeTxDispose();
#pragma warning restore S4200 // Native methods should be wrapped

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion Destructor

        #region native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeTxInit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeTxGetIsChannelIdle();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeTxSetCarrierMode();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern uint NativeTxWriteSymbolItems(byte[] item, bool waitTxDone);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern uint NativeTxWriteEncoder(bool waitTxDone);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeTxDispose();

        #endregion Stubs
    }
}
