/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class IntIterator4Impl : IIntIterator4
	{
		private readonly int _count;

		private int[] _content;

		private int _current;

		public IntIterator4Impl(int[] content, int count)
		{
			_content = content;
			_count = count;
			Reset();
		}

		public virtual int CurrentInt()
		{
			if (_content == null || _current == _count)
			{
				throw new InvalidOperationException();
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
			if (_current < _count - 1)
			{
				_current++;
				return true;
			}
			_content = null;
			return false;
		}

		public virtual void Reset()
		{
			_current = -1;
		}
	}
}
