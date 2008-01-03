/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class MarshallingContext : IFieldListInfo, IMarshallingInfo, IWriteContext
	{
		private const int HeaderLength = Const4.LeadingLength + Const4.IdLength + 1 + Const4
			.IntLength;

		public const byte HandlerVersion = (byte)2;

		private const int NoIndirection = 3;

		private readonly Db4objects.Db4o.Internal.Transaction _transaction;

		private readonly ObjectReference _reference;

		private int _updateDepth;

		private readonly bool _isNew;

		private readonly BitMap4 _nullBitMap;

		private readonly MarshallingBuffer _writeBuffer;

		private MarshallingBuffer _currentBuffer;

		private int _fieldWriteCount;

		private BufferImpl _debugPrepend;

		private object _currentMarshalledObject;

		private object _currentIndexEntry;

		public MarshallingContext(Db4objects.Db4o.Internal.Transaction trans, ObjectReference
			 @ref, int updateDepth, bool isNew)
		{
			_transaction = trans;
			_reference = @ref;
			_nullBitMap = new BitMap4(FieldCount());
			_updateDepth = ClassMetadata().AdjustUpdateDepth(trans, updateDepth);
			_isNew = isNew;
			_writeBuffer = new MarshallingBuffer();
			_currentBuffer = _writeBuffer;
		}

		private int FieldCount()
		{
			return ClassMetadata().FieldCount();
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _reference.ClassMetadata();
		}

		public virtual bool IsNew()
		{
			return _isNew;
		}

		public virtual bool IsNull(int fieldIndex)
		{
			return false;
		}

		public virtual void IsNull(int fieldIndex, bool flag)
		{
			_nullBitMap.Set(fieldIndex, flag);
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		private Slot CreateNewSlot(int length)
		{
			Slot slot = new Slot(-1, length);
			if (_transaction is LocalTransaction)
			{
				slot = ((LocalTransaction)_transaction).File().GetSlot(length);
				_transaction.SlotFreeOnRollback(ObjectID(), slot);
			}
			_transaction.SetPointer(ObjectID(), slot);
			return slot;
		}

		private Slot CreateUpdateSlot(int length)
		{
			if (Transaction() is LocalTransaction)
			{
				return ((LocalTransaction)Transaction()).File().GetSlotForUpdate(Transaction(), ObjectID
					(), length);
			}
			return new Slot(0, length);
		}

		public virtual Pointer4 AllocateSlot()
		{
			int length = Container().BlockAlignedBytes(MarshalledLength());
			Slot slot = IsNew() ? CreateNewSlot(length) : CreateUpdateSlot(length);
			return new Pointer4(ObjectID(), slot);
		}

		public virtual BufferImpl ToWriteBuffer(Pointer4 pointer)
		{
			BufferImpl buffer = new BufferImpl(pointer.Length());
			_writeBuffer.MergeChildren(this, pointer.Address(), WriteBufferOffset());
			WriteObjectClassID(buffer, ClassMetadata().GetID());
			buffer.WriteByte(HandlerVersion);
			buffer.WriteInt(FieldCount());
			buffer.WriteBitMap(_nullBitMap);
			_writeBuffer.TransferContentTo(buffer);
			return buffer;
		}

		private int WriteBufferOffset()
		{
			return HeaderLength + _nullBitMap.MarshalledLength();
		}

		private int MarshalledLength()
		{
			int length = WriteBufferOffset();
			_writeBuffer.CheckBlockAlignment(this, null, new IntByRef(length));
			return length + _writeBuffer.MarshalledLength() + Const4.BracketsBytes;
		}

		public virtual int RequiredLength(MarshallingBuffer buffer, bool align)
		{
			if (!align)
			{
				return buffer.Length();
			}
			return Container().BlockAlignedBytes(buffer.Length());
		}

		private void WriteObjectClassID(BufferImpl reader, int id)
		{
			reader.WriteInt(-id);
		}

		public virtual object GetObject()
		{
			return _reference.GetObject();
		}

		public virtual Config4Class ClassConfiguration()
		{
			return ClassMetadata().Config();
		}

		public virtual int UpdateDepth()
		{
			return _updateDepth;
		}

		public virtual void UpdateDepth(int depth)
		{
			_updateDepth = depth;
		}

		public virtual int ObjectID()
		{
			return _reference.GetID();
		}

		public virtual object CurrentIndexEntry()
		{
			return null;
		}

		public virtual ObjectContainerBase Container()
		{
			return Transaction().Container();
		}

		public virtual IObjectContainer ObjectContainer()
		{
			return Transaction().ObjectContainer();
		}

		public virtual void WriteByte(byte b)
		{
			PreWrite();
			_currentBuffer.WriteByte(b);
			PostWrite();
		}

		public virtual void WriteBytes(byte[] bytes)
		{
			PreWrite();
			_currentBuffer.WriteBytes(bytes);
			PostWrite();
		}

		public virtual void WriteInt(int i)
		{
			PreWrite();
			_currentBuffer.WriteInt(i);
			PostWrite();
		}

		public virtual void WriteLong(long l)
		{
			PreWrite();
			_currentBuffer.WriteLong(l);
			PostWrite();
		}

		private void PreWrite()
		{
			_fieldWriteCount++;
			if (IsSecondWriteToField())
			{
				CreateChildBuffer(true, true);
			}
		}

		private void PostWrite()
		{
		}

		public virtual void CreateChildBuffer(bool transferLastWrite, bool storeLengthInLink
			)
		{
			MarshallingBuffer childBuffer = _currentBuffer.AddChild(false, storeLengthInLink);
			if (transferLastWrite)
			{
				_currentBuffer.TransferLastWriteTo(childBuffer, storeLengthInLink);
			}
			_currentBuffer.ReserveChildLinkSpace(storeLengthInLink);
			_currentBuffer = childBuffer;
		}

		private bool IsSecondWriteToField()
		{
			return _fieldWriteCount == 2;
		}

		public virtual void NextField()
		{
			_fieldWriteCount = 0;
			_currentBuffer = _writeBuffer;
		}

		public virtual void FieldCount(int fieldCount)
		{
			_writeBuffer.WriteInt(fieldCount);
		}

		public virtual void DebugPrependNextWrite(BufferImpl prepend)
		{
		}

		public virtual void DebugWriteEnd(byte b)
		{
			_currentBuffer.WriteByte(b);
		}

		public virtual void WriteObject(object obj)
		{
			int id = Container().SetInternal(Transaction(), obj, _updateDepth, true);
			WriteInt(id);
			_currentMarshalledObject = obj;
			_currentIndexEntry = id;
		}

		public virtual void WriteObject(ITypeHandler4 handler, object obj)
		{
			MarshallingContextState state = CurrentState();
			if (obj == null)
			{
				WriteNullObject(handler);
			}
			else
			{
				CreateIndirection(handler);
				handler.Write(this, obj);
			}
			RestoreState(state);
		}

		private void WriteNullObject(ITypeHandler4 handler)
		{
			if (HandlerRegistry().IsVariableLength(handler))
			{
				DoNotIndirectWrites();
				WriteNullLink();
				return;
			}
			if (handler is PrimitiveHandler)
			{
				PrimitiveHandler primitiveHandler = (PrimitiveHandler)handler;
				handler.Write(this, primitiveHandler.NullRepresentationInUntypedArrays());
				return;
			}
			handler.Write(this, null);
		}

		private void WriteNullLink()
		{
			WriteInt(0);
			WriteInt(0);
		}

		public virtual void AddIndexEntry(FieldMetadata fieldMetadata, object obj)
		{
			if (!_currentBuffer.HasParent())
			{
				object indexEntry = (obj == _currentMarshalledObject) ? _currentIndexEntry : obj;
				fieldMetadata.AddIndexEntry(Transaction(), ObjectID(), indexEntry);
				return;
			}
			_currentBuffer.RequestIndexEntry(fieldMetadata);
		}

		public virtual ObjectReference Reference()
		{
			return _reference;
		}

		public virtual void DoNotIndirectWrites()
		{
			_fieldWriteCount = NoIndirection;
		}

		public virtual void PrepareIndirectionOfSecondWrite()
		{
			_fieldWriteCount = 0;
		}

		private Db4objects.Db4o.Internal.HandlerRegistry HandlerRegistry()
		{
			return Container().Handlers();
		}

		public virtual void CreateIndirection(ITypeHandler4 handler)
		{
			if (HandlerRegistry().IsVariableLength(handler))
			{
				CreateChildBuffer(false, true);
				DoNotIndirectWrites();
			}
		}

		public virtual IBuffer Buffer()
		{
			return null;
		}

		public virtual MarshallingContextState CurrentState()
		{
			return new MarshallingContextState(_currentBuffer, _fieldWriteCount);
		}

		public virtual void RestoreState(MarshallingContextState state)
		{
			_currentBuffer = state._buffer;
			_fieldWriteCount = state._fieldWriteCount;
		}
	}
}
