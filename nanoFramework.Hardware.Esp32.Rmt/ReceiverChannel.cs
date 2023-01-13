//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// A class that can be used to Receive RMT items on ESP32
    /// </summary>
    public class ReceiverChannel : RmtChannel, IDisposable
    {
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly ReceiverChannelSettings _receiverChannelSettings;

        /// <inheritdoc/>
        public override ChannelMode Mode => ChannelMode.Receive;

        /// <summary>
        /// Gets or sets the idle threshold after which the receiver will go into idle mode 
        /// and <see cref="RmtCommand"/>s are copied into the ring buffer and availble to your code.
        /// </summary>
        public ushort IdleThreshold
        {
            get => _receiverChannelSettings.IdleThreshold;
            set
            {
                NativeRxSetIdleThresold(value);
                _receiverChannelSettings.IdleThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the the filter is enabled. 
        /// If enabled, the receiver will ignore pulses with widths less than specified in <see cref="FilterThreshold"/>.
        /// </summary>
        public bool EnableFilter
        {
            get => _receiverChannelSettings.EnableFilter;
            set
            {
                NativeRxEnableFilter(value, _receiverChannelSettings.FilterThreshold);
                _receiverChannelSettings.EnableFilter = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold for pulse widths to ignore when <see cref="EnableFilter"/> is set to <see langword="true"/>.
        /// </summary>
        public byte FilterThreshold
        {
            get => _receiverChannelSettings.FilterThreshold;
            set
            {
                NativeRxEnableFilter(_receiverChannelSettings.EnableFilter, value);
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
            _receiverChannelSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            _settings.Channel = NativeRxInit();
        }

        /// <summary>
        /// Start receiving data on channel.
        /// </summary>
        /// <param name="clearBuffer">Clears buffer before starting.</param>
        public void Start(bool clearBuffer)
        {
            NativeRxStart(clearBuffer);
        }

        /// <summary>
        /// Stop receiving data on channel.
        /// </summary>
        public void Stop()
        {
            NativeRxStop();
        }

        /// <summary>
        /// Get all RmtCommand items available.
        /// </summary>
        /// <remarks>If no signal received in time-out period then empty array will be returned.</remarks>
        /// <returns>Return array of RMTCommand. <br/>
        /// If no signal received in time-out period then empty array will be returned.
        /// </returns>
        public RmtCommand[] GetAllItems()
        {
            return NativeRxGetAllItems();
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
        private extern void NativeRxStart(bool clearBuffer);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeRxStop();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeRxGetRingBufferCount();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern RmtCommand[] NativeRxGetAllItems();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeRxEnableFilter(bool enable, byte threshold);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeRxSetIdleThresold(ushort threshold);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeRxDispose();

        #endregion  Native calls
    }
}
