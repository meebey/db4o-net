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
using System;
using Db4oUnit;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Db4o;
using Db4objects.Drs.Tests.Dotnet;

namespace Db4objects.Drs.Tests
{
	public partial class Db4oTests : DrsTestSuite
	{
		public static int Main(string[] args)
		{
			//if (true) return new Db4oTests().runDb4oDb4o();
			int failureCount = new Db4oTests().RunDb4oDb4o();
			failureCount += new Db4oTests().Rundb4oCS();
			//new Db4oTests().runCSdb4o();
			failureCount += new Db4oTests().RunCSCS();
			//new Db4oTests().runDb4oDb4o();
			return failureCount;
		}

		public virtual int RunDb4oDb4o()
		{
			return new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oDrsFixture("db4o-a")
				, new Db4oDrsFixture("db4o-b"), GetType())).Run();
		}

		public virtual int RunCSCS()
		{
			return new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oClientServerDrsFixture
				("db4o-cs-a", unchecked((int)(0xdb40))), new Db4oClientServerDrsFixture("db4o-cs-b"
				, 4455), GetType())).Run();
		}

		public virtual int Rundb4oCS()
		{
			return new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oDrsFixture("db4o-a")
				, new Db4oClientServerDrsFixture("db4o-cs-b", 4455), GetType())).Run();
		}

		public virtual void RunCSdb4o()
		{
			new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oClientServerDrsFixture("db4o-cs-a"
				, 4455), new Db4oDrsFixture("db4o-b"), GetType())).Run();
		}

		protected override Type[] SpecificTestCases()
		{
			return Concat(PlatformSpecificTestCases(), new Type[] { typeof(ArrayTestSuite), typeof(
				CustomArrayListTestCase), typeof(DateReplicationTestCase), typeof(StructTestCase
				), typeof(DeepListGraphTestCase), typeof(UntypedFieldTestCase), typeof(PartialCollectionReplicationTestCase
				), typeof(TheSimplestWithCallConstructors) });
		}
	}
}
