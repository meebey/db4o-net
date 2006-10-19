namespace Db4objects.Db4o.Header
{
	/// <exclude></exclude>
	public abstract class FileHeader
	{
		private static readonly Db4objects.Db4o.Header.FileHeader[] AVAILABLE_FILE_HEADERS
			 = new Db4objects.Db4o.Header.FileHeader[] { new Db4objects.Db4o.Header.FileHeader0
			(), new Db4objects.Db4o.Header.FileHeader1() };

		private static int ReaderLength()
		{
			int length = AVAILABLE_FILE_HEADERS[0].Length();
			for (int i = 1; i < AVAILABLE_FILE_HEADERS.Length; i++)
			{
				length = Db4objects.Db4o.YInt.Max(length, AVAILABLE_FILE_HEADERS[i].Length());
			}
			return length;
		}

		public static Db4objects.Db4o.Header.FileHeader ReadFixedPart(Db4objects.Db4o.YapFile
			 file)
		{
			Db4objects.Db4o.YapReader reader = PrepareFileHeaderReader(file);
			Db4objects.Db4o.Header.FileHeader header = DetectFileHeader(file, reader);
			if (header == null)
			{
				Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(17);
			}
			else
			{
				header.ReadFixedPart(file, reader);
			}
			return header;
		}

		private static Db4objects.Db4o.YapReader PrepareFileHeaderReader(Db4objects.Db4o.YapFile
			 file)
		{
			Db4objects.Db4o.YapReader reader = new Db4objects.Db4o.YapReader(ReaderLength());
			reader.Read(file, 0, 0);
			return reader;
		}

		private static Db4objects.Db4o.Header.FileHeader DetectFileHeader(Db4objects.Db4o.YapFile
			 file, Db4objects.Db4o.YapReader reader)
		{
			for (int i = 0; i < AVAILABLE_FILE_HEADERS.Length; i++)
			{
				reader.Seek(0);
				Db4objects.Db4o.Header.FileHeader result = AVAILABLE_FILE_HEADERS[i].NewOnSignatureMatch
					(file, reader);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		public abstract void Close();

		public abstract void InitNew(Db4objects.Db4o.YapFile file);

		public abstract Db4objects.Db4o.Transaction InterruptedTransaction();

		public abstract int Length();

		protected abstract Db4objects.Db4o.Header.FileHeader NewOnSignatureMatch(Db4objects.Db4o.YapFile
			 file, Db4objects.Db4o.YapReader reader);

		protected virtual long TimeToWrite(long time, bool shuttingDown)
		{
			return shuttingDown ? 0 : time;
		}

		protected abstract void ReadFixedPart(Db4objects.Db4o.YapFile file, Db4objects.Db4o.YapReader
			 reader);

		public abstract void ReadVariablePart(Db4objects.Db4o.YapFile file);

		protected virtual bool SignatureMatches(Db4objects.Db4o.YapReader reader, byte[] 
			signature, byte version)
		{
			for (int i = 0; i < signature.Length; i++)
			{
				if (reader.ReadByte() != signature[i])
				{
					return false;
				}
			}
			return reader.ReadByte() == version;
		}

		public abstract void WriteFixedPart(Db4objects.Db4o.YapFile file, bool shuttingDown
			, Db4objects.Db4o.YapWriter writer, int blockSize, int freespaceID);

		public abstract void WriteTransactionPointer(Db4objects.Db4o.Transaction systemTransaction
			, int transactionAddress);

		protected virtual void WriteTransactionPointer(Db4objects.Db4o.Transaction systemTransaction
			, int transactionAddress, int address, int offset)
		{
			Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(systemTransaction
				, address, Db4objects.Db4o.YapConst.INT_LENGTH * 2);
			bytes.MoveForward(offset);
			bytes.WriteInt(transactionAddress);
			bytes.WriteInt(transactionAddress);
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				bytes.SetID(Db4objects.Db4o.YapConst.IGNORE_ID);
			}
			bytes.Write();
		}

		public abstract void WriteVariablePart(Db4objects.Db4o.YapFile file, int part);

		protected virtual void ReadClassCollectionAndFreeSpace(Db4objects.Db4o.YapFile file
			, Db4objects.Db4o.YapReader reader)
		{
			Db4objects.Db4o.Inside.SystemData systemData = file.SystemData();
			systemData.ClassCollectionID(reader.ReadInt());
			systemData.FreespaceID(reader.ReadInt());
		}
	}
}
