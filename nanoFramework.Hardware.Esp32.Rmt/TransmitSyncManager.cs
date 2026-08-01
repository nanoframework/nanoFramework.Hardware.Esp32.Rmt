//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.Rmt
{
    /// <summary>
    /// Class to synchronize transmitter across multiple channels.
    /// </summary>
    public class TransmitSyncManager : IDisposable
    {
        private bool disposedValue;
        private TransmitterChannel[] _txChannelArray;

        /// <summary>
        /// Create Sync manager object to synchronize sending from an array of channels.
        /// </summary>
        /// <param name="TxChannelArray"></param>
        public TransmitSyncManager(TransmitterChannel[] TxChannelArray)
        {
            _txChannelArray = TxChannelArray;

            Int32[] channelHandles = new Int32[TxChannelArray.Length];
            int index = 0;

            foreach(TransmitterChannel tc in TxChannelArray)
            {
                channelHandles[index++] = tc.Handle;
            }

            NativeCreateSyncManager(channelHandles);
        }

        /// <summary>
        /// Reset SyncManager.
        /// The reset needs to be called before restarting a simultaneous transmission.
        /// </summary>
        public void Reset()
        {
            NativeResetSyncManager();
        }

        /// <summary>
        /// Dispose 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                NativeDisposeSyncManager();

                _txChannelArray = null;

                disposedValue = true;
            }
        }

        /// <summary>
        /// Finalize r for TransmitSyncManager.
        /// </summary>
        ~TransmitSyncManager()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Dispose TransmitSyncManager object.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region native calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeCreateSyncManager(Int32[] channelHandles);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeDisposeSyncManager();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeResetSyncManager();
        #endregion
    }
}
