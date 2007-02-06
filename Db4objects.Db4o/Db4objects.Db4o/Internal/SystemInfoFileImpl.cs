namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class SystemInfoFileImpl : Db4objects.Db4o.Ext.ISystemInfo
	{
		private Db4objects.Db4o.Internal.LocalObjectContainer _file;

		public SystemInfoFileImpl(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			_file = file;
		}

		public virtual int FreespaceEntryCount()
		{
			if (!HasFreespaceManager())
			{
				return 0;
			}
			return FreespaceManager().EntryCount();
		}

		private bool HasFreespaceManager()
		{
			return FreespaceManager() != null;
		}

		private Db4objects.Db4o.Internal.Freespace.FreespaceManager FreespaceManager()
		{
			return _file.FreespaceManager();
		}

		public virtual long FreespaceSize()
		{
			if (!HasFreespaceManager())
			{
				return 0;
			}
			long blockSize = _file.BlockSize();
			long blockedSize = FreespaceManager().FreeSize();
			return blockSize * blockedSize;
		}

		public virtual long TotalSize()
		{
			return _file.FileLength();
		}
	}
}
