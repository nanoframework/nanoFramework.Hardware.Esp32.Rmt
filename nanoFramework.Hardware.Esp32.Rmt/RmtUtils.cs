//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt Utils

using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Static class contains general utilities related to coding and decoding RMT data.
    /// </summary>
    public static class Utils
    {
        #region Native calls

        /// <summary>
        /// Decodes a sequence of captured RMT symbols into a packed byte array.
        /// This method interprets each symbol as one bit, using the duration of the
        /// SPACE level (carrier OFF) to determine whether the bit is 0 or 1.
        /// </summary>
        /// <param name="symbols">
        /// The array of <see cref="RmtSymbol"/> structures captured by an RMT receiver.
        /// Each symbol contains two durations and two logic levels representing the
        /// waveform transitions.
        /// </param>
        /// <param name="thresholdMicroseconds">
        /// The duration threshold, in microseconds, used to classify a bit.
        /// Durations shorter than this value are treated as logical 0; durations
        /// equal to or longer than this value are treated as logical 1.
        /// </param>
        /// <param name="msbFirst">
        /// If true, bits are packed into each byte starting from the most significant bit.
        /// If false, bits are packed starting from the least significant bit.
        /// </param>
        /// <param name="symbolOffset">
        /// The index of the first symbol to decode. This allows skipping protocol‑specific
        /// headers, leaders, or preambles. Must be within the bounds of the symbol array.
        /// </param>
        /// <param name="symbolLength">
        /// The number of symbols to decode. If set to -1, all remaining symbols from
        /// <paramref name="symbolOffset"/> to the end of the array are decoded.
        /// </param>
        /// <param name="spaceIsHigh">
        /// Indicates which logic level represents SPACE (carrier OFF).
        /// If true, SPACE corresponds to logic HIGH; if false, SPACE corresponds to logic LOW.
        /// The duration of the SPACE level is used to determine the bit value.
        /// </param>
        /// <returns>
        /// A byte array containing the decoded bit stream, packed sequentially from the
        /// selected symbols. The array length is <c>(symbolLength + 7) / 8</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern byte[] DecodeSymbolArrayToBytes(
            RmtSymbol[] symbols,
            int thresholdMicroseconds,
            bool msbFirst,
            int symbolOffset,
            int symbolLength,
            bool spaceIsHigh
        );

        #endregion Native
    }
}
