/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Encoding;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class DefragmentContextImpl : IReadWriteBuffer, IDefragmentContext
	{
		private ByteArrayBuffer _source;

		private ByteArrayBuffer _target;

		private IDefragmentServices _services;

		private readonly ObjectHeader _objectHeader;

		private int _aspectCount;

		public DefragmentContextImpl(ByteArrayBuffer source, Db4objects.Db4o.Internal.DefragmentContextImpl
			 context) : this(source, context._services, context._objectHeader)
		{
		}

		public DefragmentContextImpl(ByteArrayBuffer source, IDefragmentServices services
			) : this(source, services, null)
		{
		}

		public DefragmentContextImpl(ByteArrayBuffer source, IDefragmentServices services
			, ObjectHeader header)
		{
			_source = source;
			_services = services;
			_target = new ByteArrayBuffer(Length());
			_source.CopyTo(_target, 0, 0, Length());
			_objectHeader = header;
		}

		public DefragmentContextImpl(Db4objects.Db4o.Internal.DefragmentContextImpl parentContext
			, ObjectHeader header)
		{
			_source = parentContext._source;
			_target = parentContext._target;
			_services = parentContext._services;
			_objectHeader = header;
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
			// TODO: There is no test case for the zero case
			if (orig == 0)
			{
				_target.WriteInt(0);
				return 0;
			}
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
			bool flipped = flipNegative && (id < 0);
			if (flipped)
			{
				id = -id;
			}
			int mapped = _services.MappedID(id, lenient);
			if (flipped)
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
			sio.ReadLengthAndString(buffer);
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

		public static void ProcessCopy(IDefragmentServices services, int sourceID, ISlotCopyHandler
			 command)
		{
			ProcessCopy(services, sourceID, command, false);
		}

		public static void ProcessCopy(IDefragmentServices context, int sourceID, ISlotCopyHandler
			 command, bool registerAddressMapping)
		{
			ByteArrayBuffer sourceReader = context.SourceBufferByID(sourceID);
			ProcessCopy(context, sourceID, command, registerAddressMapping, sourceReader);
		}

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

		public int HandlerVersion()
		{
			return _objectHeader.HandlerVersion();
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
			return Container();
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

		/// <exception cref="System.IO.IOException"></exception>
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

		/// <exception cref="System.IO.IOException"></exception>
		public ByteArrayBuffer SourceBufferByAddress(int sourceAddress, int length)
		{
			ByteArrayBuffer sourceBuffer = _services.SourceBufferByAddress(sourceAddress, length
				);
			return sourceBuffer;
		}

		/// <exception cref="System.IO.IOException"></exception>
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

		public void SeekCurrentInt()
		{
			Seek(ReadInt());
		}

		public IReadBuffer Buffer()
		{
			return _source;
		}

		public void Defragment(ITypeHandler4 handler)
		{
			ITypeHandler4 typeHandler = HandlerRegistry.CorrectHandlerVersion(this, handler);
			if (Handlers4.UseDedicatedSlot(this, typeHandler))
			{
				if (Handlers4.HasClassIndex(typeHandler))
				{
					CopyID();
				}
				else
				{
					CopyUnindexedID();
				}
				return;
			}
			typeHandler.Defragment(this);
		}

		public void BeginSlot()
		{
		}

		// do nothing
		public Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _objectHeader.ClassMetadata();
		}

		public bool IsNull(int fieldIndex)
		{
			return _objectHeader._headerAttributes.IsNull(fieldIndex);
		}

		public int AspectCount()
		{
			return _aspectCount;
		}

		public void AspectCount(int count)
		{
			_aspectCount = count;
		}

		public Db4objects.Db4o.Internal.Marshall.SlotFormat SlotFormat()
		{
			return Db4objects.Db4o.Internal.Marshall.SlotFormat.ForHandlerVersion(HandlerVersion
				());
		}
	}
}
