/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
		private static readonly FileHeader[] AvailableFileHeaders = new FileHeader[] { new 
			FileHeader0(), new FileHeader1() };

		private static int ReaderLength()
		{
			int length = AvailableFileHeaders[0].Length();
			for (int i = 1; i < AvailableFileHeaders.Length; i++)
			{
				length = Math.Max(length, AvailableFileHeaders[i].Length());
			}
			return length;
		}

		/// <exception cref="OldFormatException"></exception>
		public static FileHeader ReadFixedPart(LocalObjectContainer file)
		{
			ByteArrayBuffer reader = PrepareFileHeaderReader(file);
			FileHeader header = DetectFileHeader(file, reader);
			if (header == null)
			{
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.IncompatibleFormat
					);
			}
			else
			{
				header.ReadFixedPart(file, reader);
			}
			return header;
		}

		private static ByteArrayBuffer PrepareFileHeaderReader(LocalObjectContainer file)
		{
			ByteArrayBuffer reader = new ByteArrayBuffer(ReaderLength());
			reader.Read(file, 0, 0);
			return reader;
		}

		private static FileHeader DetectFileHeader(LocalObjectContainer file, ByteArrayBuffer
			 reader)
		{
			for (int i = 0; i < AvailableFileHeaders.Length; i++)
			{
				reader.Seek(0);
				FileHeader result = AvailableFileHeaders[i].NewOnSignatureMatch(file, reader);
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

		protected abstract FileHeader NewOnSignatureMatch(LocalObjectContainer file, ByteArrayBuffer
			 reader);

		protected virtual long TimeToWrite(long time, bool shuttingDown)
		{
			return shuttingDown ? 0 : time;
		}

		protected abstract void ReadFixedPart(LocalObjectContainer file, ByteArrayBuffer 
			reader);

		public abstract void ReadVariablePart(LocalObjectContainer file);

		protected virtual bool SignatureMatches(ByteArrayBuffer reader, byte[] signature, 
			byte version)
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

		// TODO: freespaceID should not be passed here, it should be taken from SystemData
		public abstract void WriteFixedPart(LocalObjectContainer file, bool startFileLockingThread
			, bool shuttingDown, StatefulBuffer writer, int blockSize, int freespaceID);

		public abstract void WriteTransactionPointer(Transaction systemTransaction, int transactionAddress
			);

		protected virtual void WriteTransactionPointer(Transaction systemTransaction, int
			 transactionAddress, int address, int offset)
		{
			StatefulBuffer bytes = new StatefulBuffer(systemTransaction, address, Const4.IntLength
				 * 2);
			bytes.MoveForward(offset);
			bytes.WriteInt(transactionAddress);
			bytes.WriteInt(transactionAddress);
			if (Debug.xbytes && Deploy.overwrite)
			{
				bytes.SetID(Const4.IgnoreId);
			}
			bytes.Write();
		}

		public abstract void WriteVariablePart(LocalObjectContainer file, int part);

		protected virtual void ReadClassCollectionAndFreeSpace(LocalObjectContainer file, 
			ByteArrayBuffer reader)
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
