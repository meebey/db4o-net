/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Slots;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// public for .NET conversion reasons
	/// TODO: Split this class for individual usecases.
	/// </summary>
	/// <remarks>
	/// public for .NET conversion reasons
	/// TODO: Split this class for individual usecases. Only use the member
	/// variables needed for the respective usecase.
	/// </remarks>
	/// <exclude></exclude>
	public sealed class StatefulBuffer : Db4objects.Db4o.Internal.Buffer
	{
		private int i_address;

		private int _addressOffset;

		private int i_cascadeDelete;

		private int i_id;

		private IActivationDepth i_instantionDepth;

		private int i_length;

		internal Transaction i_trans;

		private int i_updateDepth = 1;

		public int _payloadOffset;

		public StatefulBuffer(Transaction a_trans, int a_initialBufferSize)
		{
			i_trans = a_trans;
			i_length = a_initialBufferSize;
			_buffer = new byte[i_length];
		}

		public StatefulBuffer(Transaction a_trans, int address, int length) : this(a_trans
			, length)
		{
			i_address = address;
		}

		public StatefulBuffer(Transaction trans, Db4objects.Db4o.Internal.Slots.Slot slot
			) : this(trans, slot.Address(), slot.Length())
		{
		}

		public StatefulBuffer(Transaction trans, Pointer4 pointer) : this(trans, pointer.
			_slot)
		{
			i_id = pointer._id;
		}

		public void DebugCheckBytes()
		{
		}

		public int GetAddress()
		{
			return i_address;
		}

		public int AddressOffset()
		{
			return _addressOffset;
		}

		public int GetID()
		{
			return i_id;
		}

		public IActivationDepth GetInstantiationDepth()
		{
			return i_instantionDepth;
		}

		public override int Length()
		{
			return i_length;
		}

		public ObjectContainerBase GetStream()
		{
			return i_trans.Container();
		}

		public ObjectContainerBase Stream()
		{
			return i_trans.Container();
		}

		public LocalObjectContainer File()
		{
			return ((LocalTransaction)i_trans).File();
		}

		public Transaction GetTransaction()
		{
			return i_trans;
		}

		public int GetUpdateDepth()
		{
			return i_updateDepth;
		}

		public byte[] GetWrittenBytes()
		{
			byte[] bytes = new byte[_offset];
			System.Array.Copy(_buffer, 0, bytes, 0, _offset);
			return bytes;
		}

		public int PreparePayloadRead()
		{
			int newPayLoadOffset = ReadInt();
			int length = ReadInt();
			int linkOffSet = _offset;
			_offset = newPayLoadOffset;
			_payloadOffset += length;
			return linkOffSet;
		}

		/// <exception cref="Db4oIOException"></exception>
		public void Read()
		{
			Stream().ReadBytes(_buffer, i_address, _addressOffset, i_length);
		}

		/// <exception cref="Db4oIOException"></exception>
		public Db4objects.Db4o.Internal.StatefulBuffer ReadEmbeddedObject()
		{
			int id = ReadInt();
			int length = ReadInt();
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Internal.StatefulBuffer bytes = null;
			bytes = Stream().ReadWriterByAddress(i_trans, id, length);
			if (bytes != null)
			{
				bytes.SetID(id);
			}
			if (bytes != null)
			{
				bytes.SetUpdateDepth(GetUpdateDepth());
				bytes.SetInstantiationDepth(GetInstantiationDepth());
			}
			return bytes;
		}

		public Db4objects.Db4o.Internal.StatefulBuffer ReadYapBytes()
		{
			int length = ReadInt();
			if (length == 0)
			{
				return null;
			}
			Db4objects.Db4o.Internal.StatefulBuffer yb = new Db4objects.Db4o.Internal.StatefulBuffer
				(i_trans, length);
			System.Array.Copy(_buffer, _offset, yb._buffer, 0, length);
			_offset += length;
			return yb;
		}

		public void RemoveFirstBytes(int aLength)
		{
			i_length -= aLength;
			byte[] temp = new byte[i_length];
			System.Array.Copy(_buffer, aLength, temp, 0, i_length);
			_buffer = temp;
			_offset -= aLength;
			if (_offset < 0)
			{
				_offset = 0;
			}
		}

		public void Address(int a_address)
		{
			i_address = a_address;
		}

		public void SetID(int a_id)
		{
			i_id = a_id;
		}

		public void SetInstantiationDepth(IActivationDepth a_depth)
		{
			i_instantionDepth = a_depth;
		}

		public void SetTransaction(Transaction aTrans)
		{
			i_trans = aTrans;
		}

		public void SetUpdateDepth(int a_depth)
		{
			i_updateDepth = a_depth;
		}

		public void SlotDelete()
		{
			i_trans.SlotDelete(i_id, Slot());
		}

		public void Trim4(int a_offset, int a_length)
		{
			byte[] temp = new byte[a_length];
			System.Array.Copy(_buffer, a_offset, temp, 0, a_length);
			_buffer = temp;
			i_length = a_length;
		}

		public void UseSlot(int a_adress)
		{
			i_address = a_adress;
			_offset = 0;
		}

		public void UseSlot(int address, int length)
		{
			UseSlot(new Db4objects.Db4o.Internal.Slots.Slot(address, length));
		}

		public void UseSlot(Db4objects.Db4o.Internal.Slots.Slot slot)
		{
			i_address = slot.Address();
			_offset = 0;
			if (slot.Length() > _buffer.Length)
			{
				_buffer = new byte[slot.Length()];
			}
			i_length = slot.Length();
		}

		public void UseSlot(int a_id, int a_adress, int a_length)
		{
			i_id = a_id;
			UseSlot(a_adress, a_length);
		}

		public void Write()
		{
			File().WriteBytes(this, i_address, _addressOffset);
		}

		public void WriteEmbeddedNull()
		{
			WriteInt(0);
			WriteInt(0);
		}

		public void WriteEncrypt()
		{
			File().WriteEncrypt(this, i_address, _addressOffset);
		}

		public void WritePayload(Db4objects.Db4o.Internal.StatefulBuffer payLoad, bool topLevel
			)
		{
			CheckMinimumPayLoadOffsetAndWritePointerAndLength(payLoad.Length(), topLevel);
			System.Array.Copy(payLoad._buffer, 0, _buffer, _payloadOffset, payLoad._buffer.Length
				);
			TransferPayLoadAddress(payLoad, _payloadOffset);
			_payloadOffset += payLoad._buffer.Length;
		}

		private void CheckMinimumPayLoadOffsetAndWritePointerAndLength(int length, bool alignToBlockSize
			)
		{
			if (_payloadOffset <= _offset + (Const4.INT_LENGTH * 2))
			{
				_payloadOffset = _offset + (Const4.INT_LENGTH * 2);
			}
			if (alignToBlockSize)
			{
				_payloadOffset = Stream().BlockAlignedBytes(_payloadOffset);
			}
			WriteInt(_payloadOffset);
			WriteInt(length);
		}

		public int ReserveAndPointToPayLoadSlot(int length)
		{
			CheckMinimumPayLoadOffsetAndWritePointerAndLength(length, false);
			int linkOffset = _offset;
			_offset = _payloadOffset;
			_payloadOffset += length;
			return linkOffset;
		}

		public Db4objects.Db4o.Internal.Buffer ReadPayloadWriter(int offset, int length)
		{
			Db4objects.Db4o.Internal.StatefulBuffer payLoad = new Db4objects.Db4o.Internal.StatefulBuffer
				(i_trans, 0, length);
			System.Array.Copy(_buffer, offset, payLoad._buffer, 0, length);
			TransferPayLoadAddress(payLoad, offset);
			return payLoad;
		}

		private void TransferPayLoadAddress(Db4objects.Db4o.Internal.StatefulBuffer toWriter
			, int offset)
		{
			int blockedOffset = offset / Stream().BlockSize();
			toWriter.i_address = i_address + blockedOffset;
			toWriter.i_id = toWriter.i_address;
			toWriter._addressOffset = _addressOffset;
		}

		internal void WriteShortString(string a_string)
		{
			WriteShortString(i_trans, a_string);
		}

		public void MoveForward(int length)
		{
			_addressOffset += length;
		}

		public void WriteForward()
		{
			Write();
			_addressOffset += i_length;
			_offset = 0;
		}

		public override string ToString()
		{
			return base.ToString();
			return "id " + i_id + " adr " + i_address + " len " + i_length;
		}

		public void NoXByteCheck()
		{
			if (Debug.xbytes && Deploy.overwrite)
			{
				SetID(Const4.IGNORE_ID);
			}
		}

		public void WriteIDs(IIntIterator4 idIterator, int maxCount)
		{
			int savedOffset = _offset;
			WriteInt(0);
			int actualCount = 0;
			while (idIterator.MoveNext())
			{
				WriteInt(idIterator.CurrentInt());
				actualCount++;
				if (actualCount >= maxCount)
				{
					break;
				}
			}
			int secondSavedOffset = _offset;
			_offset = savedOffset;
			WriteInt(actualCount);
			_offset = secondSavedOffset;
		}

		public Db4objects.Db4o.Internal.Slots.Slot Slot()
		{
			return new Db4objects.Db4o.Internal.Slots.Slot(i_address, i_length);
		}

		public Pointer4 Pointer()
		{
			return new Pointer4(i_id, Slot());
		}

		public int CascadeDeletes()
		{
			return i_cascadeDelete;
		}

		public void SetCascadeDeletes(int depth)
		{
			i_cascadeDelete = depth;
		}
	}
}
