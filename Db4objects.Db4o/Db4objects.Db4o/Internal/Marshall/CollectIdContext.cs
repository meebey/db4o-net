/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class CollectIdContext : ObjectHeaderContext, IMarshallingInfo, IHandlerVersionContext
	{
		private readonly IdObjectCollector _collector = new IdObjectCollector();

		public CollectIdContext(Transaction transaction, ObjectHeader oh, IReadBuffer buffer
			) : base(transaction, buffer, oh)
		{
		}

		public virtual void AddId()
		{
			int id = ReadInt();
			if (id <= 0)
			{
				return;
			}
			AddId(id);
		}

		private void AddId(int id)
		{
			_collector.AddId(id);
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _objectHeader.ClassMetadata();
		}

		public virtual Tree Ids()
		{
			return _collector.Ids();
		}

		public virtual void ReadID(IReadsObjectIds objectIDHandler)
		{
			ObjectID objectID = objectIDHandler.ReadObjectID(this);
			if (objectID.IsValid())
			{
				AddId(objectID._id);
			}
		}

		public virtual IdObjectCollector Collector()
		{
			return _collector;
		}
	}
}
