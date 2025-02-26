﻿//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// A class that can be used to create and transmit RMT commands on ESP32
    /// </summary>
    /// <remarks>
    /// For detailed explanation of ESP32 RMT Module, please check the Espressif official documentation here: https://docs.espressif.com/projects/esp-idf/en/v4.4.3/esp32/api-reference/peripherals/rmt.html
    /// </remarks>
    public class TransmitterChannel : RmtChannel, IDisposable
    {
        #region Fields

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly TransmitChannelSettings _transmitterChannelSettings;

        private readonly ArrayList _commands = new ArrayList();

        #endregion Fields

        #region Properties

        /// <inheritdoc/>
        public override ChannelMode Mode => ChannelMode.Transmit;

        /// <summary>
        /// Access a command from the array of commands that will be sent
        /// </summary>
        /// <param name="index">Index into RMTCommand array</param>
        /// <returns>RMT command from index</returns>
        public RmtCommand this[int index]
        {
            get
            {
                if (_commands.Count < index + 1)
                {
#pragma warning disable S112 // OK to throw this here
                    throw new IndexOutOfRangeException();
#pragma warning restore S112 // General exceptions should never be thrown
                }

                var res = _commands[index];
                return (RmtCommand)res;
            }

            set
            {
                if (_commands.Count < index + 1)
                {
#pragma warning disable S112 // OK to throw this here
                    throw new IndexOutOfRangeException();
#pragma warning restore S112 // General exceptions should never be thrown
                }

                _commands[index] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the channel is in idle mode.
        /// </summary>
        public bool IsChannelIdle
        {
            get => NativeTxGetIsChannelIdle();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable looping through the ring buffer when transmitting <see cref="RmtCommand"/>s.
        /// </summary>
        public bool EnableLooping
        {
            get => _transmitterChannelSettings.EnableLooping;
            set
            {
                NativeTxSetLoopingMode(value);
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
                NativeTxSetLoopCount(value);
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
                NativeTxSetIdleLevel(value);
                _transmitterChannelSettings.IdleLevel = value;
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitterChannel"/> class.
        /// </summary>
        /// <param name="settings">The channel settings to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> cannot be null.</exception>
        public TransmitterChannel(TransmitChannelSettings settings) : base(settings)
        {
            _transmitterChannelSettings = settings ?? throw new ArgumentNullException();
            _settings.Channel = NativeTxInit();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Add new RMT command to the list of commands that will be sent
        /// </summary>
        /// <param name="cmd">RmtCommand to Add</param>
        public void AddCommand(RmtCommand cmd)
        {
            _commands.Add(cmd);
        }

        /// <summary>
        /// Clear list of commands.
        /// </summary>
        public void ClearCommands()
        {
            _commands.Clear();
        }

        /// <summary>
        /// Send the filled RMT commands to the transmitter
        /// </summary>
        /// <param name="waitTxDone">If true wait the TX process to end, false function returns without waiting, but if another command is send before the end of the previous process an error will occur.</param>
        public void Send(bool waitTxDone)
            => SendData(RmtCommandSerializer.SerializeCommands(_commands), waitTxDone);

        /// <summary>
        /// Send a RAW data to RMT module
        /// </summary>
        /// <param name="data">byte array of data for tx module ready for native function</param>
        /// <param name="waitTxDone"></param>
#pragma warning disable S4200 // Native methods should be wrapped
        public void SendData(byte[] data, bool waitTxDone)
            => NativeTxWriteItems(data, waitTxDone);
#pragma warning restore S4200 // Native methods should be wrapped
        #endregion Methods

        #region Destructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~TransmitterChannel() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable S4200 // Native methods should be wrapped
        protected virtual void Dispose(bool disposing) => NativeTxDispose();
#pragma warning restore S4200 // Native methods should be wrapped

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion Destructors

        #region native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeTxInit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeTxGetIsChannelIdle();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeTxSetLoopingMode(bool enabled);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeTxSetLoopCount(int count);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeTxSetCarrierMode();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeTxSetIdleLevel(bool value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern uint NativeTxWriteItems(byte[] items, bool waitTxDone);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeTxDispose();

        #endregion Stubs
    }
}
