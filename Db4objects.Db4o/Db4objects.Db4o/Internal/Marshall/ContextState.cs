/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ContextState
	{
		public readonly int _offset;

		public readonly int _currentSlot;

		public ContextState(int offset, int currentSlot)
		{
			_offset = offset;
			_currentSlot = currentSlot;
		}
	}
}
