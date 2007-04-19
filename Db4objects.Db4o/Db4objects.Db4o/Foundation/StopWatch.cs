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
			_elapsed = Runtime.CurrentTimeMillis() - _started;
		}

		public virtual long Elapsed()
		{
			return _elapsed;
		}
	}
}
