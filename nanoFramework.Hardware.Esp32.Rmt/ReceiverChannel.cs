//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Runtime.CompilerServices;


namespace nanoFramework.Hardware.Esp32.Rmt
{
	/// <summary>
	/// A class that can be used to Receive RMT items on ESP32
	/// </summary>
	public class ReceiverChannel : RmtChannel, IDisposable
	{
		#region Fields

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private TimeSpan _receiveTimeout = new TimeSpan(0, 0, 1); // 1 sec

		#endregion Fields

		/// <summary>
		/// The receive time-out used when calling GetAllItems. Default 1 second.
		/// </summary>
		public TimeSpan ReceiveTimeout
		{
			get => _receiveTimeout;
			set => _receiveTimeout = value;
		}
		/// <summary>
		/// Public constructor to create receiver channel object.
		/// </summary>
		/// <param name="Gpio">The GPIO pin number that we want to use for receiving</param>
		/// <param name="RmtBufferSize">The maximum number of RMT commands to be reserved in receiver input buffer. 
		/// Default 100.</param>
		public ReceiverChannel(int Gpio, int RmtBufferSize = 100)
		{
			// Default to 1MHZ tick ( 1us )
			_clockDivider = 80;

			// Initialise channel with GPIO and ring buffer size
			_channel = NativeRxInit(Gpio, RmtBufferSize);
		}

		/// <summary>
		/// Start receiving data on channel.
		/// </summary>
		public void Start(bool ClearBuffer)
		{
			NativeRxStart(ClearBuffer);
		}

		/// <summary>
		/// Stop receiving data on channel.
		/// </summary>
		public void Stop()
		{
			NativeRxStop();
		}

		/// <summary>
		/// Enable / Disable filter for channel.
		/// </summary>
		/// <param name="Enable">True to Enable filter</param>
		/// <param name="Threshold">Pulse width to ignore expressed in number of source clock cycles,
		/// Value between 1-255</param>

		public void EnableFilter(bool Enable, Byte Threshold)
		{
			NativeRxEnableFilter(Enable, Threshold);
		}

		/// <summary>
		/// Set the Idle Threshold in ticks.
		/// </summary>
		/// <Remarks>
		/// The receive process finishes(goes idle) when no edges have been detected for Idle Threshold clock cycles.</Remarks>
		/// <param name="Threshold">Value between 1 and 65535 (0xFFFF)</param>
		public void SetIdleThresold(UInt16 Threshold)
		{
			if (Threshold == 0)
				throw new IndexOutOfRangeException();

			NativeRxSetIdleThresold(Threshold);
		}

		/// <summary>
		/// Get all RmtCommand items available.
		/// </summary>
		/// <remarks>If no signal received in time-out period then empty array will be returned.</remarks>
		/// <returns>Return array of RMTCommand. <br/>
		/// If no signal received in time-out period then empty array will be returned.
		/// </returns>
		public RmtCommand[] GetAllItems()
		{
			return NativeRxGetAllItems();
		}
		#region Destructor

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		~ReceiverChannel() => Dispose(false);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

#pragma warning disable S4200 // Native methods should be wrapped
		protected virtual void Dispose(bool disposing) => NativeRxDispose();
#pragma warning restore S4200 // Native methods should be wrapped

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		#endregion Destructor

		#region Native calls

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int NativeRxInit(int gpio, int rmtBufferSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void NativeRxStart(bool ClearBuffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void NativeRxStop();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int NativeRxGetRingBufferCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern RmtCommand[] NativeRxGetAllItems();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void NativeRxEnableFilter(bool Enable, Byte Threshold);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void NativeRxSetIdleThresold(UInt16 Threshold);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void NativeRxDispose();

		#endregion  Native calls
	}

}
