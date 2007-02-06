namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public abstract class FileHeader
	{
		private static readonly Db4objects.Db4o.Internal.Fileheader.FileHeader[] AVAILABLE_FILE_HEADERS
			 = new Db4objects.Db4o.Internal.Fileheader.FileHeader[] { new Db4objects.Db4o.Internal.Fileheader.FileHeader0
			(), new Db4objects.Db4o.Internal.Fileheader.FileHeader1() };

		private static int ReaderLength()
		{
			int length = AVAILABLE_FILE_HEADERS[0].Length();
			for (int i = 1; i < AVAILABLE_FILE_HEADERS.Length; i++)
			{
				length = System.Math.Max(length, AVAILABLE_FILE_HEADERS[i].Length());
			}
			return length;
		}

		public static Db4objects.Db4o.Internal.Fileheader.FileHeader ReadFixedPart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareFileHeaderReader(file);
			Db4objects.Db4o.Internal.Fileheader.FileHeader header = DetectFileHeader(file, reader
				);
			if (header == null)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages
					.INCOMPATIBLE_FORMAT);
			}
			else
			{
				header.ReadFixedPart(file, reader);
			}
			return header;
		}

		private static Db4objects.Db4o.Internal.Buffer PrepareFileHeaderReader(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(ReaderLength
				());
			reader.Read(file, 0, 0);
			return reader;
		}

		private static Db4objects.Db4o.Internal.Fileheader.FileHeader DetectFileHeader(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, Db4objects.Db4o.Internal.Buffer reader)
		{
			for (int i = 0; i < AVAILABLE_FILE_HEADERS.Length; i++)
			{
				reader.Seek(0);
				Db4objects.Db4o.Internal.Fileheader.FileHeader result = AVAILABLE_FILE_HEADERS[i]
					.NewOnSignatureMatch(file, reader);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		public abstract void Close();

		public abstract void InitNew(Db4objects.Db4o.Internal.LocalObjectContainer file);

		public abstract Db4objects.Db4o.Internal.Transaction InterruptedTransaction();

		public abstract int Length();

		protected abstract Db4objects.Db4o.Internal.Fileheader.FileHeader NewOnSignatureMatch
			(Db4objects.Db4o.Internal.LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader);

		protected virtual long TimeToWrite(long time, bool shuttingDown)
		{
			return shuttingDown ? 0 : time;
		}

		protected abstract void ReadFixedPart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, Db4objects.Db4o.Internal.Buffer reader);

		public abstract void ReadVariablePart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file);

		protected virtual bool SignatureMatches(Db4objects.Db4o.Internal.Buffer reader, byte[]
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

		public abstract void WriteFixedPart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, bool shuttingDown, Db4objects.Db4o.Internal.StatefulBuffer writer, int blockSize
			, int freespaceID);

		public abstract void WriteTransactionPointer(Db4objects.Db4o.Internal.Transaction
			 systemTransaction, int transactionAddress);

		protected virtual void WriteTransactionPointer(Db4objects.Db4o.Internal.Transaction
			 systemTransaction, int transactionAddress, int address, int offset)
		{
			Db4objects.Db4o.Internal.StatefulBuffer bytes = new Db4objects.Db4o.Internal.StatefulBuffer
				(systemTransaction, address, Db4objects.Db4o.Internal.Const4.INT_LENGTH * 2);
			bytes.MoveForward(offset);
			bytes.WriteInt(transactionAddress);
			bytes.WriteInt(transactionAddress);
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				bytes.SetID(Db4objects.Db4o.Internal.Const4.IGNORE_ID);
			}
			bytes.Write();
		}

		public abstract void WriteVariablePart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, int part);

		protected virtual void ReadClassCollectionAndFreeSpace(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, Db4objects.Db4o.Internal.Buffer reader)
		{
			Db4objects.Db4o.Internal.SystemData systemData = file.SystemData();
			systemData.ClassCollectionID(reader.ReadInt());
			systemData.FreespaceID(reader.ReadInt());
		}
	}
}
