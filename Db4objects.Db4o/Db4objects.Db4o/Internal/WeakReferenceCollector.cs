/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	internal class WeakReferenceCollector : IRunnable
	{
		internal readonly object _queue;

		private readonly ObjectContainerBase _stream;

		private SimpleTimer _timer;

		public readonly bool _weak;

		internal WeakReferenceCollector(ObjectContainerBase a_stream)
		{
			_stream = a_stream;
			_weak = (!(a_stream is TransportObjectContainer) && Platform4.HasWeakReferences()
				 && a_stream.ConfigImpl().WeakReferences());
			_queue = _weak ? Platform4.CreateReferenceQueue() : null;
		}

		internal virtual object CreateYapRef(ObjectReference a_yo, object obj)
		{
			if (!_weak)
			{
				return obj;
			}
			return Platform4.CreateActiveObjectReference(_queue, a_yo, obj);
		}

		internal virtual void PollReferenceQueue()
		{
			if (!_weak)
			{
				return;
			}
			Platform4.PollReferenceQueue(_stream, _queue);
		}

		public virtual void Run()
		{
			try
			{
				PollReferenceQueue();
			}
			catch (Exception e)
			{
				// don't bring down the thread
				Sharpen.Runtime.PrintStackTrace(e);
			}
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
			_timer = new SimpleTimer(this, _stream.ConfigImpl().WeakReferenceCollectionInterval
				(), "db4o WeakReference collector");
			_timer.Start();
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
