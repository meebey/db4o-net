/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;
using System.Threading;

using Sharpen.Lang;

namespace Db4objects.Db4o.Foundation
{
	public sealed class SimpleTimer
	{
		private readonly Timer _timer;
		private bool _started = false;

		public SimpleTimer(IRunnable runnable, int interval, string name)
		{
			_timer = new Timer(new TimerCallback(Run), runnable, 0, interval);
		}

		public void Start()
		{
			_started = true;
		}

		private void Run(object state)
		{
			if (!_started) return;
			((IRunnable)state).Run();
		}

		public void Stop()
		{
			_timer.Dispose();
		}
	}
}
