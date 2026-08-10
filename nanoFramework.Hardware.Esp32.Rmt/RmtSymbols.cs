//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano Rmt

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Class to encapsulate an array of RmtSymbols.
    /// </summary>
    public class RmtSymbols : IEnumerable
    {
        private readonly ArrayList _symbols = new ArrayList();

        // Holds the native representation of RmtSymbols as byte[].
        // This is used to avoid regenerating the native symbols on every call to Serialize().
        private byte[] _nativeSymbolsBuffer = null;

        /// <summary>
        /// Construct RmtSymbols object.
        /// </summary>
        public RmtSymbols()
        {
        }

        /// <summary>
        /// Construct RmtSymbols object with RmtSymbol array.
        /// </summary>
        /// <param name="symbolsArray">An array or RmtSymbols or null</param>
        public RmtSymbols(RmtSymbol[] symbolsArray)
        {
            if (symbolsArray != null)
            {
                foreach (RmtSymbol symbol in symbolsArray)
                {
                    _symbols.Add(symbol);
                }
            }
        }

        /// <summary>
        /// Count of number of added symbols.
        /// </summary>
        public int Count => _symbols.Count;

        /// <summary>
        /// Gets the internal ArrayList object.
        /// </summary>
        internal ArrayList Symbols => _symbols;

        /// <summary>
        /// Access a RmtSymbol from the array of symbols.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A RmtSymbol object.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public RmtSymbol this[int index]
        {
            get
            {
                if (index < 0 || index >= _symbols.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                var res = _symbols[index];
                return (RmtSymbol)res;
            }

            set
            {
                if (index < 0 || index >= _symbols.Count)
                {
#pragma warning disable S112 // OK to throw this here
                    throw new IndexOutOfRangeException();
#pragma warning restore S112 // General exceptions should never be thrown
                }

                _symbols[index] = value;
                _nativeSymbolsBuffer = null;
            }
        }

        /// <summary>
        /// Add new RMT symbol to the list of symbols that will be sent.
        /// </summary>
        /// <param name="cmd">RmtSymbol to Add</param>
        public void Add(RmtSymbol cmd)
        {
            _symbols.Add(cmd);
            _nativeSymbolsBuffer = null;
        }

        /// <summary>
        /// Clear list of symbols.
        /// </summary>
        public void Clear()
        {
            _symbols.Clear();
            _nativeSymbolsBuffer = null;
        }

        /// <summary>
        /// Serialize the symbols object to native symbols as byte buffer.
        /// </summary>
        /// <returns>Native RmtSymbol in a byte array.</returns>
        public byte[] Serialize()
        {
            // Regenerate symbols if required.
            if (_nativeSymbolsBuffer == null)
            {
                _nativeSymbolsBuffer = NativeRmtSymbolsToBytes();
            }
            return _nativeSymbolsBuffer;
        }

        /// <summary>
        /// GetEnumerator to allow enumeration of RmtSymbols using foreach loop.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Symbols).GetEnumerator();
        }

        #region native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern byte[] NativeRmtSymbolsToBytes();

        #endregion native stubs
    }
}
