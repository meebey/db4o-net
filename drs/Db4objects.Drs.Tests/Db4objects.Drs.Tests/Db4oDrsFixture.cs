/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Tests
{
	public class Db4oDrsFixture : Db4objects.Drs.Tests.IDrsFixture
	{
		internal static readonly Sharpen.IO.File RAM_DRIVE = new Sharpen.IO.File("w:");

		protected string _name;

		protected Db4objects.Db4o.Ext.IExtObjectContainer _db;

		protected Db4objects.Drs.Inside.ITestableReplicationProviderInside _provider;

		protected readonly Sharpen.IO.File testFile;

		public Db4oDrsFixture(string name)
		{
			_name = name;
			if (RAM_DRIVE.Exists())
			{
				testFile = new Sharpen.IO.File(RAM_DRIVE.GetPath() + "drs_cs_" + _name + ".yap");
			}
			else
			{
				testFile = new Sharpen.IO.File("drs_cs_" + _name + ".yap");
			}
		}

		public virtual Db4objects.Drs.Inside.ITestableReplicationProviderInside Provider(
			)
		{
			return _provider;
		}

		public virtual void Clean()
		{
			testFile.Delete();
		}

		public virtual void Close()
		{
			_provider.Destroy();
			_db.Close();
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer Db()
		{
			return _db;
		}

		public virtual void Open()
		{
			_db = Db4objects.Db4o.Db4oFactory.OpenFile(testFile.GetPath()).Ext();
			_provider = Db4objects.Drs.Db4o.Db4oProviderFactory.NewInstance(_db, _name);
		}
	}
}
