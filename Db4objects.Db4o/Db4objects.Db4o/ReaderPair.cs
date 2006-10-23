namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class ReaderPair : Db4objects.Db4o.ISlotReader
	{
		private Db4objects.Db4o.YapReader _source;

		private Db4objects.Db4o.YapReader _target;

		private Db4objects.Db4o.IDefragContext _mapping;

		private Db4objects.Db4o.Transaction _systemTrans;

		public ReaderPair(Db4objects.Db4o.YapReader source, Db4objects.Db4o.IDefragContext
			 mapping, Db4objects.Db4o.Transaction systemTrans)
		{
			_source = source;
			_mapping = mapping;
			_target = new Db4objects.Db4o.YapReader(source.GetLength());
			_source.CopyTo(_target, 0, 0, _source.GetLength());
			_systemTrans = systemTrans;
		}

		public virtual int Offset()
		{
			return _source.Offset();
		}

		public virtual void Offset(int offset)
		{
			_source.Offset(offset);
			_target.Offset(offset);
		}

		public virtual void IncrementOffset(int numBytes)
		{
			_source.IncrementOffset(numBytes);
			_target.IncrementOffset(numBytes);
		}

		public virtual void IncrementIntSize()
		{
			IncrementIntSize(1);
		}

		public virtual void IncrementIntSize(int times)
		{
			IncrementOffset(times * Db4objects.Db4o.YapConst.INT_LENGTH);
		}

		public virtual int CopyID(bool flipNegative)
		{
			int id = _source.ReadInt();
			if (flipNegative && id < 0)
			{
				id = -id;
			}
			int mapped = _mapping.MappedID(id);
			if (flipNegative && id < 0)
			{
				mapped = -mapped;
			}
			_target.WriteInt(mapped);
			return mapped;
		}

		public virtual int CopyID()
		{
			return CopyID(false);
		}

		public virtual void ReadBegin(byte identifier)
		{
			_source.ReadBegin(identifier);
			_target.ReadBegin(identifier);
		}

		public virtual byte ReadByte()
		{
			byte value = _source.ReadByte();
			_target.IncrementOffset(1);
			return value;
		}

		public virtual int ReadInt()
		{
			int value = _source.ReadInt();
			_target.IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
			return value;
		}

		public virtual void WriteInt(int value)
		{
			_source.IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
			_target.WriteInt(value);
		}

		public virtual void Write(Db4objects.Db4o.YapFile file, int address)
		{
			file.WriteBytes(_target, address, 0);
		}

		public virtual string ReadShortString(Db4objects.Db4o.YapStringIO sio)
		{
			string value = Db4objects.Db4o.Inside.Marshall.StringMarshaller.ReadShort(sio, false
				, _source);
			Db4objects.Db4o.Inside.Marshall.StringMarshaller.ReadShort(sio, false, _target);
			return value;
		}

		public virtual Db4objects.Db4o.YapReader Source()
		{
			return _source;
		}

		public virtual Db4objects.Db4o.YapReader Target()
		{
			return _target;
		}

		public virtual Db4objects.Db4o.IIDMapping Mapping()
		{
			return _mapping;
		}

		public virtual Db4objects.Db4o.Transaction SystemTrans()
		{
			return _systemTrans;
		}

		public virtual Db4objects.Db4o.IDefragContext Context()
		{
			return _mapping;
		}

		public static void ProcessCopy(Db4objects.Db4o.IDefragContext context, int sourceID
			, Db4objects.Db4o.ISlotCopyHandler command)
		{
			Db4objects.Db4o.YapReader sourceReader = context.SourceReaderByID(sourceID);
			int targetID = context.MappedID(sourceID);
			int targetLength = sourceReader.GetLength();
			int targetAddress = context.AllocateTargetSlot(targetLength);
			Db4objects.Db4o.YapReader targetPointerReader = new Db4objects.Db4o.YapReader(Db4objects.Db4o.YapConst
				.POINTER_LENGTH);
			targetPointerReader.WriteInt(targetAddress);
			targetPointerReader.WriteInt(targetLength);
			context.TargetWriteBytes(targetPointerReader, targetID);
			Db4objects.Db4o.ReaderPair readers = new Db4objects.Db4o.ReaderPair(sourceReader, 
				context, context.SystemTrans());
			command.ProcessCopy(readers);
			context.TargetWriteBytes(readers, targetAddress);
		}

		public virtual void Append(byte value)
		{
			_source.IncrementOffset(1);
			_target.Append(value);
		}

		public virtual long ReadLong()
		{
			long value = _source.ReadLong();
			_target.IncrementOffset(Db4objects.Db4o.YapConst.LONG_LENGTH);
			return value;
		}

		public virtual void WriteLong(long value)
		{
			_source.IncrementOffset(Db4objects.Db4o.YapConst.LONG_LENGTH);
			_target.WriteLong(value);
		}

		public virtual Db4objects.Db4o.Foundation.BitMap4 ReadBitMap(int bitCount)
		{
			Db4objects.Db4o.Foundation.BitMap4 value = _source.ReadBitMap(bitCount);
			_target.IncrementOffset(value.MarshalledLength());
			return value;
		}

		public virtual void CopyBytes(byte[] target, int sourceOffset, int targetOffset, 
			int length)
		{
			_source.CopyBytes(target, sourceOffset, targetOffset, length);
		}

		public virtual void ReadEnd()
		{
			_source.ReadEnd();
			_target.ReadEnd();
		}
	}
}