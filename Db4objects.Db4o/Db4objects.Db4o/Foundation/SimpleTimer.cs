namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class SimpleTimer : Sharpen.Lang.IRunnable
	{
		private readonly Sharpen.Lang.IRunnable _runnable;

		private readonly int _interval;

		public volatile bool stopped = false;

		public SimpleTimer(Sharpen.Lang.IRunnable runnable, int interval, string name)
		{
			_runnable = runnable;
			_interval = interval;
			Sharpen.Lang.Thread thread = new Sharpen.Lang.Thread(this);
			thread.SetDaemon(true);
			thread.SetName(name);
			thread.Start();
		}

		public virtual void Stop()
		{
			stopped = true;
		}

		public virtual void Run()
		{
			while (!stopped)
			{
				Db4objects.Db4o.Foundation.Cool.SleepIgnoringInterruption(_interval);
				if (!stopped)
				{
					_runnable.Run();
				}
			}
		}
	}
}
