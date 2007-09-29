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
namespace Db4objects.Drs.Test
{
	public class DrsTestSuiteBuilder : Db4oUnit.ReflectionTestSuiteBuilder
	{
		private Db4objects.Drs.Test.IDrsFixture _a;

		private Db4objects.Drs.Test.IDrsFixture _b;

		public DrsTestSuiteBuilder(Db4objects.Drs.Test.IDrsFixture a, Db4objects.Drs.Test.IDrsFixture
			 b, System.Type clazz) : base(clazz)
		{
			A(a);
			B(b);
		}

		public DrsTestSuiteBuilder(Db4objects.Drs.Test.IDrsFixture a, Db4objects.Drs.Test.IDrsFixture
			 b, System.Type[] classes) : base(classes)
		{
			A(a);
			B(b);
		}

		private void A(Db4objects.Drs.Test.IDrsFixture fixture)
		{
			if (null == fixture)
			{
				throw new System.ArgumentException("fixture");
			}
			_a = fixture;
		}

		private void B(Db4objects.Drs.Test.IDrsFixture fixture)
		{
			if (null == fixture)
			{
				throw new System.ArgumentException("fixture");
			}
			_b = fixture;
		}

		protected override object NewInstance(System.Type clazz)
		{
			object instance = base.NewInstance(clazz);
			if (instance is Db4objects.Drs.Test.DrsTestCase)
			{
				Db4objects.Drs.Test.DrsTestCase testCase = (Db4objects.Drs.Test.DrsTestCase)instance;
				testCase.A(_a);
				testCase.B(_b);
			}
			return instance;
		}
	}
}
