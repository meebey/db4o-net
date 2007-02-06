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

		private Db4objects.Db4o.Config.IConfiguration _config;

		private Db4objects.Db4o.Defragment.IStoredClassFilter _storedClassFilter = null;

		private bool _forceBackupDelete = false;

		private int _objectCommitFrequency;

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

		/// <returns>The path to the file to be defragmented.</returns>
		public virtual string OrigPath()
		{
			return _origPath;
		}

		/// <returns>The path to the backup of the original file.</returns>
		public virtual string BackupPath()
		{
			return _backupPath;
		}

		/// <returns>The intermediate mapping used internally. For internal use only.</returns>
		public virtual Db4objects.Db4o.Defragment.IContextIDMapping Mapping()
		{
			return _mapping;
		}

		/// <returns>
		/// The
		/// <see cref="Db4objects.Db4o.Defragment.IStoredClassFilter">Db4objects.Db4o.Defragment.IStoredClassFilter
		/// 	</see>
		/// used to select stored class extents to
		/// be included into the defragmented file.
		/// </returns>
		public virtual Db4objects.Db4o.Defragment.IStoredClassFilter StoredClassFilter()
		{
			return (_storedClassFilter == null ? NULLFILTER : _storedClassFilter);
		}

		/// <param name="storedClassFilter">
		/// The
		/// <see cref="Db4objects.Db4o.Defragment.IStoredClassFilter">Db4objects.Db4o.Defragment.IStoredClassFilter
		/// 	</see>
		/// used to select stored class extents to
		/// be included into the defragmented file.
		/// </param>
		public virtual void StoredClassFilter(Db4objects.Db4o.Defragment.IStoredClassFilter
			 storedClassFilter)
		{
			_storedClassFilter = storedClassFilter;
		}

		/// <returns>true, if an existing backup file should be deleted, false otherwise.</returns>
		public virtual bool ForceBackupDelete()
		{
			return _forceBackupDelete;
		}

		/// <param name="forceBackupDelete">true, if an existing backup file should be deleted, false otherwise.
		/// 	</param>
		public virtual void ForceBackupDelete(bool forceBackupDelete)
		{
			_forceBackupDelete = forceBackupDelete;
		}

		/// <returns>
		/// The db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// to be applied
		/// during the defragment process.
		/// </returns>
		public virtual Db4objects.Db4o.Config.IConfiguration Db4oConfig()
		{
			if (_config == null)
			{
				_config = VanillaDb4oConfig();
			}
			return _config;
		}

		/// <param name="config">
		/// The db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// to be applied
		/// during the defragment process.
		/// </param>
		public virtual void Db4oConfig(Db4objects.Db4o.Config.IConfiguration config)
		{
			_config = config;
		}

		public virtual int ObjectCommitFrequency()
		{
			return _objectCommitFrequency;
		}

		public virtual void ObjectCommitFrequency(int objectCommitFrequency)
		{
			_objectCommitFrequency = objectCommitFrequency;
		}

		private class NullFilter : Db4objects.Db4o.Defragment.IStoredClassFilter
		{
			public virtual bool Accept(Db4objects.Db4o.Ext.IStoredClass storedClass)
			{
				return true;
			}
		}

		private static readonly Db4objects.Db4o.Defragment.IStoredClassFilter NULLFILTER = 
			new Db4objects.Db4o.Defragment.DefragmentConfig.NullFilter();

		public static Db4objects.Db4o.Config.IConfiguration VanillaDb4oConfig()
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			config.WeakReferences(false);
			return config;
		}
	}
}
