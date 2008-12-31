/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;
using Sharpen;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class TimerFileLockEnabled : TimerFileLock
	{
		private readonly BlockAwareBin _timerFile;

		private readonly object _timerLock;

		private byte[] _longBytes = new byte[Const4.LongLength];

		private byte[] _intBytes = new byte[Const4.IntLength];

		private int _headerLockOffset = 2 + Const4.IntLength;

		private readonly long _opentime;

		private int _baseAddress = -1;

		private int _openTimeOffset;

		private int _accessTimeOffset;

		private bool _closed = false;

		public TimerFileLockEnabled(IoAdaptedObjectContainer file)
		{
			_timerLock = new object();
			_timerFile = file.TimerFile();
			_opentime = UniqueOpenTime();
		}

		public override void CheckHeaderLock()
		{
			long openTime = ReadInt(0, _headerLockOffset);
			if (((int)_opentime) != openTime)
			{
				throw new DatabaseFileLockedException(_timerFile.ToString());
			}
			WriteHeaderLock();
		}

		public override void CheckOpenTime()
		{
			long readOpenTime = ReadLong(_baseAddress, _openTimeOffset);
			if (_opentime != readOpenTime)
			{
				throw new DatabaseFileLockedException(_timerFile.ToString());
			}
			WriteOpenTime();
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void CheckIfOtherSessionAlive(LocalObjectContainer container, int
			 address, int offset, long lastAccessTime)
		{
			if (_timerFile == null)
			{
				// need to check? 
				return;
			}
			long waitTime = Const4.LockTimeInterval * 5;
			long currentTime = Runtime.CurrentTimeMillis();
			// If someone changes the system clock here, he is out of luck.
			while (Runtime.CurrentTimeMillis() < currentTime + waitTime)
			{
				Cool.SleepIgnoringInterruption(waitTime);
			}
			long currentAccessTime = ReadLong(address, offset);
			if ((currentAccessTime > lastAccessTime))
			{
				throw new DatabaseFileLockedException(container.ToString());
			}
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Close()
		{
			WriteAccessTime(true);
			lock (_timerLock)
			{
				_closed = true;
			}
		}

		public override bool LockFile()
		{
			return true;
		}

		public override long OpenTime()
		{
			return _opentime;
		}

		public override void Run()
		{
			while (true)
			{
				lock (_timerLock)
				{
					if (_closed)
					{
						return;
					}
					WriteAccessTime(false);
				}
				Cool.SleepIgnoringInterruption(Const4.LockTimeInterval);
			}
		}

		public override void SetAddresses(int baseAddress, int openTimeOffset, int accessTimeOffset
			)
		{
			_baseAddress = baseAddress;
			_openTimeOffset = openTimeOffset;
			_accessTimeOffset = accessTimeOffset;
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Start()
		{
			WriteAccessTime(false);
			CheckOpenTime();
			Thread thread = new Thread(this);
			thread.SetName("db4o file lock");
			thread.SetDaemon(true);
			thread.Start();
		}

		private long UniqueOpenTime()
		{
			return Runtime.CurrentTimeMillis();
		}

		// TODO: More security is possible here to make this time unique
		// to other processes. 
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		private bool WriteAccessTime(bool closing)
		{
			if (NoAddressSet())
			{
				return true;
			}
			long time = closing ? 0 : Runtime.CurrentTimeMillis();
			bool ret = WriteLong(_baseAddress, _accessTimeOffset, time);
			Sync();
			return ret;
		}

		private bool NoAddressSet()
		{
			return _baseAddress < 0;
		}

		public override void WriteHeaderLock()
		{
			WriteInt(0, _headerLockOffset, (int)_opentime);
			Sync();
		}

		public override void WriteOpenTime()
		{
			WriteLong(_baseAddress, _openTimeOffset, _opentime);
			Sync();
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		private bool WriteLong(int address, int offset, long time)
		{
			lock (_timerLock)
			{
				if (_timerFile == null)
				{
					return false;
				}
				PrimitiveCodec.WriteLong(_longBytes, time);
				_timerFile.BlockWrite(address, offset, _longBytes);
				return true;
			}
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		private long ReadLong(int address, int offset)
		{
			lock (_timerLock)
			{
				if (_timerFile == null)
				{
					return 0;
				}
				_timerFile.SyncRead(address + offset, _longBytes, Const4.LongLength);
				return PrimitiveCodec.ReadLong(_longBytes, 0);
			}
		}

		private bool WriteInt(int address, int offset, int time)
		{
			lock (_timerLock)
			{
				if (_timerFile == null)
				{
					return false;
				}
				PrimitiveCodec.WriteInt(_intBytes, 0, time);
				_timerFile.BlockWrite(address, offset, _intBytes);
				return true;
			}
		}

		private long ReadInt(int address, int offset)
		{
			lock (_timerLock)
			{
				if (_timerFile == null)
				{
					return 0;
				}
				_timerFile.SyncRead(address + offset, _longBytes, Const4.LongLength);
				return PrimitiveCodec.ReadInt(_longBytes, 0);
			}
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		private void Sync()
		{
			try
			{
				_timerFile.Sync();
			}
			catch (EmergencyShutdownReadOnlyException)
			{
			}
		}
		// ignore this one, emergency shutdown in progress
	}
}
