namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Iterator4Impl : System.Collections.IEnumerator
	{
		private Db4objects.Db4o.Foundation.List4 _first;

		private Db4objects.Db4o.Foundation.List4 _next;

		private object _current;

		public Iterator4Impl(Db4objects.Db4o.Foundation.List4 first)
		{
			_first = first;
			_next = first;
			_current = Db4objects.Db4o.Foundation.Iterators.NO_ELEMENT;
		}

		public virtual bool MoveNext()
		{
			if (_next == null)
			{
				_current = Db4objects.Db4o.Foundation.Iterators.NO_ELEMENT;
				return false;
			}
			_current = _next._element;
			_next = _next._next;
			return true;
		}

		public virtual object Current
		{
			get
			{
				if (Db4objects.Db4o.Foundation.Iterators.NO_ELEMENT == _current)
				{
					throw new System.InvalidOperationException();
				}
				return _current;
			}
		}

		public virtual void Reset()
		{
			_next = _first;
			_current = Db4objects.Db4o.Foundation.Iterators.NO_ELEMENT;
		}
	}
}
