/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;
using Sharpen;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader1 : FileHeader
	{
		private static readonly byte[] Signature = new byte[] { (byte)'d', (byte)'b', (byte
			)'4', (byte)'o' };

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
			_variablePart = CreateVariablePart(file, 0);
			WriteVariablePart(file, 0);
		}

		protected virtual FileHeaderVariablePart1 CreateVariablePart(LocalObjectContainer
			 file, int id)
		{
			return new FileHeaderVariablePart1(file, id, file.SystemData());
		}

		protected override FileHeader NewOnSignatureMatch(LocalObjectContainer file, ByteArrayBuffer
			 reader)
		{
			if (SignatureMatches(reader, Signature, Version()))
			{
				return CreateNew();
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
			SystemData systemData = container.SystemData();
			container.GlobalIdSystem().CompleteInterruptedTransaction(systemData.TransactionPointer1
				(), systemData.TransactionPointer2());
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
			file.SystemData().TransactionPointer1(reader.ReadInt());
			file.SystemData().TransactionPointer2(reader.ReadInt());
			reader.Seek(BlocksizeOffset);
			file.BlockSizeReadFromFile(reader.ReadInt());
			ReadClassCollectionAndFreeSpace(file, reader);
			_variablePart = CreateVariablePart(file, reader.ReadInt());
			_variablePart.Read();
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
			SystemData systemData = file.SystemData();
			writer.Append(Signature);
			writer.WriteByte(Version());
			writer.WriteInt((int)TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(Runtime.CurrentTimeMillis(), shuttingDown));
			writer.WriteInt(systemData.TransactionPointer1());
			writer.WriteInt(systemData.TransactionPointer2());
			writer.WriteInt(blockSize);
			writer.WriteInt(systemData.ClassCollectionID());
			writer.WriteInt(freespaceID);
			writer.WriteInt(_variablePart.Id());
			writer.Write();
			file.SyncFiles();
			if (startFileLockingThread)
			{
				file.ThreadPool().Start(_timerFileLock);
			}
		}

		public override void WriteTransactionPointer(Transaction systemTransaction, int transactionPointer1
			, int transactionPointer2)
		{
			WriteTransactionPointer(systemTransaction, transactionPointer1, transactionPointer2
				, 0, TransactionPointerOffset);
		}

		public override void WriteVariablePart(LocalObjectContainer file, int part)
		{
			IRunnable commitHook = Commit();
			file.SyncFiles();
			commitHook.Run();
			file.SyncFiles();
		}

		public override void ReadIdentity(LocalObjectContainer container)
		{
			_variablePart.ReadIdentity((LocalTransaction)container.SystemTransaction());
		}

		public override IRunnable Commit()
		{
			return _variablePart.Commit();
		}

		protected virtual FileHeader1 CreateNew()
		{
			return new FileHeader1();
		}

		protected virtual byte Version()
		{
			return (byte)1;
		}

		public override FileHeader Convert(LocalObjectContainer file)
		{
			FileHeader2 fileHeader = new FileHeader2();
			fileHeader.InitNew(file);
			return fileHeader;
		}
	}
}
