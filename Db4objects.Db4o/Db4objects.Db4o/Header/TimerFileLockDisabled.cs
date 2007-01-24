namespace Db4objects.Db4o.Header
{
	/// <exclude></exclude>
	public class TimerFileLockDisabled : Db4objects.Db4o.Header.TimerFileLock
	{
		public override void CheckHeaderLock()
		{
		}

		public override void CheckOpenTime()
		{
		}

		public override void Close()
		{
		}

		public override bool LockFile()
		{
			return false;
		}

		public override long OpenTime()
		{
			return 0;
		}

		public override void Run()
		{
		}

		public override void SetAddresses(int baseAddress, int openTimeOffset, int accessTimeOffset
			)
		{
		}

		public override void Start()
		{
		}

		public override void WriteHeaderLock()
		{
		}

		public override void WriteOpenTime()
		{
		}
	}
}
