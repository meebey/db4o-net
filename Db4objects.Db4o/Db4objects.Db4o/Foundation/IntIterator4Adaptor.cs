namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class IntIterator4Adaptor : Db4objects.Db4o.Foundation.IIntIterator4
	{
		private readonly System.Collections.IEnumerator _iterator;

		public IntIterator4Adaptor(System.Collections.IEnumerator iterator)
		{
			_iterator = iterator;
		}

		public IntIterator4Adaptor(System.Collections.IEnumerable iterable) : this(iterable
			.GetEnumerator())
		{
		}

		public virtual int CurrentInt()
		{
			return ((int)Current);
		}

		public virtual object Current
		{
			get
			{
				return _iterator.Current;
			}
		}

		public virtual bool MoveNext()
		{
			return _iterator.MoveNext();
		}

		public virtual void Reset()
		{
			_iterator.Reset();
		}
	}
}
