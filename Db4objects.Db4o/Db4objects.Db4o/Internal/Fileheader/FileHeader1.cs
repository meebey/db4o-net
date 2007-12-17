/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;
using Sharpen;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader1 : FileHeader
	{
		private static readonly byte[] SIGNATURE = new byte[] { (byte)'d', (byte)'b', (byte
			)'4', (byte)'o' };

		private static byte VERSION = 1;

		private static readonly int HEADER_LOCK_OFFSET = SIGNATURE.Length + 1;

		private static readonly int OPEN_TIME_OFFSET = HEADER_LOCK_OFFSET + Const4.INT_LENGTH;

		private static readonly int ACCESS_TIME_OFFSET = OPEN_TIME_OFFSET + Const4.LONG_LENGTH;

		private static readonly int TRANSACTION_POINTER_OFFSET = ACCESS_TIME_OFFSET + Const4
			.LONG_LENGTH;

		public static readonly int LENGTH = TRANSACTION_POINTER_OFFSET + (Const4.INT_LENGTH
			 * 6);

		private TimerFileLock _timerFileLock;

		private Transaction _interruptedTransaction;

		private FileHeaderVariablePart1 _variablePart;

		/// <exception cref="Db4oIOException"></exception>
		public override void Close()
		{
			_timerFileLock.Close();
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void InitNew(LocalObjectContainer file)
		{
			CommonTasksForNewAndRead(file);
			_variablePart = new FileHeaderVariablePart1(0, file.SystemData());
			WriteVariablePart(file, 0);
		}

		protected override FileHeader NewOnSignatureMatch(LocalObjectContainer file, BufferImpl
			 reader)
		{
			if (SignatureMatches(reader, SIGNATURE, VERSION))
			{
				return new FileHeader1();
			}
			return null;
		}

		private void NewTimerFileLock(LocalObjectContainer file)
		{
			_timerFileLock = TimerFileLock.ForFile(file);
			_timerFileLock.SetAddresses(0, OPEN_TIME_OFFSET, ACCESS_TIME_OFFSET);
		}

		public override Transaction InterruptedTransaction()
		{
			return _interruptedTransaction;
		}

		public override int Length()
		{
			return LENGTH;
		}

		protected override void ReadFixedPart(LocalObjectContainer file, BufferImpl reader
			)
		{
			CommonTasksForNewAndRead(file);
			CheckThreadFileLock(file, reader);
			reader.Seek(TRANSACTION_POINTER_OFFSET);
			_interruptedTransaction = LocalTransaction.ReadInterruptedTransaction(file, reader
				);
			file.BlockSizeReadFromFile(reader.ReadInt());
			ReadClassCollectionAndFreeSpace(file, reader);
			_variablePart = new FileHeaderVariablePart1(reader.ReadInt(), file.SystemData());
		}

		private void CheckThreadFileLock(LocalObjectContainer container, BufferImpl reader
			)
		{
			reader.Seek(ACCESS_TIME_OFFSET);
			long lastAccessTime = reader.ReadLong();
			if (FileHeader.LockedByOtherSession(container, lastAccessTime))
			{
				_timerFileLock.CheckIfOtherSessionAlive(container, 0, ACCESS_TIME_OFFSET, lastAccessTime
					);
			}
		}

		private void CommonTasksForNewAndRead(LocalObjectContainer file)
		{
			NewTimerFileLock(file);
			file._handlers.OldEncryptionOff();
		}

		public override void ReadVariablePart(LocalObjectContainer file)
		{
			_variablePart.Read(file.SystemTransaction());
		}

		public override void WriteFixedPart(LocalObjectContainer file, bool startFileLockingThread
			, bool shuttingDown, StatefulBuffer writer, int blockSize, int freespaceID)
		{
			writer.Append(SIGNATURE);
			writer.WriteByte(VERSION);
			writer.WriteInt((int)TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(Runtime.CurrentTimeMillis(), shuttingDown));
			writer.WriteInt(0);
			writer.WriteInt(0);
			writer.WriteInt(blockSize);
			writer.WriteInt(file.SystemData().ClassCollectionID());
			writer.WriteInt(freespaceID);
			writer.WriteInt(_variablePart.GetID());
			writer.NoXByteCheck();
			writer.Write();
			file.SyncFiles();
			if (startFileLockingThread)
			{
				_timerFileLock.Start();
			}
		}

		public override void WriteTransactionPointer(Transaction systemTransaction, int transactionAddress
			)
		{
			WriteTransactionPointer(systemTransaction, transactionAddress, 0, TRANSACTION_POINTER_OFFSET
				);
		}

		public override void WriteVariablePart(LocalObjectContainer file, int part)
		{
			_variablePart.SetStateDirty();
			_variablePart.Write(file.SystemTransaction());
		}
	}
}
