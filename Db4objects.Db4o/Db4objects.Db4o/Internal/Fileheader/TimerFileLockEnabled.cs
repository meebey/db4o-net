namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class TimerFileLockEnabled : Db4objects.Db4o.Internal.Fileheader.TimerFileLock
	{
		private readonly Db4objects.Db4o.IO.IoAdapter _timerFile;

		private readonly object _timerLock;

		private byte[] _longBytes = new byte[Db4objects.Db4o.Internal.Const4.LONG_LENGTH];

		private byte[] _intBytes = new byte[Db4objects.Db4o.Internal.Const4.INT_LENGTH];

		private int _headerLockOffset = 2 + Db4objects.Db4o.Internal.Const4.INT_LENGTH;

		private readonly long _opentime;

		private int _baseAddress = -1;

		private int _openTimeOffset;

		private int _accessTimeOffset;

		private bool _closed = false;

		public TimerFileLockEnabled(Db4objects.Db4o.Internal.IoAdaptedObjectContainer file
			)
		{
			_timerLock = file.Lock();
			_timerFile = file.TimerFile();
			_opentime = UniqueOpenTime();
		}

		public override void CheckHeaderLock()
		{
			try
			{
				if (((int)_opentime) == ReadInt(0, _headerLockOffset))
				{
					WriteHeaderLock();
					return;
				}
			}
			catch (System.IO.IOException)
			{
			}
			throw new Db4objects.Db4o.Ext.DatabaseFileLockedException();
		}

		public override void CheckOpenTime()
		{
			try
			{
				if (_opentime == ReadLong(_baseAddress, _openTimeOffset))
				{
					WriteOpenTime();
					return;
				}
			}
			catch (System.IO.IOException)
			{
			}
			throw new Db4objects.Db4o.Ext.DatabaseFileLockedException();
		}

		public override void Close()
		{
			WriteAccessTime(true);
			_closed = true;
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
			Sharpen.Lang.Thread t = Sharpen.Lang.Thread.CurrentThread();
			t.SetName("db4o file lock");
			try
			{
				while (WriteAccessTime(false))
				{
					Db4objects.Db4o.Foundation.Cool.SleepIgnoringInterruption(Db4objects.Db4o.Internal.Const4
						.LOCK_TIME_INTERVAL);
					if (_closed)
					{
						break;
					}
				}
			}
			catch (System.IO.IOException)
			{
			}
		}

		public override void SetAddresses(int baseAddress, int openTimeOffset, int accessTimeOffset
			)
		{
			_baseAddress = baseAddress;
			_openTimeOffset = openTimeOffset;
			_accessTimeOffset = accessTimeOffset;
		}

		public override void Start()
		{
			WriteAccessTime(false);
			_timerFile.Sync();
			CheckOpenTime();
			new Sharpen.Lang.Thread(this).Start();
		}

		private long UniqueOpenTime()
		{
			return Sharpen.Runtime.CurrentTimeMillis();
		}

		private bool WriteAccessTime(bool closing)
		{
			if (NoAddressSet())
			{
				return true;
			}
			long time = closing ? 0 : Sharpen.Runtime.CurrentTimeMillis();
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
			try
			{
				WriteInt(0, _headerLockOffset, (int)_opentime);
				Sync();
			}
			catch (System.IO.IOException)
			{
			}
		}

		public override void WriteOpenTime()
		{
			try
			{
				WriteLong(_baseAddress, _openTimeOffset, _opentime);
				Sync();
			}
			catch (System.IO.IOException)
			{
			}
		}

		private bool WriteLong(int address, int offset, long time)
		{
			lock (_timerLock)
			{
				if (_timerFile == null)
				{
					return false;
				}
				_timerFile.BlockSeek(address, offset);
				Db4objects.Db4o.Foundation.PrimitiveCodec.WriteLong(_longBytes, time);
				_timerFile.Write(_longBytes);
				return true;
			}
		}

		private long ReadLong(int address, int offset)
		{
			lock (_timerLock)
			{
				if (_timerFile == null)
				{
					return 0;
				}
				_timerFile.BlockSeek(address, offset);
				_timerFile.Read(_longBytes);
				return Db4objects.Db4o.Foundation.PrimitiveCodec.ReadLong(_longBytes, 0);
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
				_timerFile.BlockSeek(address, offset);
				Db4objects.Db4o.Foundation.PrimitiveCodec.WriteInt(_intBytes, 0, time);
				_timerFile.Write(_intBytes);
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
				_timerFile.BlockSeek(address, offset);
				_timerFile.Read(_longBytes);
				return Db4objects.Db4o.Foundation.PrimitiveCodec.ReadInt(_longBytes, 0);
			}
		}

		private void Sync()
		{
			_timerFile.Sync();
		}
	}
}
