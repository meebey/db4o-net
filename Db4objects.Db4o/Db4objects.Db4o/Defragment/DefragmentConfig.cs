namespace Db4objects.Db4o.Defragment
{
	/// <summary>Configuration for a defragmentation run.</summary>
	/// <remarks>Configuration for a defragmentation run.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public class DefragmentConfig
	{
		public const bool DEBUG = false;

		public static readonly string BACKUP_SUFFIX = "backup";

		private string _origPath;

		private string _backupPath;

		private Db4objects.Db4o.Defragment.IContextIDMapping _mapping;

		private Db4objects.Db4o.Defragment.IStoredClassFilter _storedClassFilter = null;

		private bool _forceBackupDelete = false;

		public DefragmentConfig(string origPath) : this(origPath, origPath + "." + BACKUP_SUFFIX
			)
		{
		}

		public DefragmentConfig(string origPath, string backupPath) : this(origPath, backupPath
			, new Db4objects.Db4o.Defragment.TreeIDMapping())
		{
		}

		public DefragmentConfig(string origPath, string backupPath, Db4objects.Db4o.Defragment.IContextIDMapping
			 mapping)
		{
			_origPath = origPath;
			_backupPath = backupPath;
			_mapping = mapping;
		}

		public virtual string OrigPath()
		{
			return _origPath;
		}

		public virtual string BackupPath()
		{
			return _backupPath;
		}

		public virtual Db4objects.Db4o.Defragment.IContextIDMapping Mapping()
		{
			return _mapping;
		}

		public virtual Db4objects.Db4o.Defragment.IStoredClassFilter StoredClassFilter()
		{
			return (_storedClassFilter == null ? NULLFILTER : _storedClassFilter);
		}

		public virtual void StoredClassFilter(Db4objects.Db4o.Defragment.IStoredClassFilter
			 storedClassFilter)
		{
			_storedClassFilter = storedClassFilter;
		}

		public virtual bool ForceBackupDelete()
		{
			return _forceBackupDelete;
		}

		public virtual void ForceBackupDelete(bool forceBackupDelete)
		{
			_forceBackupDelete = forceBackupDelete;
		}

		internal class NullFilter : Db4objects.Db4o.Defragment.IStoredClassFilter
		{
			public virtual bool Accept(Db4objects.Db4o.Ext.IStoredClass storedClass)
			{
				return true;
			}
		}

		private static readonly Db4objects.Db4o.Defragment.IStoredClassFilter NULLFILTER = 
			new Db4objects.Db4o.Defragment.DefragmentConfig.NullFilter();

		public static Db4objects.Db4o.Config.IConfiguration Db4oConfig()
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4o.NewConfiguration
				();
			config.WeakReferences(false);
			return config;
		}
	}
}
