//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt Espressif

using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// A class used to Receive RMT symbols on ESP32.
    /// </summary>
    /// <remarks>
    /// For detailed explanation of ESP32 RMT Module, please check the Espressif official documentation here: https://docs.espressif.com/projects/esp-idf/en/v4.4.3/esp32/api-reference/peripherals/rmt.html
    /// </remarks>
    public class ReceiverChannel : RmtChannel, IDisposable
    {
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly ReceiverChannelSettings _receiverChannelSettings;

        /// <summary>
        /// Gets or sets the idle threshold after which the receiver will go into idle mode and the receive will complete.
        /// </summary>
        /// <remarks>
        /// The receive process finishes(goes idle) when no edges have been detected for the specified <see cref="IdleThreshold"/> nanoseconds.
        /// So setting this property to a value of 200000 means the threshold is 200us.
        /// </remarks>
        public UInt32 IdleThreshold
        {
            get => _receiverChannelSettings.IdleThreshold;
            set
            {
                _receiverChannelSettings.IdleThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold, in nanoseconds of the filter.
        /// It will ignore pulses shorter than the specified threshold.
        /// </summary>
        /// <remarks>
        /// Setting <see cref="FilterThreshold"/> to a value like 100000 will cause the receiver channel to ignore any pulses that are shorter than 100 microseconds.
        /// </remarks>
        public UInt32 FilterThreshold
        {
            get => _receiverChannelSettings.FilterThreshold;
            set
            {
                _receiverChannelSettings.FilterThreshold = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverChannel"/> class.
        /// </summary>
        /// <param name="settings">The channel settings to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> cannot be null.</exception>
        public ReceiverChannel(ReceiverChannelSettings settings) : base(settings)
        {
            _receiverChannelSettings = settings ?? throw new ArgumentNullException();
            _settings.Handle = NativeRxInit();
        }

        /// <summary>
        /// Starts continuous non-blocking receive mode.
        /// Symbols can be retrieved using <see cref="TryGetReceivedSymbols"/>.
        /// Throws <see cref="InvalidOperationException"/> if blocking receive is active.
        /// </summary>
        public void Start()
        {
            NativeStartReceive();
        }

        /// <summary>
        /// Stops continuous non-blocking receive mode.
        /// Throws <see cref="InvalidOperationException"/> if continuous mode is not active.
        /// </summary>
        public void Stop()
        {
            NativeStopReceive();
        }

        /// <summary>
        /// Attempts to retrieve received RMT symbols without blocking.
        /// Returns <c>null</c> if no symbols are available.
        /// Throws <see cref="InvalidOperationException"/> if Start is not active or Receive is active. 
        /// </summary>
        /// <returns>A <see cref="RmtSymbols"/> instance containing received symbols, or <c>null</c>.</returns>
        public RmtSymbols TryGetReceivedSymbols()
        {
            var arr = NativeTryGetReceived();
            if (arr == null || arr.Length == 0)
            {
                return null;
            }

            return new RmtSymbols(arr);
        }

        /// <summary>
        /// Performs a blocking receive operation.
        /// This call waits until symbols are received or a timeout occurs.
        /// Throws <see cref="InvalidOperationException"/> if StartReceive is active.
        /// </summary>
        /// <returns>A <see cref="RmtSymbols"/> instance containing received symbols, or <c>null</c>.
        /// </returns>
        public RmtSymbols Receive()
        {
            RmtSymbol[] symbols = NativeReceive();
            if (symbols == null || symbols.Length == 0)
            {
                return null;
            }
            return new RmtSymbols(symbols);
        }

        #region Destructor

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~ReceiverChannel() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable S4200 // Native methods should be wrapped
        protected virtual void Dispose(bool disposing) => NativeRxDispose();
#pragma warning restore S4200 // Native methods should be wrapped

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion Destructor

        #region Native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeRxInit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern RmtSymbol[] NativeReceive();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern RmtSymbol[] NativeTryGetReceived();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeRxDispose();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStartReceive();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStopReceive();

        #endregion  Native calls
    }
}
