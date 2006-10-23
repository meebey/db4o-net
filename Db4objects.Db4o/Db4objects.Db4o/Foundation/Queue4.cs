namespace Db4objects.Db4o.Foundation
{
	/// <summary>
	/// Using the CollectionElement the other way around:
	/// CollectionElement.i_next points to the previous element
	/// </summary>
	/// <exclude></exclude>
	public class Queue4
	{
		private Db4objects.Db4o.Foundation.List4 _first;

		private Db4objects.Db4o.Foundation.List4 _last;

		public void Add(object obj)
		{
			Db4objects.Db4o.Foundation.List4 ce = new Db4objects.Db4o.Foundation.List4(null, 
				obj);
			if (_first == null)
			{
				_last = ce;
			}
			else
			{
				_first._next = ce;
			}
			_first = ce;
		}

		public object Next()
		{
			if (_last == null)
			{
				return null;
			}
			object ret = _last._element;
			_last = _last._next;
			if (_last == null)
			{
				_first = null;
			}
			return ret;
		}

		public bool HasNext()
		{
			return _last != null;
		}
	}
}
