using System;


namespace nanoFramework.Hardware.Esp32.Rmt
{
    public class RmtCommand
    {
        /// <summary>
        /// Command level 1
        /// </summary>
        private bool _level0 = true;

        /// <summary>
        /// Command level 2
        /// </summary>
        private bool _level1 = false;

        /// <summary>
        /// Command duration 1
        /// </summary>
        private UInt16 _duration0 = 1;

        /// <summary>
        /// Command duration 2
        /// </summary>
        private UInt16 _duration1 = 0;
        
        /// <summary>
        /// Max value of rmt_item32_t::durationX (15 bit unsigned value)
        /// </summary>
        private const UInt16 MaxDuration = 32767;

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
            _level0 = level1;
            _level1 = level2;
        }


        /// <summary>
		/// Level1 duration in RMT chanel ticks
		/// </summary>
		public UInt16 Duration0
        {
            get => _duration0;
            set
            {
                CheckDuration(value);
                _duration0 = value;
            }
        }

        /// <summary>
        /// Level2 duration in RMT chanel ticks
        /// </summary>
        public UInt16 Duration1
        {
            get => _duration1;
            set
            {
                CheckDuration(value);
                _duration1 = value;
            }
        }

        /// <summary>
        /// Level 0 value high/low
        /// </summary>
        public bool Level0
        {
            get => _level0;
            set
            {
                _level0 = value;
            }
        }

        /// <summary>
        /// Level 1 value high/low
        /// </summary>
        public bool Level1
        {
            get => _level1;
            set
            {
                _level1 = value;
            }
        }


        /// <summary>
        /// Check if the given duration exceeds max duration, Maximum supported duration is 32768
        /// </summary>
        /// <param name="duration"></param>
        private static void CheckDuration(UInt16 duration)
        {
            if (duration > MaxDuration)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

    }
}
