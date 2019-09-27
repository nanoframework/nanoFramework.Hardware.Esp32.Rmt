using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    public class TransmitterChannel : BaseRmtChannel
    {
        #region Methods

        /// <summary>
        /// Create new Rmt channel in transmitter mode and connect it to the specified GPIO pin.
        /// </summary>
        /// <param name="gpio">The GPIO pin to connect the channel to.</param>
        /// <returns>A <c>TransmitterChannel</c> object.</returns>
        public static TransmitterChannel Register(int gpio)
        {
            int channel = NativeInitializeTransmitter(gpio);
            if (channel < 0)
            {
                throw new NotSupportedException($"No Rmt channels available, or GPIO{gpio} unavailable");
            }
            return new TransmitterChannel(channel);
        }

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

        public TransmitterChannel(int channel)
        {
            Channel = channel;
            ConfigureCarrier();
        }

        #endregion Constructor

        #region Stubs

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int NativeInitializeTransmitter(int gpio);

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

        #endregion Stubs
    }
}