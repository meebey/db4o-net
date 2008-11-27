/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal.Transactionlog
{
	/// <exclude></exclude>
	public class FileBasedTransactionLogHandler : TransactionLogHandler
	{
		internal const int LockInt = int.MaxValue - 1;

		private IBin _lockFile;

		private IBin _logFile;

		private readonly string _fileName;

		public FileBasedTransactionLogHandler(LocalTransaction trans, string fileName)
		{
			_fileName = fileName;
		}

		public static string LogFileName(string fileName)
		{
			return fileName + ".log";
		}

		public static string LockFileName(string fileName)
		{
			return fileName + ".lock";
		}

		private IBin OpenBin(LocalTransaction trans, string fileName)
		{
			return new FileStorage().Open(fileName, trans.Config().LockFile(), 0, false);
		}

		public override bool CheckForInterruptedTransaction(LocalTransaction trans, ByteArrayBuffer
			 reader)
		{
			reader.IncrementOffset(Const4.IntLength * 2);
			if (!System.IO.File.Exists(LockFileName(_fileName)))
			{
				return false;
			}
			return LockFileSignalsInterruptedTransaction(trans);
		}

		private bool LockFileSignalsInterruptedTransaction(LocalTransaction trans)
		{
			OpenLockFile(trans);
			ByteArrayBuffer buffer = NewLockFileBuffer();
			Read(_lockFile, buffer);
			for (int i = 0; i < 2; i++)
			{
				int checkInt = buffer.ReadInt();
				if (checkInt != LockInt)
				{
					CloseLockFile();
					return false;
				}
			}
			CloseLockFile();
			return true;
		}

		public override void Close()
		{
			if (!LogsOpened())
			{
				return;
			}
			CloseLockFile();
			CloseLogFile();
			DeleteLockFile();
			DeleteLogFile();
		}

		private void CloseLockFile()
		{
			_lockFile.Close();
		}

		private void CloseLogFile()
		{
			_logFile.Close();
		}

		private void DeleteLockFile()
		{
			File4.Delete(LockFileName(_fileName));
		}

		private void DeleteLogFile()
		{
			File4.Delete(LogFileName(_fileName));
		}

		public override Slot AllocateSlot(LocalTransaction trans, bool append)
		{
			// do nothing
			return null;
		}

		public override void ApplySlotChanges(LocalTransaction trans, Slot reservedSlot)
		{
			int slotChangeCount = CountSlotChanges(trans);
			if (slotChangeCount < 1)
			{
				return;
			}
			FlushDatabaseFile(trans);
			EnsureLogAndLock(trans);
			int length = TransactionLogSlotLength(trans);
			ByteArrayBuffer logBuffer = new ByteArrayBuffer(length);
			logBuffer.WriteInt(length);
			logBuffer.WriteInt(slotChangeCount);
			AppendSlotChanges(trans, logBuffer);
			Write(_logFile, logBuffer);
			_logFile.Sync();
			WriteToLockFile(LockInt);
			if (trans.WriteSlots())
			{
				FlushDatabaseFile(trans);
			}
			WriteToLockFile(0);
		}

		private void WriteToLockFile(int lockSignal)
		{
			ByteArrayBuffer lockBuffer = NewLockFileBuffer();
			lockBuffer.WriteInt(lockSignal);
			lockBuffer.WriteInt(lockSignal);
			Write(_lockFile, lockBuffer);
			_lockFile.Sync();
		}

		private ByteArrayBuffer NewLockFileBuffer()
		{
			return new ByteArrayBuffer(LockFileBufferLength());
		}

		private int LockFileBufferLength()
		{
			return Const4.LongLength * 2;
		}

		private void EnsureLogAndLock(LocalTransaction trans)
		{
			if (trans.Config().IsReadOnly())
			{
				return;
			}
			if (LogsOpened())
			{
				return;
			}
			OpenLockFile(trans);
			OpenLogFile(trans);
		}

		private void OpenLogFile(LocalTransaction trans)
		{
			_logFile = OpenBin(trans, LogFileName(_fileName));
		}

		private void OpenLockFile(LocalTransaction trans)
		{
			_lockFile = OpenBin(trans, LockFileName(_fileName));
		}

		private bool LogsOpened()
		{
			return _lockFile != null;
		}

		public override void CompleteInterruptedTransaction(LocalTransaction trans)
		{
			ByteArrayBuffer buffer = new ByteArrayBuffer(Const4.IntLength);
			OpenLogFile(trans);
			Read(_logFile, buffer);
			int length = buffer.ReadInt();
			if (length > 0)
			{
				buffer = new ByteArrayBuffer(length);
				Read(_logFile, buffer);
				buffer.IncrementOffset(Const4.IntLength);
				trans.ReadSlotChanges(buffer);
				if (trans.WriteSlots())
				{
					FlushDatabaseFile(trans);
				}
				DeleteLockFile();
				trans.FreeSlotChanges(false);
			}
			else
			{
				DeleteLockFile();
			}
			CloseLogFile();
			DeleteLogFile();
		}

		private void Read(IBin storage, ByteArrayBuffer buffer)
		{
			storage.Read(0, buffer._buffer, buffer.Length());
		}

		private void Write(IBin storage, ByteArrayBuffer buffer)
		{
			storage.Write(0, buffer._buffer, buffer.Length());
		}
	}
}
