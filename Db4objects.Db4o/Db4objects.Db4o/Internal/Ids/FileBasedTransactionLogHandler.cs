/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class FileBasedTransactionLogHandler : TransactionLogHandler
	{
		internal const int LockInt = int.MaxValue - 1;

		private IBin _lockFile;

		private IBin _logFile;

		private readonly string _fileName;

		public FileBasedTransactionLogHandler(IdSystem idSystem, string fileName)
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

		private IBin OpenBin(IdSystem idSystem, string fileName)
		{
			return new FileStorage().Open(new BinConfiguration(fileName, idSystem.Config().LockFile
				(), 0, false));
		}

		public override bool CheckForInterruptedTransaction(IdSystem idSystem, ByteArrayBuffer
			 reader)
		{
			reader.IncrementOffset(Const4.IntLength * 2);
			if (!System.IO.File.Exists(LockFileName(_fileName)))
			{
				return false;
			}
			return LockFileSignalsInterruptedTransaction(idSystem);
		}

		private bool LockFileSignalsInterruptedTransaction(IdSystem idSystem)
		{
			OpenLockFile(idSystem);
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
			SyncAndClose(_lockFile);
		}

		private void SyncAndClose(IBin bin)
		{
			try
			{
				bin.Sync();
			}
			finally
			{
				bin.Close();
			}
		}

		private void CloseLogFile()
		{
			SyncAndClose(_logFile);
		}

		private void DeleteLockFile()
		{
			File4.Delete(LockFileName(_fileName));
		}

		private void DeleteLogFile()
		{
			File4.Delete(LogFileName(_fileName));
		}

		public override Slot AllocateSlot(IdSystem idSystem, bool append)
		{
			// do nothing
			return null;
		}

		public override void ApplySlotChanges(IdSystem idSystem, Slot reservedSlot)
		{
			int slotChangeCount = CountSlotChanges(idSystem);
			if (slotChangeCount < 1)
			{
				return;
			}
			FlushDatabaseFile(idSystem);
			EnsureLogAndLock(idSystem);
			int length = TransactionLogSlotLength(idSystem);
			ByteArrayBuffer logBuffer = new ByteArrayBuffer(length);
			logBuffer.WriteInt(length);
			logBuffer.WriteInt(slotChangeCount);
			AppendSlotChanges(idSystem, logBuffer);
			Write(_logFile, logBuffer);
			_logFile.Sync();
			WriteToLockFile(LockInt);
			if (idSystem.WriteSlots())
			{
				FlushDatabaseFile(idSystem);
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

		private void EnsureLogAndLock(IdSystem idSystem)
		{
			if (idSystem.Config().IsReadOnly())
			{
				return;
			}
			if (LogsOpened())
			{
				return;
			}
			OpenLockFile(idSystem);
			OpenLogFile(idSystem);
		}

		private void OpenLogFile(IdSystem idSystem)
		{
			_logFile = OpenBin(idSystem, LogFileName(_fileName));
		}

		private void OpenLockFile(IdSystem idSystem)
		{
			_lockFile = OpenBin(idSystem, LockFileName(_fileName));
		}

		private bool LogsOpened()
		{
			return _lockFile != null;
		}

		public override void CompleteInterruptedTransaction(IdSystem idSystem)
		{
			ByteArrayBuffer buffer = new ByteArrayBuffer(Const4.IntLength);
			OpenLogFile(idSystem);
			Read(_logFile, buffer);
			int length = buffer.ReadInt();
			if (length > 0)
			{
				buffer = new ByteArrayBuffer(length);
				Read(_logFile, buffer);
				buffer.IncrementOffset(Const4.IntLength);
				idSystem.ReadSlotChanges(buffer);
				if (idSystem.WriteSlots())
				{
					FlushDatabaseFile(idSystem);
				}
				DeleteLockFile();
				idSystem.FreeSlotChanges(false);
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
