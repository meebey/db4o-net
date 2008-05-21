/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Drs.Db4o;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;
using Sharpen.IO;

namespace Db4objects.Drs.Tests
{
	public class Db4oDrsFixture : IDrsFixture
	{
		internal static readonly File RamDrive = new File("w:");

		protected string _name;

		protected IExtObjectContainer _db;

		protected ITestableReplicationProviderInside _provider;

		protected readonly File testFile;

		private IConfiguration _config;

		public Db4oDrsFixture(string name)
		{
			_name = name;
			if (RamDrive.Exists())
			{
				testFile = new File(RamDrive.GetPath() + "drs_cs_" + _name + ".yap");
			}
			else
			{
				testFile = new File("drs_cs_" + _name + ".yap");
			}
		}

		public virtual ITestableReplicationProviderInside Provider()
		{
			return _provider;
		}

		public virtual void Clean()
		{
			testFile.Delete();
			_config = null;
		}

		public virtual void Close()
		{
			_provider.Destroy();
			_db.Close();
		}

		public virtual IExtObjectContainer Db()
		{
			return _db;
		}

		public virtual void Open()
		{
			//	Comment out because MemoryIoAdapter has problems on .net 
			//	MemoryIoAdapter memoryIoAdapter = new MemoryIoAdapter();
			//	Db4o.configure().io(memoryIoAdapter);
			_db = Db4oFactory.OpenFile(Config(), testFile.GetPath()).Ext();
			_provider = Db4oProviderFactory.NewInstance(_db, _name);
		}

		public virtual IConfiguration Config()
		{
			if (_config == null)
			{
				_config = Db4oFactory.NewConfiguration();
			}
			return _config;
		}
	}
}
