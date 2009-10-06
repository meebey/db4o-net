/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using Db4objects.Db4o.Foundation.Network;

namespace Db4objects.Db4o.Monitoring.CS
{
	public class MonitoredServerSideClientSocket4 : MonitoredSocket4Base
	{
		public MonitoredServerSideClientSocket4(ISocket4 socket, NetworkingCounters counters) : base(socket)
		{
			_counters = counters;
		}

		protected override NetworkingCounters  Counters()
		{
			return _counters;
		}

		private readonly NetworkingCounters _counters;
	}
}

#endif