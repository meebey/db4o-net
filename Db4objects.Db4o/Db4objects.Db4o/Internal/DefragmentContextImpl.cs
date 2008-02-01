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
	public sealed class DefragmentContextImpl : IReadWriteBuffer, IDefragmentContext
	{
		private ByteArrayBuffer _source;

		private ByteArrayBuffer _target;

		private IDefragmentServices _services;

		private int _handlerVersion;

		public DefragmentContextImpl(ByteArrayBuffer source, Db4objects.Db4o.Internal.DefragmentContextImpl
			 context) : this(source, context._services)
		{
		}

		public DefragmentContextImpl(ByteArrayBuffer source, IDefragmentServices services
			)
		{
			_source = source;
			_services = services;
			_target = new ByteArrayBuffer(Length());
			_source.CopyTo(_target, 0, 0, Length());
		}

		public int Offset()
		{
			return _source.Offset();
		}

		public void Seek(int offset)
		{
			_source.Seek(offset);
			_target.Seek(offset);
		}

		public void IncrementOffset(int numBytes)
		{
			_source.IncrementOffset(numBytes);
			_target.IncrementOffset(numBytes);
		}

		public void IncrementIntSize()
		{
			IncrementOffset(Const4.IntLength);
		}

		public int CopySlotlessID()
		{
			return CopyUnindexedId(false);
		}

		public int CopyUnindexedID()
		{
			return CopyUnindexedId(true);
		}

		private int CopyUnindexedId(bool doRegister)
		{
			int orig = _source.ReadInt();
			int mapped = -1;
			try
			{
				mapped = _services.MappedID(orig);
			}
			catch (MappingNotFoundException)
			{
				mapped = _services.AllocateTargetSlot(Const4.PointerLength).Address();
				_services.MapIDs(orig, mapped, false);
				if (doRegister)
				{
					_services.RegisterUnindexed(orig);
				}
			}
			_target.WriteInt(mapped);
			return mapped;
		}

		public int CopyID()
		{
			// This code is slightly redundant. 
			// The profiler shows it's a hotspot.
			// The following would be non-redudant. 
			// return copy(false, false);
			int id = _source.ReadInt();
			return WriteMappedID(id);
		}

		public int CopyID(bool flipNegative, bool lenient)
		{
			int id = _source.ReadInt();
			return InternalCopyID(flipNegative, lenient, id);
		}

		public int CopyIDReturnOriginalID()
		{
			int id = _source.ReadInt();
			InternalCopyID(false, false, id);
			return id;
		}

		private int InternalCopyID(bool flipNegative, bool lenient, int id)
		{
			if (flipNegative && id < 0)
			{
				id = -id;
			}
			int mapped = _services.MappedID(id, lenient);
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

		public void ReadBytes(byte[] bytes)
		{
			_source.ReadBytes(bytes);
			_target.IncrementOffset(bytes.Length);
		}

		public int ReadInt()
		{
			int value = _source.ReadInt();
			_target.IncrementOffset(Const4.IntLength);
			return value;
		}

		public void WriteInt(int value)
		{
			_source.IncrementOffset(Const4.IntLength);
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

		private void IncrementStringOffset(LatinStringIO sio, ByteArrayBuffer buffer)
		{
			int length = buffer.ReadInt();
			if (length > 0)
			{
				sio.Read(buffer, length);
			}
		}

		public ByteArrayBuffer SourceBuffer()
		{
			return _source;
		}

		public ByteArrayBuffer TargetBuffer()
		{
			return _target;
		}

		public IIDMapping Mapping()
		{
			return _services;
		}

		public Db4objects.Db4o.Internal.Transaction SystemTrans()
		{
			return Transaction();
		}

		public IDefragmentServices Services()
		{
			return _services;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public static void ProcessCopy(IDefragmentServices services, int sourceID, ISlotCopyHandler
			 command)
		{
			ProcessCopy(services, sourceID, command, false);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public static void ProcessCopy(IDefragmentServices context, int sourceID, ISlotCopyHandler
			 command, bool registerAddressMapping)
		{
			ByteArrayBuffer sourceReader = context.SourceBufferByID(sourceID);
			ProcessCopy(context, sourceID, command, registerAddressMapping, sourceReader);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public static void ProcessCopy(IDefragmentServices services, int sourceID, ISlotCopyHandler
			 command, bool registerAddressMapping, ByteArrayBuffer sourceReader)
		{
			int targetID = services.MappedID(sourceID);
			Slot targetSlot = services.AllocateTargetSlot(sourceReader.Length());
			if (registerAddressMapping)
			{
				int sourceAddress = services.SourceAddressByID(sourceID);
				services.MapIDs(sourceAddress, targetSlot.Address(), false);
			}
			ByteArrayBuffer targetPointerReader = new ByteArrayBuffer(Const4.PointerLength);
			targetPointerReader.WriteInt(targetSlot.Address());
			targetPointerReader.WriteInt(targetSlot.Length());
			services.TargetWriteBytes(targetPointerReader, targetID);
			Db4objects.Db4o.Internal.DefragmentContextImpl context = new Db4objects.Db4o.Internal.DefragmentContextImpl
				(sourceReader, services);
			command.ProcessCopy(context);
			services.TargetWriteBytes(context, targetSlot.Address());
		}

		public void WriteByte(byte value)
		{
			_source.IncrementOffset(1);
			_target.WriteByte(value);
		}

		public long ReadLong()
		{
			long value = _source.ReadLong();
			_target.IncrementOffset(Const4.LongLength);
			return value;
		}

		public void WriteLong(long value)
		{
			_source.IncrementOffset(Const4.LongLength);
			_target.WriteLong(value);
		}

		public BitMap4 ReadBitMap(int bitCount)
		{
			BitMap4 value = _source.ReadBitMap(bitCount);
			_target.IncrementOffset(value.MarshalledLength());
			return value;
		}

		public void ReadEnd()
		{
			_source.ReadEnd();
			_target.ReadEnd();
		}

		public int WriteMappedID(int originalID)
		{
			int mapped = _services.MappedID(originalID, false);
			_target.WriteInt(mapped);
			return mapped;
		}

		public int Length()
		{
			return _source.Length();
		}

		public Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return Services().SystemTrans();
		}

		private ObjectContainerBase Container()
		{
			return Transaction().Container();
		}

		public ITypeHandler4 TypeHandlerForId(int id)
		{
			return Container().TypeHandlerForId(id);
		}

		private int HandlerVersion()
		{
			return _handlerVersion;
		}

		public bool IsLegacyHandlerVersion()
		{
			return HandlerVersion() == 0;
		}

		public int MappedID(int origID)
		{
			return Mapping().MappedID(origID);
		}

		public IObjectContainer ObjectContainer()
		{
			return (IObjectContainer)Container();
		}

		public void HandlerVersion(int version)
		{
			_handlerVersion = version;
		}

		public ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 handler)
		{
			return Container().Handlers().CorrectHandlerVersion(handler, HandlerVersion());
		}

		public Slot AllocateTargetSlot(int length)
		{
			return _services.AllocateTargetSlot(length);
		}

		public Slot AllocateMappedTargetSlot(int sourceAddress, int length)
		{
			Slot slot = AllocateTargetSlot(length);
			_services.MapIDs(sourceAddress, slot.Address(), false);
			return slot;
		}

		/// <exception cref="IOException"></exception>
		public int CopySlotToNewMapped(int sourceAddress, int length)
		{
			Slot slot = AllocateMappedTargetSlot(sourceAddress, length);
			ByteArrayBuffer sourceBuffer = SourceBufferByAddress(sourceAddress, length);
			TargetWriteBytes(slot.Address(), sourceBuffer);
			return slot.Address();
		}

		public void TargetWriteBytes(int address, ByteArrayBuffer buffer)
		{
			_services.TargetWriteBytes(buffer, address);
		}

		/// <exception cref="IOException"></exception>
		public ByteArrayBuffer SourceBufferByAddress(int sourceAddress, int length)
		{
			ByteArrayBuffer sourceBuffer = _services.SourceBufferByAddress(sourceAddress, length
				);
			return sourceBuffer;
		}

		/// <exception cref="IOException"></exception>
		public ByteArrayBuffer SourceBufferById(int sourceId)
		{
			ByteArrayBuffer sourceBuffer = _services.SourceBufferByID(sourceId);
			return sourceBuffer;
		}

		public void WriteToTarget(int address)
		{
			_services.TargetWriteBytes(this, address);
		}

		public void WriteBytes(byte[] bytes)
		{
			_target.WriteBytes(bytes);
			_source.IncrementOffset(bytes.Length);
		}
	}
}
