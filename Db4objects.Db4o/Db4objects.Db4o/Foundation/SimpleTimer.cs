/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public sealed class SimpleTimer : IRunnable
	{
		private readonly IRunnable _runnable;

		private readonly int _interval;

		private readonly string _name;

		private Lock4 _lock;

		public volatile bool stopped = false;

		public SimpleTimer(IRunnable runnable, int interval, string name)
		{
			_runnable = runnable;
			_interval = interval;
			_name = name;
			_lock = new Lock4();
		}

		public void Start()
		{
			Thread thread = new Thread(this);
			thread.SetDaemon(true);
			thread.SetName(_name);
			thread.Start();
		}

		public void Stop()
		{
			stopped = true;
			lock (_lock)
			{
				_lock.Awake();
			}
		}

		public void Run()
		{
			while (!stopped)
			{
				lock (_lock)
				{
					_lock.Snooze(_interval);
				}
				if (!stopped)
				{
					_runnable.Run();
				}
			}
		}
	}
}
