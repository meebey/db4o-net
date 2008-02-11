/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class MarshallingContextState
	{
		public readonly MarshallingBuffer _buffer;

		public readonly int _fieldWriteCount;

		public MarshallingContextState(MarshallingBuffer buffer, int fieldWriteCount)
		{
			_buffer = buffer;
			_fieldWriteCount = fieldWriteCount;
		}
	}
}
