using System;


namespace nanoFramework.Hardware.Esp32.Rmt
{
    public class RmtCommand
    {
        /// <summary>
        /// Command level 1
        /// </summary>
        public bool level0= true;

        /// <summary>
        /// Command level 2
        /// </summary>
        public bool level1 = false;

        /// <summary>
        /// Command duration 1
        /// </summary>
        private UInt16 mDuration0 = 1;

        /// <summary>
        /// Command duration 2
        /// </summary>
        private UInt16 mDuration1 = 0;

        /// <summary>
        /// Equals with sizeof(rmt_item32_t)
        /// </summary>
        public const int SerialisedSize = 4;

        /// <summary>
        /// Max value of rmt_item32_t::durationX (15 bit unsigned value)
        /// </summary>
        public const UInt16 MaxDuration = 32767;

        /// <summary>
        /// Create new rmt command
        /// </summary>
        /// <param name="duration1"></param>
        /// <param name="level1"></param>
        /// <param name="duration2"></param>
        /// <param name="level2"></param>
        public RmtCommand(UInt16 duration1, bool level1, UInt16 duration2, bool level2)
        {
            Duration0 = duration1;
            Duration1 = duration2;
            level0 = level0;
            level1 = level1;
        }


        /// <summary>
		/// Level1 duration in RMT chanel ticks
		/// </summary>
		public UInt16 Duration0
        {
            get => mDuration0;
            set
            {
                CheckDuration(value);
                mDuration0 = value;
            }
        }

        /// <summary>
        /// Level2 duration in RMT chanel ticks
        /// </summary>
        public UInt16 Duration1
        {
            get => mDuration1;
            set
            {
                CheckDuration(value);
                mDuration1 = value;
            }
        }

        
        /// <summary>
        /// Check if the given duration exceeds max duration
        /// </summary>
        /// <param name="duration"></param>
        private static void CheckDuration(UInt16 duration)
        {
            if (duration > MaxDuration)
            {
                throw new ArgumentOutOfRangeException("Maximum supported duration is 32768");
            }
        }

    }
}
