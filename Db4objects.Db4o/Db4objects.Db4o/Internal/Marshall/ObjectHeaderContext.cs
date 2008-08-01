/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ObjectHeaderContext : AbstractReadContext, IMarshallingInfo, IHandlerVersionContext
	{
		protected ObjectHeader _objectHeader;

		private int _aspectCount;

		public ObjectHeaderContext(Transaction transaction, IReadBuffer buffer, ObjectHeader
			 objectHeader) : base(transaction, buffer)
		{
			_objectHeader = objectHeader;
		}

		public ObjectHeaderAttributes HeaderAttributes()
		{
			return _objectHeader._headerAttributes;
		}

		public bool IsNull(int fieldIndex)
		{
			return HeaderAttributes().IsNull(fieldIndex);
		}

		public sealed override int HandlerVersion()
		{
			return _objectHeader.HandlerVersion();
		}

		public virtual void BeginSlot()
		{
		}

		// do nothing
		public virtual ContextState SaveState()
		{
			return new ContextState(Offset());
		}

		public virtual void RestoreState(ContextState state)
		{
			Seek(state._offset);
		}

		public virtual object ReadFieldValue(Db4objects.Db4o.Internal.ClassMetadata classMetadata
			, FieldMetadata field)
		{
			if (!classMetadata.SeekToField(this, field))
			{
				return null;
			}
			return field.Read(this);
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _objectHeader.ClassMetadata();
		}

		public virtual int AspectCount()
		{
			return _aspectCount;
		}

		public virtual void AspectCount(int count)
		{
			_aspectCount = count;
		}
	}
}
