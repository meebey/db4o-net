/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class CollectIdContext : ObjectHeaderContext, IMarshallingInfo
	{
		private readonly string _fieldName;

		private TreeInt _ids;

		public CollectIdContext(Transaction transaction, ObjectHeader oh, IReadBuffer buffer
			, string fieldName) : base(transaction, buffer, oh)
		{
			_fieldName = fieldName;
		}

		public virtual string FieldName()
		{
			return _fieldName;
		}

		public virtual void AddId()
		{
			int id = ReadInt();
			if (id <= 0)
			{
				return;
			}
			_ids = (TreeInt)Tree.Add(_ids, new TreeInt(id));
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _objectHeader.ClassMetadata();
		}

		public virtual Tree Ids()
		{
			return _ids;
		}
	}
}
