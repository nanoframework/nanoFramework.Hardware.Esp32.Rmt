//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt Msb

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Class to provide settings for the creation of a byte encoder. 
    /// </summary>
    public class ByteEncoderSettings : EncoderSettings
    {
        private RmtSymbol _bit0;
        private RmtSymbol _bit1;
        private bool _msbFirst;
        private int _byteLen;

        /// <summary>
        /// Returns true if the most significant bit is first.
        /// </summary>
        public bool MsbFirst => _msbFirst;

        /// <summary>
        /// Get the RMT symbol with how to represent the 0 bit.
        /// </summary>
        public RmtSymbol Bit0 => _bit0;

        /// <summary>
        /// Get the RMT symbol with how to represent the 1 bit.
        /// </summary>
        public RmtSymbol Bit1 => _bit1;


        /// <summary>
        /// Construct a ByteEncoderSettings object.
        /// </summary>
        /// <param name="Bit0">A Rmt symbol with the representation of the 0 bit.</param>
        /// <param name="Bit1">A Rmt symbol with the representation of the 1 bit.</param>
        /// <param name="MsbFirst">True if the most significant bit is first. Bit 7 of the byte.</param>
        /// <param name="byteLength">Length of data to be handled by encoder for multi stage encoding.
        /// If set to 0 then all remaining data in send will be processed. (default = 0)
        /// </param>
        public ByteEncoderSettings(RmtSymbol Bit0, RmtSymbol Bit1, bool MsbFirst, int byteLength = 0) : base(EncoderType.Byte)
        {
            _bit0 = Bit0;
            _bit1 = Bit1;
            _msbFirst = MsbFirst;
            _byteLen = byteLength;
        }
    }
}
