using System.IO;
using Db4objects.Db4o;
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
		private readonly IoAdapter _timerFile;

		private readonly object _timerLock;

		private byte[] _longBytes = new byte[Const4.LONG_LENGTH];

		private byte[] _intBytes = new byte[Const4.INT_LENGTH];

		private int _headerLockOffset = 2 + Const4.INT_LENGTH;

		private readonly long _opentime;

		private int _baseAddress = -1;

		private int _openTimeOffset;

		private int _accessTimeOffset;

		private bool _closed = false;

		public TimerFileLockEnabled(IoAdaptedObjectContainer file)
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
			catch (IOException)
			{
			}
			throw new DatabaseFileLockedException(_timerFile.ToString());
		}

		public override void CheckOpenTime()
		{
			try
			{
				long readOpenTime = ReadLong(_baseAddress, _openTimeOffset);
				if (_opentime == readOpenTime)
				{
					WriteOpenTime();
					return;
				}
			}
			catch (IOException)
			{
			}
			throw new DatabaseFileLockedException(_timerFile.ToString());
		}

		public override void CheckIfOtherSessionAlive(LocalObjectContainer container, int
			 address, int offset, long lastAccessTime)
		{
			if (_timerFile == null)
			{
				return;
			}
			long waitTime = Const4.LOCK_TIME_INTERVAL * 5;
			long currentTime = Runtime.CurrentTimeMillis();
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
			Thread t = Thread.CurrentThread();
			t.SetName("db4o file lock");
			try
			{
				while (WriteAccessTime(false))
				{
					Cool.SleepIgnoringInterruption(Const4.LOCK_TIME_INTERVAL);
					if (_closed)
					{
						break;
					}
				}
			}
			catch (IOException)
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
			new Thread(this).Start();
		}

		private long UniqueOpenTime()
		{
			return Runtime.CurrentTimeMillis();
		}

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
			try
			{
				WriteInt(0, _headerLockOffset, (int)_opentime);
				Sync();
			}
			catch (IOException)
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
			catch (IOException)
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
				PrimitiveCodec.WriteLong(_longBytes, time);
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
				_timerFile.BlockSeek(address, offset);
				PrimitiveCodec.WriteInt(_intBytes, 0, time);
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
				return PrimitiveCodec.ReadInt(_longBytes, 0);
			}
		}

		private void Sync()
		{
			_timerFile.Sync();
		}
	}
}
