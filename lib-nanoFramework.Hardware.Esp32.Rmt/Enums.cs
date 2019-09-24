using System;

namespace nanoFramework.Hardware.Esp32.Rmt
{ 
    

    public enum MemoryBlockOwner
    {
        Transmitter = 0,
        Receiver
    }

    public enum SourceClock
    {
        [Obsolete("Not yet supported")]
        ReferenceOneMHz = 0, // Not used
        ApbEightyMHz
    }

    public enum DataMode
    {
        FIFO = 0,
        Mem
    }

    public enum ChannelMode
    {
        Transmitter = 0,
        Receiver
    }

    public enum IdleLevel
    {         
        Low = 0,
        High
    }

    public enum CarrierLevel
    {
        Low = 0,
        High
    }

    public enum ChannelStatus
    {
        Uninitialized = 0,
        Idle,
        Busy
    }

    public enum RmtChannelOpenStatus
    {
        /// <summary>
        /// The channel is already in use.
        /// </summary>
        ChannelUnavailable,
        /// <summary>
        /// Memory allocation failure
        /// </summary>
        NoMemory,
        /// <summary>
        /// Parameter error
        /// </summary>
        InvalidArgument,
        /// <summary>
        /// Channel has been sucessfully opened.
        /// </summary>
        ChannelOpened,        
        UnknownError
    }


}