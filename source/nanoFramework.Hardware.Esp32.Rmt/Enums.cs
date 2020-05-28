using System;


namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Enum for source Clock types
    /// </summary>
    public enum SourceClockTypes
    {
        APB, // 80MHz, currently ESP32 only supported
        REF //Not Supported
    }
}
