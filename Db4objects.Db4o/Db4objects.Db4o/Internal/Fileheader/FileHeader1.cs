/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;
using Sharpen;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader1 : FileHeader
	{
		private static readonly byte[] Signature = new byte[] { (byte)'d', (byte)'b', (byte
			)'4', (byte)'o' };

		private static byte Version = 1;

		private static readonly int HeaderLockOffset = Signature.Length + 1;

		private static readonly int OpenTimeOffset = HeaderLockOffset + Const4.IntLength;

		private static readonly int AccessTimeOffset = OpenTimeOffset + Const4.LongLength;

		private static readonly int TransactionPointerOffset = AccessTimeOffset + Const4.
			LongLength;

		private static readonly int BlocksizeOffset = TransactionPointerOffset + (Const4.
			IntLength * 2);

		public static readonly int HeaderLength = TransactionPointerOffset + (Const4.IntLength
			 * 6);

		private TimerFileLock _timerFileLock;

		private FileHeaderVariablePart1 _variablePart;

		private int _transactionId1;

		private int _transactionId2;

		// The header format is:
		// (byte) 'd'
		// (byte) 'b'
		// (byte) '4'
		// (byte) 'o'
		// (byte) headerVersion
		// (int) headerLock
		// (long) openTime
		// (long) accessTime
		// (int) Transaction pointer 1
		// (int) Transaction pointer 2
		// (int) blockSize
		// (int) classCollectionID
		// (int) freespaceID
		// (int) variablePartID
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Close()
		{
			if (_timerFileLock == null)
			{
				return;
			}
			_timerFileLock.Close();
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void InitNew(LocalObjectContainer file)
		{
			CommonTasksForNewAndRead(file);
			_variablePart = new FileHeaderVariablePart1(file, 0, file.SystemData());
			WriteVariablePart(file, 0);
		}

		protected override FileHeader NewOnSignatureMatch(LocalObjectContainer file, ByteArrayBuffer
			 reader)
		{
			if (SignatureMatches(reader, Signature, Version))
			{
				return new FileHeader1();
			}
			return null;
		}

		private void NewTimerFileLock(LocalObjectContainer file)
		{
			_timerFileLock = TimerFileLock.ForFile(file);
			_timerFileLock.SetAddresses(0, OpenTimeOffset, AccessTimeOffset);
		}

		public override void CompleteInterruptedTransaction(LocalObjectContainer container
			)
		{
			container.GlobalIdSystem().CompleteInterruptedTransaction(_transactionId1, _transactionId2
				);
		}

		public override int Length()
		{
			return HeaderLength;
		}

		protected override void Read(LocalObjectContainer file, ByteArrayBuffer reader)
		{
			CommonTasksForNewAndRead(file);
			CheckThreadFileLock(file, reader);
			reader.Seek(TransactionPointerOffset);
			_transactionId1 = reader.ReadInt();
			_transactionId2 = reader.ReadInt();
			reader.Seek(BlocksizeOffset);
			file.BlockSizeReadFromFile(reader.ReadInt());
			ReadClassCollectionAndFreeSpace(file, reader);
			_variablePart = new FileHeaderVariablePart1(file, reader.ReadInt(), file.SystemData
				());
			_variablePart.Read(file.SystemTransaction());
		}

		private void CheckThreadFileLock(LocalObjectContainer container, ByteArrayBuffer 
			reader)
		{
			reader.Seek(AccessTimeOffset);
			long lastAccessTime = reader.ReadLong();
			if (FileHeader.LockedByOtherSession(container, lastAccessTime))
			{
				_timerFileLock.CheckIfOtherSessionAlive(container, 0, AccessTimeOffset, lastAccessTime
					);
			}
		}

		private void CommonTasksForNewAndRead(LocalObjectContainer file)
		{
			NewTimerFileLock(file);
			file._handlers.OldEncryptionOff();
		}

		public override void WriteFixedPart(LocalObjectContainer file, bool startFileLockingThread
			, bool shuttingDown, StatefulBuffer writer, int blockSize, int freespaceID)
		{
			writer.Append(Signature);
			writer.WriteByte(Version);
			writer.WriteInt((int)TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(Runtime.CurrentTimeMillis(), shuttingDown));
			writer.WriteInt(0);
			// transaction pointer 1 for "in-commit-mode"
			writer.WriteInt(0);
			// transaction pointer 2
			writer.WriteInt(blockSize);
			writer.WriteInt(file.SystemData().ClassCollectionID());
			writer.WriteInt(freespaceID);
			writer.WriteInt(_variablePart.GetID());
			writer.Write();
			file.SyncFiles();
			if (startFileLockingThread)
			{
				file.ThreadPool().Start(_timerFileLock);
			}
		}

		public override void WriteTransactionPointer(Transaction systemTransaction, int transactionAddress
			)
		{
			WriteTransactionPointer(systemTransaction, transactionAddress, 0, TransactionPointerOffset
				);
		}

		public override void WriteVariablePart(LocalObjectContainer file, int part)
		{
			_variablePart.SetStateDirty();
			_variablePart.Write(file.SystemTransaction());
		}

		public override void ReadIdentity(LocalObjectContainer container)
		{
			_variablePart.ReadIdentity((LocalTransaction)container.SystemTransaction());
		}
	}
}
