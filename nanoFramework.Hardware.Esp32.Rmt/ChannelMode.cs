//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// RMT Channel Mode Enum.
    /// </summary>
    public enum ChannelMode : byte
    {
        /// <summary>
        /// The <see cref="RmtChannel"/> is in Receive mode.
        /// </summary>
        Receive = 0x00,

        /// <summary>
        /// The <see cref="RmtChannel"/> is in transmission mode.
        /// </summary>
        Transmit = 0x01,
    }
}
