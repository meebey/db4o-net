/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Config
{
	public class LocalConfigurationImpl : ILocalConfiguration
	{
		private readonly Config4Impl _config;

		public LocalConfigurationImpl(Config4Impl config)
		{
			_config = config;
		}

		public virtual void AddAlias(IAlias alias)
		{
			_config.AddAlias(alias);
		}

		public virtual void RemoveAlias(IAlias alias)
		{
			_config.RemoveAlias(alias);
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
		public virtual IoAdapter Io
		{
			get
			{
				return _config.Io();
			}
			set
			{
				IoAdapter adapter = value;
				_config.Io(adapter);
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
	}
}
