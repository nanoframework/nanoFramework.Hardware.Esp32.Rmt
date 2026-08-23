//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Types of encoders.
    /// </summary>
    public enum EncoderType : byte
    {
        /// <summary>
        /// No encoder specified.
        /// </summary>
        None,

        /// <summary>
        /// Copy RmtSymbols to output.
        /// </summary>
        Copy,

        /// <summary>
        /// Byte encoder.
        /// </summary>
        Byte,
    };


    /// <summary>
    /// Base class for all encoders.
    /// </summary>
    public abstract class EncoderSettings
    {
        readonly private EncoderType _encoderType = EncoderType.None;

        private protected EncoderSettings(EncoderType t)
        {
            _encoderType = t;
        }

        /// <summary>
        /// Get encoder type.
        /// </summary>
        public EncoderType Type => _encoderType;
    }
}
