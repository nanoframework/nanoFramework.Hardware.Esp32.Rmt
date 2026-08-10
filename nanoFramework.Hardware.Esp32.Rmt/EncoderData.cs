//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: Rmt nano

using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Data to send to an encoder.
    /// </summary>
    public class EncoderData
    {
        byte[] _data = null;
        int _loop = 1;

        /// <summary>
        /// Construct EncoderData object with data and loop parameters. 
        /// </summary>
        /// <param name="data">Data to pass to encoder</param>
        /// <param name="loop">Number times to loop data on output, default = 1</param>
        public EncoderData(byte[] data, int loop = 1)
        {
            Data = data;
            Loop = loop;
        }

        /// <summary>
        /// The data to provide to encoder. Default is null, no data provided.
        /// ByteEncoder - Byte data to be encoded to output using ByteEncoder settings.
        /// CopyEncoder - RmtSymbols as a byte array. If null the RmtSymbols in CopyEncoderSettings will be used.
        /// </summary>
        public byte[] Data { get => _data; set => _data = value; }


        /// <summary>
        /// Get or Set the Loop value.
        /// Number of loops to repeat outputting data via encoder.
        /// Default is 1 loop.
        /// This is good for repeating a pattern without having to load the repeated pattern into the data.
        /// </summary>
        public int Loop
        {
            get => _loop;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _loop = value;
            }
        }
    }
}
