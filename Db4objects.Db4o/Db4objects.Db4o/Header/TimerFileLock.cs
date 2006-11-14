namespace Db4objects.Db4o.Header
{
	/// <exclude></exclude>
	public abstract class TimerFileLock : Sharpen.Lang.IRunnable
	{
		public static Db4objects.Db4o.Header.TimerFileLock ForFile(Db4objects.Db4o.YapFile
			 file)
		{
			if (LockFile(file))
			{
				return new Db4objects.Db4o.Header.TimerFileLockEnabled(file);
			}
			return new Db4objects.Db4o.Header.TimerFileLockDisabled();
		}

		private static bool LockFile(Db4objects.Db4o.YapFile file)
		{
			return file.NeedsLockFileThread();
		}

		public abstract void CheckHeaderLock();

		public abstract void CheckOpenTime();

		public abstract bool LockFile();

		public abstract long OpenTime();

		public abstract void SetAddresses(int baseAddress, int openTimeOffset, int accessTimeOffset
			);

		public abstract void Start();

		public abstract void WriteHeaderLock();

		public abstract void WriteOpenTime();

		public abstract void Close();

		public abstract void Run();
	}
}
