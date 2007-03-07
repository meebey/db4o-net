namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public abstract class TimerFileLock : Sharpen.Lang.IRunnable
	{
		public static Db4objects.Db4o.Internal.Fileheader.TimerFileLock ForFile(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			if (file.NeedsLockFileThread())
			{
				return new Db4objects.Db4o.Internal.Fileheader.TimerFileLockEnabled((Db4objects.Db4o.Internal.IoAdaptedObjectContainer
					)file);
			}
			return new Db4objects.Db4o.Internal.Fileheader.TimerFileLockDisabled();
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
