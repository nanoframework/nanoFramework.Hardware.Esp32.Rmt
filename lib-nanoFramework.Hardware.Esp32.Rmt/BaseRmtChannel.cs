using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    public class BaseRmtChannel : IDisposable
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

        #endregion Properties

        #region Destructors

        ~BaseRmtChannel() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => NativeDispose(Channel);

        #endregion Destructors

        #region Stubs

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