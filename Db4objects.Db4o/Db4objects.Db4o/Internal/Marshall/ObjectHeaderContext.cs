/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class ObjectHeaderContext : AbstractReadContext
	{
		protected ObjectHeader _objectHeader;

		private int _currentSlot;

		protected ObjectHeaderContext(Transaction transaction, IReadBuffer buffer, ObjectHeader
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
			_currentSlot++;
		}

		public virtual int CurrentSlot()
		{
			return _currentSlot;
		}

		public virtual ContextState SaveState()
		{
			return new ContextState(Offset(), _currentSlot);
		}

		public virtual void RestoreState(ContextState state)
		{
			Seek(state._offset);
			_currentSlot = state._currentSlot;
		}

		public virtual object ReadFieldValue(ClassMetadata classMetadata, FieldMetadata field
			)
		{
			if (!SeekToField(classMetadata, field))
			{
				return null;
			}
			return field.Read(this);
		}

		public virtual bool SeekToField(ClassMetadata classMetadata, FieldMetadata field)
		{
			return _objectHeader.ObjectMarshaller().FindOffset(classMetadata, _objectHeader._headerAttributes
				, ((ByteArrayBuffer)Buffer()), field);
		}
	}
}
