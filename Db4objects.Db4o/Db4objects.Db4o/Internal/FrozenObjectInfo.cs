/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	public class FrozenObjectInfo : IObjectInfo
	{
		private readonly Db4oDatabase _sourceDatabase;

		private readonly long _uuidLongPart;

		private readonly long _id;

		private readonly long _version;

		private readonly object _object;

		public FrozenObjectInfo(object @object, long id, Db4oDatabase sourceDatabase, long
			 uuidLongPart, long version)
		{
			_sourceDatabase = sourceDatabase;
			_uuidLongPart = uuidLongPart;
			_id = id;
			_version = version;
			_object = @object;
		}

		private FrozenObjectInfo(ObjectReference @ref, VirtualAttributes virtualAttributes
			) : this(@ref == null ? null : @ref.GetObject(), @ref == null ? -1 : @ref.GetID(
			), virtualAttributes == null ? null : virtualAttributes.i_database, virtualAttributes
			 == null ? -1 : virtualAttributes.i_uuid, @ref == null ? 0 : @ref.GetVersion())
		{
		}

		public FrozenObjectInfo(Transaction trans, ObjectReference @ref) : this(@ref, @ref
			 == null ? null : @ref.VirtualAttributes(trans, true))
		{
		}

		public virtual long GetInternalID()
		{
			return _id;
		}

		public virtual object GetObject()
		{
			return _object;
		}

		public virtual Db4oUUID GetUUID()
		{
			if (_sourceDatabase == null)
			{
				return null;
			}
			return new Db4oUUID(_uuidLongPart, _sourceDatabase.GetSignature());
		}

		public virtual long GetVersion()
		{
			return _version;
		}

		public virtual long SourceDatabaseId(Transaction trans)
		{
			if (_sourceDatabase == null)
			{
				return -1;
			}
			return _sourceDatabase.GetID(trans);
		}

		public virtual long UuidLongPart()
		{
			return _uuidLongPart;
		}
	}
}
