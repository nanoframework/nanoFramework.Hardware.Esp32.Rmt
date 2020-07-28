//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Enum for source Clock types
    /// </summary>
    public enum SourceClock
    {
        /// <summary>
        ///  80MHz clock source.
        /// </summary>
        /// <remarks>
        /// Currently this is the only supported mode on ESP32.
        /// </remarks>
        APB,

        /// <summary>
        /// Not supported
        /// </summary>
        REF
    }
}
