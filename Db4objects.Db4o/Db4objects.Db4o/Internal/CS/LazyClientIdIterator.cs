namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class LazyClientIdIterator : Db4objects.Db4o.Foundation.IIntIterator4
	{
		private readonly Db4objects.Db4o.Internal.CS.LazyClientQueryResult _queryResult;

		private int _current;

		private int[] _ids;

		private readonly int _batchSize;

		private int _available;

		public LazyClientIdIterator(Db4objects.Db4o.Internal.CS.LazyClientQueryResult queryResult
			)
		{
			_queryResult = queryResult;
			_batchSize = queryResult.Config().PrefetchObjectCount();
			_ids = new int[_batchSize];
			_current = -1;
		}

		public virtual int CurrentInt()
		{
			if (_current < 0)
			{
				throw new System.InvalidOperationException();
			}
			return _ids[_current];
		}

		public virtual object Current
		{
			get
			{
				return CurrentInt();
			}
		}

		public virtual bool MoveNext()
		{
			if (_available < 0)
			{
				return false;
			}
			if (_available == 0)
			{
				_queryResult.FetchIDs(_batchSize);
				_available--;
				_current = 0;
				return (_available > 0);
			}
			_current++;
			_available--;
			return true;
		}

		public virtual void Reset()
		{
			_queryResult.Reset();
			_available = 0;
			_current = -1;
		}

		public virtual void LoadFromIdReader(Db4objects.Db4o.Internal.Buffer reader, int 
			count)
		{
			for (int i = 0; i < count; i++)
			{
				_ids[i] = reader.ReadInt();
			}
			if (count > 0)
			{
				_available = count;
				return;
			}
			_available = -1;
		}
	}
}
