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
    /// A class that can be used to create and transmit RMT commands on ESP32
    /// </summary>
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
        /// <exception cref="ArgumentNullException"></exception>
        public TransmitterChannel(TransmitChannelSettings settings) : base(settings)
        {
            _transmitterChannelSettings = settings ?? throw new ArgumentNullException(nameof(settings));
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
            => SendData(SerializeCommands(), waitTxDone);

        /// <summary>
        /// Send a RAW data to RMT module
        /// </summary>
        /// <param name="data">byte array of data for tx module ready for native function</param>
        /// <param name="waitTxDone"></param>
#pragma warning disable S4200 // Native methods should be wrapped
        public void SendData(byte[] data, bool waitTxDone)
            => NativeTxWriteItems(data, waitTxDone);
#pragma warning restore S4200 // Native methods should be wrapped

        /// <summary>
        /// Serialize commands to rmt_item32_t native byte format
        /// </summary>
        /// <returns></returns>
        private byte[] SerializeCommands()
        {
            int i = 0;
            int remaining;
            byte[] binaryCommands = new byte[_commands.Count * 4];
            foreach (var cmd in _commands)
            {
                // First pair
                if ((cmd as RmtCommand).Duration0 <= 255)
                {
                    binaryCommands[0 + i] = (byte)(cmd as RmtCommand).Duration0;
                    binaryCommands[1 + i] = (byte)((cmd as RmtCommand).Level0 == true ? 128 : 0);
                }
                else
                {
                    remaining = (cmd as RmtCommand).Duration0 % 256;
                    binaryCommands[0 + i] = (byte)(remaining);
                    binaryCommands[1 + i] = (byte)(((cmd as RmtCommand).Level0 ? 128 : 0) + (((cmd as RmtCommand).Duration0 - remaining) / 256));
                }

                // Second pair
                if ((cmd as RmtCommand).Duration1 <= 255)
                {
                    binaryCommands[2 + i] = (byte)(cmd as RmtCommand).Duration1;
                    binaryCommands[3 + i] = (byte)((cmd as RmtCommand).Level1 ? 128 : 0);
                }
                else
                {
                    remaining = (cmd as RmtCommand).Duration1 % 256;
                    binaryCommands[2 + i] = (byte)(remaining);
                    binaryCommands[3 + i] = (byte)(((cmd as RmtCommand).Level1 ? 128 : 0) + (((cmd as RmtCommand).Duration1 - remaining) / 256));
                }
                i += 4;
            }
            return binaryCommands;
        }

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
