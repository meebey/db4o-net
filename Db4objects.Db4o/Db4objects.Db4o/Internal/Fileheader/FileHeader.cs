/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
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

		/// <exception cref="OldFormatException"></exception>
		public static FileHeader ReadFixedPart(LocalObjectContainer file)
		{
			BufferImpl reader = PrepareFileHeaderReader(file);
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

		private static BufferImpl PrepareFileHeaderReader(LocalObjectContainer file)
		{
			BufferImpl reader = new BufferImpl(ReaderLength());
			reader.Read(file, 0, 0);
			return reader;
		}

		private static FileHeader DetectFileHeader(LocalObjectContainer file, BufferImpl 
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

		/// <exception cref="Db4oIOException"></exception>
		public abstract void Close();

		/// <exception cref="Db4oIOException"></exception>
		public abstract void InitNew(LocalObjectContainer file);

		public abstract Transaction InterruptedTransaction();

		public abstract int Length();

		protected abstract FileHeader NewOnSignatureMatch(LocalObjectContainer file, BufferImpl
			 reader);

		protected virtual long TimeToWrite(long time, bool shuttingDown)
		{
			return shuttingDown ? 0 : time;
		}

		protected abstract void ReadFixedPart(LocalObjectContainer file, BufferImpl reader
			);

		public abstract void ReadVariablePart(LocalObjectContainer file);

		protected virtual bool SignatureMatches(BufferImpl reader, byte[] signature, byte
			 version)
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
			BufferImpl reader)
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
