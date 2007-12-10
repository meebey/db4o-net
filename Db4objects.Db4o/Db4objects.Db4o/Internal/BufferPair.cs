/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class BufferPair : ISlotBuffer
	{
		private Db4objects.Db4o.Internal.Buffer _source;

		private Db4objects.Db4o.Internal.Buffer _target;

		private IDefragmentServices _mapping;

		private Transaction _systemTrans;

		public BufferPair(Db4objects.Db4o.Internal.Buffer source, IDefragmentServices mapping
			, Transaction systemTrans)
		{
			_source = source;
			_mapping = mapping;
			_target = new Db4objects.Db4o.Internal.Buffer(source.Length());
			_source.CopyTo(_target, 0, 0, _source.Length());
			_systemTrans = systemTrans;
		}

		public int Offset()
		{
			return _source.Offset();
		}

		public void Offset(int offset)
		{
			_source.Offset(offset);
			_target.Offset(offset);
		}

		public void IncrementOffset(int numBytes)
		{
			_source.IncrementOffset(numBytes);
			_target.IncrementOffset(numBytes);
		}

		public void IncrementIntSize()
		{
			IncrementOffset(Const4.INT_LENGTH);
		}

		public int CopyUnindexedID()
		{
			int orig = _source.ReadInt();
			int mapped = -1;
			try
			{
				mapped = _mapping.MappedID(orig);
			}
			catch (MappingNotFoundException)
			{
				mapped = _mapping.AllocateTargetSlot(Const4.POINTER_LENGTH).Address();
				_mapping.MapIDs(orig, mapped, false);
				_mapping.RegisterUnindexed(orig);
			}
			_target.WriteInt(mapped);
			return mapped;
		}

		public int CopyID()
		{
			int id = _source.ReadInt();
			return WriteMappedID(id);
		}

		public int CopyID(bool flipNegative, bool lenient)
		{
			int id = _source.ReadInt();
			return InternalCopyID(flipNegative, lenient, id);
		}

		public MappedIDPair CopyIDAndRetrieveMapping()
		{
			int id = _source.ReadInt();
			return new MappedIDPair(id, InternalCopyID(false, false, id));
		}

		private int InternalCopyID(bool flipNegative, bool lenient, int id)
		{
			if (flipNegative && id < 0)
			{
				id = -id;
			}
			int mapped = _mapping.MappedID(id, lenient);
			if (flipNegative && id < 0)
			{
				mapped = -mapped;
			}
			_target.WriteInt(mapped);
			return mapped;
		}

		public void ReadBegin(byte identifier)
		{
			_source.ReadBegin(identifier);
			_target.ReadBegin(identifier);
		}

		public byte ReadByte()
		{
			byte value = _source.ReadByte();
			_target.IncrementOffset(1);
			return value;
		}

		public int ReadInt()
		{
			int value = _source.ReadInt();
			_target.IncrementOffset(Const4.INT_LENGTH);
			return value;
		}

		public void WriteInt(int value)
		{
			_source.IncrementOffset(Const4.INT_LENGTH);
			_target.WriteInt(value);
		}

		public void Write(LocalObjectContainer file, int address)
		{
			file.WriteBytes(_target, address, 0);
		}

		public void IncrementStringOffset(LatinStringIO sio)
		{
			IncrementStringOffset(sio, _source);
			IncrementStringOffset(sio, _target);
		}

		private void IncrementStringOffset(LatinStringIO sio, Db4objects.Db4o.Internal.Buffer
			 buffer)
		{
			int length = buffer.ReadInt();
			if (length > 0)
			{
				sio.Read(buffer, length);
			}
		}

		public Db4objects.Db4o.Internal.Buffer Source()
		{
			return _source;
		}

		public Db4objects.Db4o.Internal.Buffer Target()
		{
			return _target;
		}

		public IIDMapping Mapping()
		{
			return _mapping;
		}

		public Transaction SystemTrans()
		{
			return _systemTrans;
		}

		public IDefragmentServices Context()
		{
			return _mapping;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public static void ProcessCopy(IDefragmentServices context, int sourceID, ISlotCopyHandler
			 command)
		{
			ProcessCopy(context, sourceID, command, false);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public static void ProcessCopy(IDefragmentServices context, int sourceID, ISlotCopyHandler
			 command, bool registerAddressMapping)
		{
			Db4objects.Db4o.Internal.Buffer sourceReader = context.SourceBufferByID(sourceID);
			ProcessCopy(context, sourceID, command, registerAddressMapping, sourceReader);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public static void ProcessCopy(IDefragmentServices context, int sourceID, ISlotCopyHandler
			 command, bool registerAddressMapping, Db4objects.Db4o.Internal.Buffer sourceReader
			)
		{
			int targetID = context.MappedID(sourceID);
			Slot targetSlot = context.AllocateTargetSlot(sourceReader.Length());
			if (registerAddressMapping)
			{
				int sourceAddress = context.SourceAddressByID(sourceID);
				context.MapIDs(sourceAddress, targetSlot.Address(), false);
			}
			Db4objects.Db4o.Internal.Buffer targetPointerReader = new Db4objects.Db4o.Internal.Buffer
				(Const4.POINTER_LENGTH);
			targetPointerReader.WriteInt(targetSlot.Address());
			targetPointerReader.WriteInt(targetSlot.Length());
			context.TargetWriteBytes(targetPointerReader, targetID);
			Db4objects.Db4o.Internal.BufferPair readers = new Db4objects.Db4o.Internal.BufferPair
				(sourceReader, context, context.SystemTrans());
			command.ProcessCopy(readers);
			context.TargetWriteBytes(readers, targetSlot.Address());
		}

		public void WriteByte(byte value)
		{
			_source.IncrementOffset(1);
			_target.WriteByte(value);
		}

		public long ReadLong()
		{
			long value = _source.ReadLong();
			_target.IncrementOffset(Const4.LONG_LENGTH);
			return value;
		}

		public void WriteLong(long value)
		{
			_source.IncrementOffset(Const4.LONG_LENGTH);
			_target.WriteLong(value);
		}

		public BitMap4 ReadBitMap(int bitCount)
		{
			BitMap4 value = _source.ReadBitMap(bitCount);
			_target.IncrementOffset(value.MarshalledLength());
			return value;
		}

		public void CopyBytes(byte[] target, int sourceOffset, int targetOffset, int length
			)
		{
			_source.CopyBytes(target, sourceOffset, targetOffset, length);
		}

		public void ReadEnd()
		{
			_source.ReadEnd();
			_target.ReadEnd();
		}

		public int PreparePayloadRead()
		{
			int newPayLoadOffset = ReadInt();
			ReadInt();
			int linkOffSet = Offset();
			Offset(newPayLoadOffset);
			return linkOffSet;
		}

		public int WriteMappedID(int originalID)
		{
			int mapped = _mapping.MappedID(originalID, false);
			_target.WriteInt(mapped);
			return mapped;
		}
	}
}
