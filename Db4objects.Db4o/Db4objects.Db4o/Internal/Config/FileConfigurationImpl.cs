/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Config
{
	public class FileConfigurationImpl : IFileConfiguration
	{
		private readonly Config4Impl _config;

		public FileConfigurationImpl(Config4Impl config)
		{
			_config = config;
		}

		public virtual int BlockSize
		{
			set
			{
				int bytes = value;
				_config.BlockSize(bytes);
			}
		}

		public virtual int DatabaseGrowthSize
		{
			set
			{
				int bytes = value;
				_config.DatabaseGrowthSize(bytes);
			}
		}

		public virtual void DisableCommitRecovery()
		{
			_config.DisableCommitRecovery();
		}

		public virtual IFreespaceConfiguration Freespace
		{
			get
			{
				return _config.Freespace();
			}
		}

		public virtual ConfigScope GenerateUUIDs
		{
			set
			{
				ConfigScope setting = value;
				_config.GenerateUUIDs(setting);
			}
		}

		public virtual ConfigScope GenerateVersionNumbers
		{
			set
			{
				ConfigScope setting = value;
				_config.GenerateVersionNumbers(setting);
			}
		}

		/// <exception cref="GlobalOnlyConfigException"></exception>
		public virtual IStorage Storage
		{
			get
			{
				return _config.Storage;
			}
			set
			{
				IStorage factory = value;
				_config.Storage = factory;
			}
		}

		public virtual bool LockDatabaseFile
		{
			set
			{
				bool flag = value;
				_config.LockDatabaseFile(flag);
			}
		}

		/// <exception cref="DatabaseReadOnlyException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		public virtual long ReserveStorageSpace
		{
			set
			{
				long byteCount = value;
				_config.ReserveStorageSpace(byteCount);
			}
		}

		/// <exception cref="IOException"></exception>
		public virtual string BlobPath
		{
			set
			{
				string path = value;
				_config.SetBlobPath(path);
			}
		}

		public virtual bool ReadOnly
		{
			set
			{
				bool flag = value;
				_config.ReadOnly(flag);
			}
		}

		public virtual bool RecoveryMode
		{
			set
			{
				bool flag = value;
				_config.RecoveryMode(flag);
			}
		}
	}
}
