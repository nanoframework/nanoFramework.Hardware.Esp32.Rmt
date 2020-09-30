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
        private bool _carrierEnabled = false;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ushort _carrierHighDuration = 1;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ushort _carrierLowDuration = 1;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _carrierLevel = false;


        private readonly ArrayList _commands = new ArrayList();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Add new RMT command to the list of commands that will be sent
        /// </summary>
        /// <param name="cmd">RmtCommand to Add</param>
        public void AddCommand(RmtCommand cmd)
        {
            _commands.Add(cmd);
        }

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

        ///// <summary>
        ///// True if the channel's memory is in low power mode; false otherwise.
        ///// </summary>
        //public bool IsMemoryInLowPowerMode
        //{
        //    get
        //    {
        //        return NativeGetMemoryLowPower(Channel);
        //    }
        //    set
        //    {
        //        NativeSetMemoryLowPower(Channel, value);
        //    }
        //}

        ///// <summary>
        ///// The number of 64 * 32-bit memory blocks held by the channel.
        ///// </summary>
        ///// <remarks>
        ///// The 8 channels share a pool of 512 32-bit memory blocks.
        ///// If a channel has more than one memory blocks, it will occupy
        ///// the memory blocks of subsequent channels. Thus, Channel 0 can
        ///// use at most 8 blocks and channel 7 one.
        ///// </remarks>

        //public byte MemoryBlockNumber
        //{
        //    get
        //    {
        //        return NativeGetMemoryBlockNumber(Channel);
        //    }
        //    set
        //    {
        //        if (value > (NumberRmtChannels - Channel))
        //        {
        //            throw new ArgumentOutOfRangeException();
        //        }
        //        NativeSetMemoryBlockNumber(Channel, value);
        //    }
        //}


        //public ChannelStatus Status
        //{
        //    get
        //    {
        //        return (ChannelStatus)NativeGetChannelStatus(Channel);
        //    }
        //}

        /// <summary>
        /// Is the carrier wave enabled?
        /// </summary>
        public bool CarrierEnabled
        {
            get => _carrierEnabled;
            set
            {
                _carrierEnabled = value;

                // update carrier configuration
                ConfigureCarrier();
            }
        }

        /// <summary>
        /// Is the channel idle?
        /// </summary>
        public bool IsChannelIdle
        {
            get => NativeGetIsChannelIdle();
            set => NativeSetIsChannelIdle(value);
        }

        /// <summary>
        /// The level of the channel when in an idle state.
        /// </summary>
        public bool IdleLevel
        {
            get => NativeGetIdleLevel();
            set => NativeSetIdleLevel(value);
        }

        /// <summary>
        /// The duration of the carrier wave's high pulse, in source clock ticks.
        /// </summary>
        public ushort CarrierHighDuration
        {
            get => _carrierHighDuration;
            set
            {
                _carrierHighDuration = value;

                // update carrier configuration
                ConfigureCarrier();
            }
        }

        /// <summary>
        /// The duration of the carrier wave's low pulse, in source clock ticks.
        /// </summary>
        public ushort CarrierLowDuration
        {
            get => _carrierLowDuration;
            set
            {
                _carrierLowDuration = value;

                // update carrier configuration
                ConfigureCarrier();
            }
        }

        /// <summary>
        /// The level of the carrier wave's.
        /// </summary>
        public bool CarrierLevel
        {
            get => _carrierLevel;
            set
            {
                _carrierLevel = value;

                // update carrier configuration
                ConfigureCarrier();
            }
        }

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
                //First pair
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

                //Second pair
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

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Public constructor to create Transmitter object
        /// </summary>
        /// <param name="gpio">The GPIO pin number that we want to use for transmitting</param>
        public TransmitterChannel(int gpio)
        {
            NativeInit(gpio);

            ConfigureCarrier();
        }

        #endregion Constructor

        #region Destructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~TransmitterChannel() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable S4200 // Native methods should be wrapped
        protected virtual void Dispose(bool disposing) => NativeDispose();
#pragma warning restore S4200 // Native methods should be wrapped

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion Destructors

        #region Methods

        /// <summary>
        /// Configures the carrier's settings.
        /// </summary>
        private void ConfigureCarrier() => NativeSetCarrierMode();

        ///// <summary>
        ///// Fill the channel's memory with RmtItems
        ///// </summary>
        ///// <param name="items">Array of items to write to memory</param>
        ///// <param name="offset">Offset index of the channel's memory</param>
        //private uint FillItems(RmtItem[] items, ushort offset) =>
        //    NativeTxFillItems(Channel, items, offset);

        ///// <summary>
        ///// Start transmitting RmtItems from memory
        ///// </summary>
        ///// <param name="resetIndex">If true, reset the memory index; otherwise continue from the last index in memory</param>
        //private uint Start(bool resetIndex) =>
        //    NativeTxStart(Channel, resetIndex);

        ///// <summary>
        ///// Stop transmitting RmtItems.
        ///// </summary>
        //private uint Stop() => NativeTxStop(Channel);

        /// <summary>
        /// Send the filled RMT commands to the transmitter
        /// </summary>
        /// <param name="waitTxDone">If true wait the TX process to end, false function returns without waiting, but if another command is send before the end of the previous process an error will occur.</param>
        public void Send(bool waitTxDone) => SendData(SerializeCommands(), waitTxDone);

        /// <summary>
        /// Send a RAW data to RMT module
        /// </summary>
        /// <param name="data">byte array of data for tx module ready for native function</param>
        /// <param name="waitTxDone"></param>
#pragma warning disable S4200 // Native methods should be wrapped
        public void SendData(byte[] data, bool waitTxDone) => NativeWriteItems(data, waitTxDone);
#pragma warning restore S4200 // Native methods should be wrapped

        private uint WaitTxDone(int waitTime) =>
            NativeWaitTxDone(waitTime);

        #endregion Methods

        #region native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeInit(int gpio);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeGetIdleLevel();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeGetIsChannelIdle();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetIsChannelIdle(bool value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetIdleLevel(bool value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetCarrierMode();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern uint NativeTxFillItems(int channel, RmtItem[] items, ushort offset);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern uint NativeTxStart(int channel, bool resetIndex);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern uint NativeTxStop(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern uint NativeWriteItems(byte[] items, bool waitTxDone);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern uint NativeWaitTxDone(int waitType);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeDispose();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern int NativeGetChannelStatus(int channel);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern byte NativeGetMemoryBlockNumber(int channel);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern bool NativeGetMemoryLowPower(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeGetSourceClock();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern void NativeSetMemoryBlockNumber(int channel, byte value);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private extern void NativeSetMemoryLowPower(int channel, bool value);

        #endregion Stubs
    }
}