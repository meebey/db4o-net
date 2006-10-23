namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public class Searcher
	{
		private int _lower;

		private int _upper;

		private int _cursor;

		private int _cmp;

		private readonly Db4objects.Db4o.Inside.Btree.SearchTarget _target;

		private readonly int _count;

		public Searcher(Db4objects.Db4o.Inside.Btree.SearchTarget target, int count)
		{
			if (count < 0)
			{
				throw new System.ArgumentException();
			}
			_target = target;
			_count = count;
			_cmp = -1;
			if (count == 0)
			{
				Complete();
				return;
			}
			_cursor = -1;
			_upper = count - 1;
			AdjustCursor();
		}

		private void AdjustBounds(int cmp)
		{
			_cmp = cmp;
			if (cmp > 0)
			{
				_upper = _cursor - 1;
				if (_upper < _lower)
				{
					_upper = _lower;
				}
				return;
			}
			if (cmp < 0)
			{
				if (_lower == _cursor && _lower < _upper)
				{
					_lower++;
				}
				else
				{
					_lower = _cursor;
				}
				return;
			}
			if (_target == Db4objects.Db4o.Inside.Btree.SearchTarget.ANY)
			{
				_lower = _cursor;
				_upper = _cursor;
			}
			else
			{
				if (_target == Db4objects.Db4o.Inside.Btree.SearchTarget.HIGHEST)
				{
					_lower = _cursor;
				}
				else
				{
					if (_target == Db4objects.Db4o.Inside.Btree.SearchTarget.LOWEST)
					{
						_upper = _cursor;
					}
					else
					{
						throw new System.InvalidOperationException("Unknown target");
					}
				}
			}
		}

		private void AdjustCursor()
		{
			int oldCursor = _cursor;
			if (_upper - _lower <= 1)
			{
				if ((_target == Db4objects.Db4o.Inside.Btree.SearchTarget.LOWEST) && (_cmp == 0))
				{
					_cursor = _lower;
				}
				else
				{
					_cursor = _upper;
				}
			}
			else
			{
				_cursor = _lower + ((_upper - _lower) / 2);
			}
			if (_cursor == oldCursor)
			{
				Complete();
			}
		}

		public virtual bool AfterLast()
		{
			if (_count == 0)
			{
				return false;
			}
			return (_cursor == _count - 1) && _cmp < 0;
		}

		public virtual bool BeforeFirst()
		{
			return (_cursor == 0) && (_cmp > 0);
		}

		private void Complete()
		{
			_upper = -2;
		}

		public virtual int Count()
		{
			return _count;
		}

		public virtual int Cursor()
		{
			return _cursor;
		}

		public virtual bool FoundMatch()
		{
			return _cmp == 0;
		}

		public virtual bool Incomplete()
		{
			return _upper >= _lower;
		}

		public virtual void MoveForward()
		{
			_cursor++;
		}

		public virtual void ResultIs(int cmp)
		{
			AdjustBounds(cmp);
			AdjustCursor();
		}

		public virtual bool IsGreater()
		{
			return _cmp < 0;
		}
	}
}
