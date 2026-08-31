//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Class to encapsulate a RMT Symbol. 
    /// A RMT symbol is a pair of levels and durations that represent a single bit of data.
    /// </summary>
    public class RmtSymbol
    {
        /// <summary>
        /// Symbol level 0.
        /// </summary>
        private bool _level0 = true;

        /// <summary>
        /// Symbol level 1.
        /// </summary>
        private bool _level1 = false;

        /// <summary>
        /// Symbol duration 0.
        /// </summary>
        private ushort _duration0 = 1;

        /// <summary>
        /// Symbol duration 1.
        /// </summary>
        private ushort _duration1 = 0;

        /// <summary>
        /// Max value of rmt_item32_t::durationX (15 bit unsigned value).
        /// </summary>
        private const ushort MaxDuration = 32767;

        /// <summary>
        /// Create new rmt Symbol.
        /// </summary>
        /// <param name="duration0">Duration of the first level, in RMT channel ticks. Maximum value is 32767.</param>
        /// <param name="level0">Value of the first level. <see langword="true"/> = HIGH.</param>
        /// <param name="duration1">Duration of the second level, in RMT channel ticks. Maximum value is 32767.</param>
        /// <param name="level1">Value of the second level. <see langword="true"/> = HIGH.</param>
        public RmtSymbol(ushort duration0, bool level0, ushort duration1, bool level1)
        {
            Duration0 = duration0;
            Duration1 = duration1;
            _level0 = level0;
            _level1 = level1;
        }

        /// <summary>
        /// Level0 duration in RMT channel ticks
        /// </summary>
        public ushort Duration0
        {
            get => _duration0;
            set
            {
                CheckDuration(value);
                _duration0 = value;
            }
        }

        /// <summary>
        /// Level1 duration in RMT channel ticks
        /// </summary>
        public ushort Duration1
        {
            get => _duration1;
            set
            {
                CheckDuration(value);
                _duration1 = value;
            }
        }

        /// <summary>
        /// Level 0 value high/low.
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
        /// Level 1 value high/low.
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
        /// Check if the given duration exceeds max duration, Maximum supported duration is 32767.
        /// </summary>
        /// <param name="duration"></param>
        private static void CheckDuration(ushort duration)
        {
            if (duration > MaxDuration)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
