/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ObjectHeaderAttributes : IFieldListInfo
	{
		private readonly int _fieldCount;

		private readonly BitMap4 _nullBitMap;

		public ObjectHeaderAttributes(ByteArrayBuffer reader)
		{
			_fieldCount = reader.ReadInt();
			_nullBitMap = reader.ReadBitMap(_fieldCount);
		}

		public virtual bool IsNull(int fieldIndex)
		{
			return _nullBitMap.IsTrue(fieldIndex);
		}
	}
}
