/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System.Diagnostics;
using Db4objects.Db4o.Monitoring.Internal;

namespace Db4objects.Db4o.Monitoring.CS
{
	public class NetworkingCounters
	{
		internal PerformanceCounter BytesSent()
		{
			if (null == _bytesSent)
			{
				_bytesSent = Db4oPerformanceCounterCategory.CounterForNetworkingBytesSentPerSec();
			}

			return _bytesSent;
		}

		internal PerformanceCounter BytesReceived()
		{
			if (null == _bytesReceived)
			{
				_bytesReceived = Db4oPerformanceCounterCategory.CounterForNetworkingBytesReceivedPerSec();
			}

			return _bytesReceived;
		}

		internal PerformanceCounter MessagesSent()
		{
			if (null == _messagesSent)
			{
				_messagesSent = Db4oPerformanceCounterCategory.CounterForNetworkingMessagesSentPerSec();
			}

			return _messagesSent;
		}
		
		public void Close()
		{
			Dispose(_bytesSent);
			Dispose(_bytesReceived);
			Dispose(_messagesSent);
		}

		private static void Dispose(PerformanceCounter counter)
		{
			if (null != counter)
			{
				counter.Dispose();
			}
		}

		private PerformanceCounter _bytesSent;
		private PerformanceCounter _bytesReceived;
		private PerformanceCounter _messagesSent;

	}
}

#endif