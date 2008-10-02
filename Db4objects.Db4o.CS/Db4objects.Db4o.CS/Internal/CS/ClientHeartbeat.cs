/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class ClientHeartbeat : IRunnable
	{
		private SimpleTimer _timer;

		private readonly ClientObjectContainer _container;

		public ClientHeartbeat(ClientObjectContainer container)
		{
			_container = container;
			_timer = new SimpleTimer(this, Frequency(container.ConfigImpl()), "db4o client heartbeat"
				);
		}

		private int Frequency(Config4Impl config)
		{
			return Math.Min(config.TimeoutClientSocket(), config.TimeoutServerSocket()) / 2;
		}

		public virtual void Run()
		{
			_container.WriteMessageToSocket(Msg.Ping);
		}

		public virtual void Start()
		{
			_timer.Start();
		}

		public virtual void Stop()
		{
			if (_timer == null)
			{
				return;
			}
			_timer.Stop();
			_timer = null;
		}
	}
}
