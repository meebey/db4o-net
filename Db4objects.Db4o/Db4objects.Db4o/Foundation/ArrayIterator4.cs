namespace Db4objects.Db4o.Foundation
{
	public class ArrayIterator4 : System.Collections.IEnumerator
	{
		internal object[] _elements;

		internal int _next;

		public ArrayIterator4(object[] elements)
		{
			_elements = elements;
			_next = -1;
		}

		public virtual bool MoveNext()
		{
			if (_next < LastIndex())
			{
				++_next;
				return true;
			}
			_next = _elements.Length;
			return false;
		}

		public virtual object Current
		{
			get
			{
				return _elements[_next];
			}
		}

		public virtual void Reset()
		{
			_next = -1;
		}

		private int LastIndex()
		{
			return _elements.Length - 1;
		}
	}
}
