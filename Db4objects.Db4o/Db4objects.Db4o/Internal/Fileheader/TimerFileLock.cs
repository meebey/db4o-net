/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public abstract class TimerFileLock : IRunnable
	{
		public static TimerFileLock ForFile(LocalObjectContainer file)
		{
			if (file.NeedsLockFileThread())
			{
				return new TimerFileLockEnabled((IoAdaptedObjectContainer)file);
			}
			return new TimerFileLockDisabled();
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

		public abstract void CheckIfOtherSessionAlive(LocalObjectContainer container, int
			 address, int offset, long lastAccessTime);

		public abstract void Run();
	}
}
