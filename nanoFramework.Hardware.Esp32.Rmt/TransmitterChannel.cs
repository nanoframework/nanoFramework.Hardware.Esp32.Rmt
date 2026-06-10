//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// A class that can be used to create and transmit RMT symbols on ESP32
    /// </summary>
    /// <remarks>
    /// For detailed explanation of ESP32 RMT Module, please check the Espressif official documentation here: https://docs.espressif.com/projects/esp-idf/en/v4.4.3/esp32/api-reference/peripherals/rmt.html
    /// </remarks>
    public class TransmitterChannel : TransmitterEncodedChannel, IDisposable
    {
        #region Properties

        ///// <summary>
        ///// Gets a value indicating whether the channel is in idle mode.
        ///// </summary>
        //public bool IsChannelIdle
        //{
        //    get => base.IsChannelIdle;
        //}

        //internal Int32 Handle
        //{
        //    get => _transmitterChannelSettings.Handle;
        //}

        ///// <summary>
        ///// Gets or sets a value indicating whether to enable or disable looping through the ring buffer when transmitting <see cref="RmtSymbol"/>s.
        ///// </summary>
        //public bool EnableLooping
        //{
        //    get => _transmitterChannelSettings.EnableLooping;
        //    set
        //    {
        //        _transmitterChannelSettings.EnableLooping = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the maximum transmission loop count. Only applicable if <see cref="EnableLooping"/> is set to <see langword="true"/>.
        ///// </summary>
        ///// <remarks>
        ///// This configuration is not available on the base ESP32 target and will be ignored. Please refer to the ESP32 IDF docs for more information on feature availability for the various ESP32 targets.
        ///// </remarks>
        //public int LoopCount
        //{
        //    get => _transmitterChannelSettings.LoopCount;
        //    set
        //    {
        //        _transmitterChannelSettings.LoopCount = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets a value indicating at which level of RMT output is the carrier wave applied.
        ///// <see langword="true" /> = HIGH.
        ///// </summary>
        //public bool CarrierLevel
        //{
        //    get => _transmitterChannelSettings.CarrierLevel;
        //    set
        //    {
        //        NativeTxSetCarrierMode();
        //        _transmitterChannelSettings.CarrierLevel = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets a value indicating the RMT idle level.
        ///// <see langword="true" /> = HIGH.
        ///// </summary>
        //public bool IdleLevel
        //{
        //    get => _transmitterChannelSettings.IdleLevel;
        //    set
        //    {
        //        _transmitterChannelSettings.IdleLevel = value;
        //    }
        //}

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitterChannel"/> class.
        /// </summary>
        /// <param name="settings">The channel settings to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> cannot be null.</exception>
        public TransmitterChannel(TransmitChannelSettings settings) : base(settings, null)
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Send the filled RMT symbols to the transmitter
        /// </summary>
        /// <param name="symbols">RmtSymbols to encode and send.</param>
        /// <param name="waitTxDone">If true wait the TX process to end, false function returns without waiting, but if another symbol is send before the end of the previous process an error will occur.</param>
        public void Send(RmtSymbols symbols, bool waitTxDone = true)
        { 
            SendData(symbols.Serialize(), waitTxDone);
        }

        /// <summary>
        /// Send a RAW RmtSymbol data to RMT module
        /// </summary>
        /// <param name="data">byte array of data for TX module ready for native function</param>
        /// <param name="waitTxDone">Wait for TX to complete before returning.</param>
        public void SendData(byte[] data, bool waitTxDone = true)
        { 
            base.NativeTxWriteSymbolItems(data, waitTxDone);
        }
        #endregion Methods


#pragma warning restore S4200 // Native methods should be wrapped

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    }
}
