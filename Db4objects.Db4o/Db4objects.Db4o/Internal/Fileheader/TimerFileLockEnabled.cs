namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class TimerFileLockEnabled : Db4objects.Db4o.Internal.Fileheader.TimerFileLock
	{
		private readonly Db4objects.Db4o.Internal.LocalObjectContainer _file;

		private int _headerLockOffset = 2 + Db4objects.Db4o.Internal.Const4.INT_LENGTH;

		private readonly long _opentime;

		private int _baseAddress;

		private int _openTimeOffset;

		private int _accessTimeOffset;

		private bool _closed = false;

		public TimerFileLockEnabled(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			_file = file;
			_opentime = UniqueOpenTime();
		}

		public override void CheckHeaderLock()
		{
			Db4objects.Db4o.Internal.StatefulBuffer reader = HeaderLockIO();
			reader.Read();
			if (reader.ReadInt() != (int)_opentime)
			{
				throw new Db4objects.Db4o.Ext.DatabaseFileLockedException();
			}
			WriteHeaderLock();
		}

		public override void CheckOpenTime()
		{
			Db4objects.Db4o.Internal.StatefulBuffer reader = OpenTimeIO();
			if (reader == null)
			{
				return;
			}
			reader.Read();
			if (reader.ReadLong() != _opentime)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(22);
			}
			WriteOpenTime();
		}

		public override void Close()
		{
			WriteAccessTime(true);
			_closed = true;
		}

		private Db4objects.Db4o.Internal.StatefulBuffer GetWriter(int address, int offset
			, int length)
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = _file.GetWriter(_file.GetTransaction
				(), address, length);
			writer.MoveForward(offset);
			return writer;
		}

		private Db4objects.Db4o.Internal.StatefulBuffer HeaderLockIO()
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = GetWriter(0, _headerLockOffset, 
				Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			return writer;
		}

		public override bool LockFile()
		{
			return true;
		}

		public override long OpenTime()
		{
			return _opentime;
		}

		private Db4objects.Db4o.Internal.StatefulBuffer OpenTimeIO()
		{
			if (_baseAddress == 0)
			{
				return null;
			}
			Db4objects.Db4o.Internal.StatefulBuffer writer = GetWriter(_baseAddress, _openTimeOffset
				, Db4objects.Db4o.Internal.Const4.LONG_LENGTH);
			return writer;
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
			_file.SyncFiles();
			CheckOpenTime();
			new Sharpen.Lang.Thread(this).Start();
		}

		private long UniqueOpenTime()
		{
			return Sharpen.Runtime.CurrentTimeMillis();
		}

		private bool WriteAccessTime(bool closing)
		{
			if (_baseAddress < 1)
			{
				return true;
			}
			long time = closing ? 0 : Sharpen.Runtime.CurrentTimeMillis();
			return _file.WriteAccessTime(_baseAddress, _accessTimeOffset, time);
		}

		public override void WriteHeaderLock()
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = HeaderLockIO();
			writer.WriteInt((int)_opentime);
			writer.Write();
		}

		public override void WriteOpenTime()
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = OpenTimeIO();
			if (writer == null)
			{
				return;
			}
			writer.WriteLong(_opentime);
			writer.Write();
		}
	}
}
