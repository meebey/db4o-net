namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class ReverseIntIterator4Impl : Db4objects.Db4o.Foundation.IIntIterator4
	{
		private readonly int _count;

		private int[] _content;

		private int _current;

		public ReverseIntIterator4Impl(int[] content, int count)
		{
			_content = content;
			_count = count;
			_current = count;
		}

		public virtual int CurrentInt()
		{
			if (_content == null || _current == _count)
			{
				throw new System.InvalidOperationException();
			}
			return _content[_current];
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
			if (_current > 0)
			{
				--_current;
				return true;
			}
			_content = null;
			return false;
		}

		public virtual void Reset()
		{
			_current = _count;
		}
	}
}
