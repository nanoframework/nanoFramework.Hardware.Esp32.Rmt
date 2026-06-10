//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt 

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Class to provide settings for the creation of a byte encoder. 
    /// </summary>
    public class CopyEncoderSettings : EncoderSettings
    {
        private readonly RmtSymbols _symbols = null;

        /// <summary>
        /// Construct a CopyEncoderSettings object with RmtSymbols object.
        /// </summary>
        /// <param name="symbols"></param>
        public CopyEncoderSettings(RmtSymbols symbols = null) : base(EncoderType.Copy)
        {
            _symbols = symbols;
            if (_symbols != null)
            {
                _symbols.Serialize();
            }
        }

        /// <summary>
        /// Construct a CopyEncoderSettings object with fixed array of RmtSymbols.
        /// </summary>
        /// <param name="symbols"></param>
        public CopyEncoderSettings(RmtSymbol[] symbols) : base(EncoderType.Copy)
        {
            _symbols = new(symbols);
            _symbols.Serialize();
        }
    }
}
