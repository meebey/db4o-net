/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
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
	public sealed class StatefulBuffer : ByteArrayBuffer
	{
		private int i_address;

		private int _addressOffset;

		private int i_cascadeDelete;

		private int i_id;

		private int i_length;

		internal Db4objects.Db4o.Internal.Transaction i_trans;

		public int _payloadOffset;

		public StatefulBuffer(Db4objects.Db4o.Internal.Transaction a_trans, int a_initialBufferSize
			)
		{
			i_trans = a_trans;
			i_length = a_initialBufferSize;
			_buffer = new byte[i_length];
		}

		public StatefulBuffer(Db4objects.Db4o.Internal.Transaction a_trans, int address, 
			int length) : this(a_trans, length)
		{
			i_address = address;
		}

		public StatefulBuffer(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Slots.Slot
			 slot) : this(trans, slot.Address(), slot.Length())
		{
		}

		public StatefulBuffer(Db4objects.Db4o.Internal.Transaction trans, Pointer4 pointer
			) : this(trans, pointer._slot)
		{
			i_id = pointer._id;
		}

		public void DebugCheckBytes()
		{
		}

		// Db4o.log("!!! YapBytes.debugCheckBytes not all bytes used");
		// This is normal for writing The FreeSlotArray, becauce one
		// slot is possibly reserved by it's own pointer.
		public int GetAddress()
		{
			return i_address;
		}

		public int GetID()
		{
			return i_id;
		}

		public override int Length()
		{
			return i_length;
		}

		public ObjectContainerBase Container()
		{
			return i_trans.Container();
		}

		public LocalObjectContainer File()
		{
			return ((LocalTransaction)i_trans).File();
		}

		public Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return i_trans;
		}

		public byte[] GetWrittenBytes()
		{
			byte[] bytes = new byte[_offset];
			System.Array.Copy(_buffer, 0, bytes, 0, _offset);
			return bytes;
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public void Read()
		{
			Container().ReadBytes(_buffer, i_address, _addressOffset, i_length);
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

		public void SetTransaction(Db4objects.Db4o.Internal.Transaction aTrans)
		{
			i_trans = aTrans;
		}

		public void SlotDelete()
		{
			i_trans.SlotDelete(i_id, Slot());
		}

		public void UseSlot(int a_adress)
		{
			i_address = a_adress;
			_offset = 0;
		}

		// FIXME: FB remove
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

		// FIXME: FB remove
		public void UseSlot(int a_id, int a_adress, int a_length)
		{
			i_id = a_id;
			UseSlot(a_adress, a_length);
		}

		public void Write()
		{
			File().WriteBytes(this, i_address, _addressOffset);
		}

		public void WriteEncrypt()
		{
			File().WriteEncrypt(this, i_address, _addressOffset);
		}

		public ByteArrayBuffer ReadPayloadWriter(int offset, int length)
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
			int blockedOffset = offset / Container().BlockSize();
			toWriter.i_address = i_address + blockedOffset;
			toWriter.i_id = toWriter.i_address;
			toWriter._addressOffset = _addressOffset;
		}

		public void MoveForward(int length)
		{
			_addressOffset += length;
		}

		public override string ToString()
		{
			return "id " + i_id + " adr " + i_address + " len " + i_length;
		}

		public void NoXByteCheck()
		{
			if (Debug4.xbytes && Deploy.overwrite)
			{
				SetID(Const4.IgnoreId);
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
