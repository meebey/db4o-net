/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class MarshallingContext : IFieldListInfo, IWriteContext
	{
		private const int HEADER_LENGTH = Const4.LEADING_LENGTH + Const4.ID_LENGTH + 1 + 
			Const4.INT_LENGTH;

		private const byte MARSHALLER_FAMILY_VERSION = (byte)3;

		private readonly Db4objects.Db4o.Internal.Transaction _transaction;

		private readonly ObjectReference _reference;

		private int _updateDepth;

		private readonly bool _isNew;

		private readonly BitMap4 _nullBitMap;

		private readonly MarshallingBuffer _writeBuffer;

		private MarshallingBuffer _currentBuffer;

		private int _fieldWriteCount;

		private Db4objects.Db4o.Internal.Buffer _debugPrepend;

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

		private StatefulBuffer CreateNewBuffer()
		{
			Slot slot = new Slot(-1, MarshalledLength());
			if (_transaction is LocalTransaction)
			{
				slot = ((LocalTransaction)_transaction).File().GetSlot(MarshalledLength());
				_transaction.SlotFreeOnRollback(ObjectID(), slot);
			}
			_transaction.SetPointer(ObjectID(), slot);
			return CreateUpdateBuffer(slot.Address());
		}

		private StatefulBuffer CreateUpdateBuffer(int address)
		{
			int length = _transaction.Container().BlockAlignedBytes(MarshalledLength());
			StatefulBuffer buffer = new StatefulBuffer(_transaction, length);
			buffer.UseSlot(ObjectID(), address, length);
			buffer.SetUpdateDepth(_updateDepth);
			if ((address == 0) && (Transaction() is LocalTransaction))
			{
				((LocalTransaction)Transaction()).File().GetSlotForUpdate(buffer);
			}
			return buffer;
		}

		public virtual StatefulBuffer ToWriteBuffer()
		{
			_writeBuffer.MergeChildren(WriteBufferOffset());
			StatefulBuffer buffer = IsNew() ? CreateNewBuffer() : CreateUpdateBuffer(0);
			WriteObjectClassID(buffer, ClassMetadata().GetID());
			buffer.WriteByte(MARSHALLER_FAMILY_VERSION);
			buffer.WriteInt(FieldCount());
			buffer.WriteBitMap(_nullBitMap);
			_writeBuffer.TransferContentTo(buffer);
			return buffer;
		}

		private int WriteBufferOffset()
		{
			return HEADER_LENGTH + _nullBitMap.MarshalledLength();
		}

		private int MarshalledLength()
		{
			return HEADER_LENGTH + NullBitMapLength() + RequiredLength(_writeBuffer);
		}

		private int NullBitMapLength()
		{
			return Const4.INT_LENGTH + _nullBitMap.MarshalledLength();
		}

		private int RequiredLength(MarshallingBuffer buffer)
		{
			return Container().BlockAlignedBytes(buffer.Length());
		}

		private void WriteObjectClassID(Db4objects.Db4o.Internal.Buffer reader, int id)
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
				CreateChildBuffer();
			}
		}

		private void PostWrite()
		{
		}

		private void CreateChildBuffer()
		{
			MarshallingBuffer childBuffer = _currentBuffer.AddChild(false);
			_currentBuffer.TransferLastWriteTo(childBuffer);
			_currentBuffer.ReserveChildLinkSpace();
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

		public virtual void DebugPrependNextWrite(Db4objects.Db4o.Internal.Buffer prepend
			)
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
		}

		public virtual void WriteObject(ITypeHandler4 handler, object obj)
		{
			MarshallingBuffer tempBuffer = _currentBuffer;
			int tempFieldWriteCount = _fieldWriteCount;
			_fieldWriteCount = 0;
			handler.Write(this, obj);
			_fieldWriteCount = tempFieldWriteCount;
			_currentBuffer = tempBuffer;
		}

		public virtual void WriteAny(object obj)
		{
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = Db4objects.Db4o.Internal.ClassMetadata
				.ForObject(Transaction(), obj, false);
			MarshallingBuffer tempBuffer = _currentBuffer;
			int tempFieldWriteCount = _fieldWriteCount;
			_fieldWriteCount = 0;
			WriteInt(classMetadata.GetID());
			classMetadata.Write(this, obj);
			_fieldWriteCount = tempFieldWriteCount;
			_currentBuffer = tempBuffer;
		}

		public virtual void AddIndexEntry(FieldMetadata fieldMetadata, object obj)
		{
			if (!_currentBuffer.HasParent())
			{
				fieldMetadata.AddIndexEntry(Transaction(), ObjectID(), obj);
				return;
			}
			_currentBuffer.AddIndexEntry(fieldMetadata);
		}
	}
}
