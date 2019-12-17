using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    public class TransmitterChannel : IDisposable
    {
        #region Constants

        /// <summary>
        /// The largest channel number. Channel numbers start at 0.
        /// </summary>
        public const byte MaxChannelNumber = NumberRmtChannels - 1;

        /// <summary>
        /// The number of RMT channels available.
        /// </summary>
        public const byte NumberRmtChannels = 8;

        #endregion Constants

        #region Methods

        /// <summary>
        /// Configures the carrier's settings.
        /// </summary>
        private void ConfigureCarrier() =>
            NativeSetCarrierMode(Channel, CarrierEnabled, CarrierHighDuration, CarrierLowDuration, (int)CarrierLevel);

        /// <summary>
        /// Fill the channel's memory with RmtItems
        /// </summary>
        /// <param name="items">Array of items to write to memory</param>
        /// <param name="offset">Offset index of the channel's memory</param>
        private uint FillItems(RmtItem[] items, ushort offset) =>
            NativeTxFillItems(Channel, items, offset);

        /// <summary>
        /// Start transmitting RmtItems from memory
        /// </summary>
        /// <param name="resetIndex">If true, reset the memory index; otherwise continue from the last index in memory</param>
        private uint Start(bool resetIndex) =>
            NativeTxStart(Channel, resetIndex);

        /// <summary>
        /// Stop transmitting RmtItems.
        /// </summary>
        private uint Stop() => NativeTxStop(Channel);

        /// <summary>
        /// Write RmtItems from array.
        /// </summary>
        /// <param name="items">RmtItems array to write.</param>
        /// <param name="waitTxDone">If true, function will block until transmission is complete; otherwise, the function returns immediately.</param>
        private uint WriteItems(RmtItem[] items, bool waitTxDone) =>
            NativeWriteItems(Channel, items, waitTxDone);

        private uint WaitTxDone(int waitTime) =>
            NativeWaitTxDone(Channel, waitTime);

        #endregion Methods

        #region Properties

        private int _channel;

        public int Channel
        {
            get
            {
                return _channel;
            }

            protected set
            {
                if (value > MaxChannelNumber)
                {
                    throw new ArgumentOutOfRangeException("Channel", $"The maximum channel number is {MaxChannelNumber}");
                }
                _channel = value;
            }
        }

        public byte ClockDivider
        {
            get
            {
                return NativeGetClockDivider(Channel);
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("ClockDivider", "Must be in the range 1..255");
                }
                NativeSetClockDivider(Channel, value);
            }
        }

        /// <summary>
        /// True if the channel's memory is in low power mode; false otherwise.
        /// </summary>
        public bool IsMemoryInLowPowerMode
        {
            get
            {
                return NativeGetMemoryLowPower(Channel);
            }
            set
            {
                NativeSetMemoryLowPower(Channel, value);
            }
        }

        /// <summary>
        /// The number of 64 * 32-bit memory blocks held by the channel.
        /// </summary>
        /// <remarks>
        /// The 8 channels share a pool of 512 32-bit memory blocks.
        /// If a channel has more than one memory blocks, it will occupy
        /// the memory blocks of subsequent channels. Thus, Channel 0 can
        /// use at most 8 blocks and channel 7 one.
        /// </remarks>

        public byte MemoryBlockNumber
        {
            get
            {
                return NativeGetMemoryBlockNumber(Channel);
            }
            set
            {
                if (value > (NumberRmtChannels - Channel))
                {
                    throw new ArgumentOutOfRangeException("MemoryBlockNumber", "");
                }
                NativeSetMemoryBlockNumber(Channel, value);
            }
        }

        /// <summary>
        /// The source clock. only the 80MHz APB clock is currently supported.
        /// </summary>
        public SourceClock SourceClock
        {
            get
            {
                return (SourceClock)NativeGetSourceClock(Channel);
            }
            set
            {
                NativeSetSourceClock(Channel, (int)value);
            }
        }

        public ChannelStatus Status
        {
            get
            {
                return (ChannelStatus)NativeGetChannelStatus(Channel);
            }
        }

        private readonly bool _carrierEnabled;

        /// <summary>
        /// Is the carrier wave enabled?
        /// </summary>
        public bool CarrierEnabled
        {
            get => _carrierEnabled;
            set => ConfigureCarrier();
        }

        /// <summary>
        /// Is the channel idle?
        /// </summary>
        public bool IsChannelIdle
        {
            get => NativeGetIsChannelIdle(Channel);
            set => NativeSetIsChannelIdle(Channel, value);
        }

        /// <summary>
        /// The level of the channel when in an idle state.
        /// </summary>
        private IdleLevel IdleLevel
        {
            get => (IdleLevel)NativeGetIdleLevel(Channel);
            set => NativeSetIdleLevel(Channel, (int)value);
        }

        private readonly UInt16 _carrierHighDuration;

        /// <summary>
        /// The duration of the carrier wave's high pulse, in source clock ticks.
        /// </summary>
        private UInt16 CarrierHighDuration
        {
            get => _carrierHighDuration;
            set => ConfigureCarrier();
        }

        private readonly UInt16 _carrierLowDuration;

        /// <summary>
        /// The duration of the carrier wave's low pulse, in source clock ticks.
        /// </summary>
        private ushort CarrierLowDuration
        {
            get => _carrierLowDuration;
            set => ConfigureCarrier();
        }

        private readonly CarrierLevel _carrierLevel;

        private CarrierLevel CarrierLevel
        {
            get => _carrierLevel;
            set => ConfigureCarrier();
        }

        #endregion Properties

        #region Constructor

        public TransmitterChannel(int channel, int gpio)
        {
            Channel = channel;
            NativeInitializeTransmitter(channel, gpio);
            ConfigureCarrier();
        }

        #endregion Constructor

        #region Destructors

        ~TransmitterChannel() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => NativeDispose(Channel);

        #endregion Destructors

        #region Stubs

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int NativeInitializeTransmitter(int channel, int gpio);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int NativeGetIdleLevel(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool NativeGetIsChannelIdle(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetIsChannelIdle(int channel, bool value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetIdleLevel(int channel, int value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetCarrierMode(int channel, bool isEnabled, ushort highDuration, ushort lowDuration, int carrierLevel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint NativeTxFillItems(int channel, RmtItem[] items, ushort offset);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint NativeTxStart(int channel, bool resetIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint NativeTxStop(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint NativeWriteItems(int channel, RmtItem[] items, bool waitTxDone);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint NativeWaitTxDone(int channel, int waitType);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeDispose(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int NativeGetChannelStatus(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern byte NativeGetClockDivider(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern byte NativeGetMemoryBlockNumber(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool NativeGetMemoryLowPower(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int NativeGetSourceClock(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetClockDivider(int channel, byte value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetMemoryBlockNumber(int channel, byte value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetMemoryLowPower(int channel, bool value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetSourceClock(int channel, int value);

        #endregion Stubs
    }
}