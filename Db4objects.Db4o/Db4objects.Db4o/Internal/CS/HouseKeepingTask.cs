/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	public class HouseKeepingTask : IRunnable
	{
		private readonly ObjectServerImpl _server;

		public HouseKeepingTask(ObjectServerImpl server)
		{
			_server = server;
		}

		public virtual void Run()
		{
			BroadcastPing();
		}

		private void BroadcastPing()
		{
			_server.BroadcastMsg(Msg.PING, new _IBroadcastFilter_19(this));
		}

		private sealed class _IBroadcastFilter_19 : IBroadcastFilter
		{
			public _IBroadcastFilter_19(HouseKeepingTask _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Accept(IServerMessageDispatcher dispatcher)
			{
				return dispatcher.IsPingTimeout();
			}

			private readonly HouseKeepingTask _enclosing;
		}
	}
}
