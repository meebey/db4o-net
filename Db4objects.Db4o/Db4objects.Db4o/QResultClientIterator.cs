namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	internal class QResultClientIterator : System.Collections.IEnumerator
	{
		private object[] _prefetchedObjects;

		private int _remainingObjects;

		private int _prefetchRight;

		private readonly Db4objects.Db4o.QResultClient _client;

		private readonly Db4objects.Db4o.Foundation.IIntIterator4 _ids;

		public QResultClientIterator(Db4objects.Db4o.QResultClient client)
		{
			_client = client;
			_ids = client.IterateIDs();
		}

		public virtual object Current
		{
			get
			{
				lock (StreamLock())
				{
					return _client.Activate(PrefetchedCurrent());
				}
			}
		}

		private object StreamLock()
		{
			return _client.StreamLock();
		}

		public virtual void Reset()
		{
			_remainingObjects = 0;
			_ids.Reset();
		}

		public virtual bool MoveNext()
		{
			lock (StreamLock())
			{
				if (_remainingObjects > 0)
				{
					--_remainingObjects;
					return SkipNulls();
				}
				Prefetch();
				--_remainingObjects;
				if (_remainingObjects < 0)
				{
					return false;
				}
				return SkipNulls();
			}
		}

		private bool SkipNulls()
		{
			if (PrefetchedCurrent() == null)
			{
				return MoveNext();
			}
			return true;
		}

		private void Prefetch()
		{
			EnsureObjectCacheAllocated(PrefetchCount());
			_remainingObjects = Stream().PrefetchObjects(_ids, _prefetchedObjects, PrefetchCount
				());
			_prefetchRight = _remainingObjects;
		}

		private int PrefetchCount()
		{
			return Stream().Config().PrefetchObjectCount();
		}

		private Db4objects.Db4o.YapClient Stream()
		{
			return (Db4objects.Db4o.YapClient)_client.i_trans.Stream();
		}

		private object PrefetchedCurrent()
		{
			return _prefetchedObjects[_prefetchRight - _remainingObjects - 1];
		}

		private void EnsureObjectCacheAllocated(int prefetchObjectCount)
		{
			if (_prefetchedObjects == null)
			{
				_prefetchedObjects = new object[prefetchObjectCount];
				return;
			}
			if (prefetchObjectCount > _prefetchedObjects.Length)
			{
				object[] newPrefetchedObjects = new object[prefetchObjectCount];
				System.Array.Copy(_prefetchedObjects, 0, newPrefetchedObjects, 0, _prefetchedObjects
					.Length);
				_prefetchedObjects = newPrefetchedObjects;
			}
		}
	}
}
