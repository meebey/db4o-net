namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Stack4
	{
		private Db4objects.Db4o.Foundation.List4 _tail;

		public virtual void Push(object obj)
		{
			_tail = new Db4objects.Db4o.Foundation.List4(_tail, obj);
		}

		public virtual object Peek()
		{
			if (_tail == null)
			{
				return null;
			}
			return _tail._element;
		}

		public virtual object Pop()
		{
			if (_tail == null)
			{
				throw new System.InvalidOperationException();
			}
			object res = _tail._element;
			_tail = _tail._next;
			return res;
		}
	}
}
