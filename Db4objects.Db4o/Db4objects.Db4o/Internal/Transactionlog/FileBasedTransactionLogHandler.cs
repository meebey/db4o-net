/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;
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

		public FileBasedTransactionLogHandler(StandardIdSystem idSystem, string fileName)
			 : base(idSystem)
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

		private IBin OpenBin(string fileName)
		{
			return new FileStorage().Open(new BinConfiguration(fileName, _idSystem.Config().LockFile
				(), 0, false));
		}

		public override IInterruptedTransactionHandler InterruptedTransactionHandler(ByteArrayBuffer
			 reader)
		{
			reader.IncrementOffset(Const4.IntLength * 2);
			if (!System.IO.File.Exists(LockFileName(_fileName)))
			{
				return null;
			}
			if (!LockFileSignalsInterruptedTransaction())
			{
				return null;
			}
			return new _IInterruptedTransactionHandler_50(this);
		}

		private sealed class _IInterruptedTransactionHandler_50 : IInterruptedTransactionHandler
		{
			public _IInterruptedTransactionHandler_50(FileBasedTransactionLogHandler _enclosing
				)
			{
				this._enclosing = _enclosing;
			}

			public void CompleteInterruptedTransaction()
			{
				ByteArrayBuffer buffer = new ByteArrayBuffer(Const4.IntLength);
				this._enclosing.OpenLogFile();
				this._enclosing.Read(this._enclosing._logFile, buffer);
				int length = buffer.ReadInt();
				if (length > 0)
				{
					buffer = new ByteArrayBuffer(length);
					this._enclosing.Read(this._enclosing._logFile, buffer);
					buffer.IncrementOffset(Const4.IntLength);
					this._enclosing._idSystem.ReadWriteSlotChanges(buffer);
					this._enclosing.DeleteLockFile();
					this._enclosing._idSystem.FreeAndClearSystemSlotChanges();
				}
				else
				{
					this._enclosing.DeleteLockFile();
				}
				this._enclosing.CloseLogFile();
				this._enclosing.DeleteLogFile();
			}

			private readonly FileBasedTransactionLogHandler _enclosing;
		}

		private bool LockFileSignalsInterruptedTransaction()
		{
			OpenLockFile();
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
			_lockFile = null;
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
			_logFile = null;
		}

		private void DeleteLockFile()
		{
			File4.Delete(LockFileName(_fileName));
		}

		private void DeleteLogFile()
		{
			File4.Delete(LogFileName(_fileName));
		}

		public override Slot AllocateSlot(LocalTransaction transaction, bool append)
		{
			// do nothing
			return null;
		}

		public override void ApplySlotChanges(LocalTransaction transaction, Slot reservedSlot
			)
		{
			int slotChangeCount = CountSlotChanges(transaction);
			if (slotChangeCount < 1)
			{
				return;
			}
			FlushDatabaseFile();
			EnsureLogAndLock();
			int length = TransactionLogSlotLength(transaction);
			ByteArrayBuffer logBuffer = new ByteArrayBuffer(length);
			logBuffer.WriteInt(length);
			logBuffer.WriteInt(slotChangeCount);
			AppendSlotChanges(transaction, logBuffer);
			Write(_logFile, logBuffer);
			_logFile.Sync();
			WriteToLockFile(LockInt);
			if (_idSystem.WriteSlots(transaction))
			{
				FlushDatabaseFile();
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

		private void EnsureLogAndLock()
		{
			if (_idSystem.IsReadOnly())
			{
				return;
			}
			if (LogsOpened())
			{
				return;
			}
			OpenLockFile();
			OpenLogFile();
		}

		private void OpenLogFile()
		{
			_logFile = OpenBin(LogFileName(_fileName));
		}

		private void OpenLockFile()
		{
			_lockFile = OpenBin(LockFileName(_fileName));
		}

		private bool LogsOpened()
		{
			return _lockFile != null;
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
