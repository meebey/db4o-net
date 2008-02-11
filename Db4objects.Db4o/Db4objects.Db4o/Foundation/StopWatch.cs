/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Sharpen;

namespace Db4objects.Db4o.Foundation
{
	public class StopWatch
	{
		private long _started;

		private long _elapsed;

		public StopWatch()
		{
		}

		public virtual void Start()
		{
			_started = Runtime.CurrentTimeMillis();
		}

		public virtual void Stop()
		{
			_elapsed = Peek();
		}

		public virtual long Peek()
		{
			return Runtime.CurrentTimeMillis() - _started;
		}

		public virtual long Elapsed()
		{
			return _elapsed;
		}
	}
}
