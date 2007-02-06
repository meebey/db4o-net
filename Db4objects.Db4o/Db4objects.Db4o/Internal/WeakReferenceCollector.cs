namespace Db4objects.Db4o.Internal
{
	internal class WeakReferenceCollector : Sharpen.Lang.IRunnable
	{
		internal readonly object _queue;

		private readonly Db4objects.Db4o.Internal.ObjectContainerBase _stream;

		private Db4objects.Db4o.Foundation.SimpleTimer _timer;

		public readonly bool _weak;

		internal WeakReferenceCollector(Db4objects.Db4o.Internal.ObjectContainerBase a_stream
			)
		{
			_stream = a_stream;
			_weak = (!(a_stream is Db4objects.Db4o.Internal.TransportObjectContainer) && Db4objects.Db4o.Internal.Platform4
				.HasWeakReferences() && a_stream.ConfigImpl().WeakReferences());
			_queue = _weak ? Db4objects.Db4o.Internal.Platform4.CreateReferenceQueue() : null;
		}

		internal virtual object CreateYapRef(Db4objects.Db4o.Internal.ObjectReference a_yo
			, object obj)
		{
			if (!_weak)
			{
				return obj;
			}
			return Db4objects.Db4o.Internal.Platform4.CreateYapRef(_queue, a_yo, obj);
		}

		internal virtual void PollReferenceQueue()
		{
			if (_weak)
			{
				Db4objects.Db4o.Internal.Platform4.PollReferenceQueue(_stream, _queue);
			}
		}

		public virtual void Run()
		{
			PollReferenceQueue();
		}

		internal virtual void StartTimer()
		{
			if (!_weak)
			{
				return;
			}
			if (!_stream.ConfigImpl().WeakReferences())
			{
				return;
			}
			if (_stream.ConfigImpl().WeakReferenceCollectionInterval() <= 0)
			{
				return;
			}
			if (_timer != null)
			{
				return;
			}
			_timer = new Db4objects.Db4o.Foundation.SimpleTimer(this, _stream.ConfigImpl().WeakReferenceCollectionInterval
				(), "db4o WeakReference collector");
		}

		internal virtual void StopTimer()
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
