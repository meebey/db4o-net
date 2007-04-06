using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public abstract class FileHeader
	{
		private static readonly FileHeader[] AVAILABLE_FILE_HEADERS = new FileHeader[] { 
			new FileHeader0(), new FileHeader1() };

		private static int ReaderLength()
		{
			int length = AVAILABLE_FILE_HEADERS[0].Length();
			for (int i = 1; i < AVAILABLE_FILE_HEADERS.Length; i++)
			{
				length = Math.Max(length, AVAILABLE_FILE_HEADERS[i].Length());
			}
			return length;
		}

		public static FileHeader ReadFixedPart(LocalObjectContainer file)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareFileHeaderReader(file);
			FileHeader header = DetectFileHeader(file, reader);
			if (header == null)
			{
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.INCOMPATIBLE_FORMAT
					);
			}
			else
			{
				header.ReadFixedPart(file, reader);
			}
			return header;
		}

		private static Db4objects.Db4o.Internal.Buffer PrepareFileHeaderReader(LocalObjectContainer
			 file)
		{
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(ReaderLength
				());
			reader.Read(file, 0, 0);
			return reader;
		}

		private static FileHeader DetectFileHeader(LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			for (int i = 0; i < AVAILABLE_FILE_HEADERS.Length; i++)
			{
				reader.Seek(0);
				FileHeader result = AVAILABLE_FILE_HEADERS[i].NewOnSignatureMatch(file, reader);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		public abstract void Close();

		public abstract void InitNew(LocalObjectContainer file);

		public abstract Transaction InterruptedTransaction();

		public abstract int Length();

		protected abstract FileHeader NewOnSignatureMatch(LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader);

		protected virtual long TimeToWrite(long time, bool shuttingDown)
		{
			return shuttingDown ? 0 : time;
		}

		protected abstract void ReadFixedPart(LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader);

		public abstract void ReadVariablePart(LocalObjectContainer file);

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

		public abstract void WriteFixedPart(LocalObjectContainer file, bool startFileLockingThread
			, bool shuttingDown, StatefulBuffer writer, int blockSize, int freespaceID);

		public abstract void WriteTransactionPointer(Transaction systemTransaction, int transactionAddress
			);

		protected virtual void WriteTransactionPointer(Transaction systemTransaction, int
			 transactionAddress, int address, int offset)
		{
			StatefulBuffer bytes = new StatefulBuffer(systemTransaction, address, Const4.INT_LENGTH
				 * 2);
			bytes.MoveForward(offset);
			bytes.WriteInt(transactionAddress);
			bytes.WriteInt(transactionAddress);
			if (Debug.xbytes && Deploy.overwrite)
			{
				bytes.SetID(Const4.IGNORE_ID);
			}
			bytes.Write();
		}

		public abstract void WriteVariablePart(LocalObjectContainer file, int part);

		protected virtual void ReadClassCollectionAndFreeSpace(LocalObjectContainer file, 
			Db4objects.Db4o.Internal.Buffer reader)
		{
			SystemData systemData = file.SystemData();
			systemData.ClassCollectionID(reader.ReadInt());
			systemData.FreespaceID(reader.ReadInt());
		}

		public static bool LockedByOtherSession(LocalObjectContainer container, long lastAccessTime
			)
		{
			return container.NeedsLockFileThread() && (lastAccessTime != 0);
		}
	}
}
