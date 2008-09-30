/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
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
