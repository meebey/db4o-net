/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Freespace;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Tests.Common.Migration;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class Db4oMigrationTestSuite : ITestSuiteBuilder
	{
		public static void Main(string[] args)
		{
			new TestRunner(typeof(Db4oMigrationTestSuite)).Run();
		}

		public virtual TestSuite Build()
		{
			return new Db4oMigrationSuiteBuilder(TestCases(), Libraries()).Build();
		}

		private string[] Libraries()
		{
			if (true)
			{
				return Db4oMigrationSuiteBuilder.ALL;
			}
			if (true)
			{
				return new string[] { WorkspaceServices.WorkspacePath("db4o.archives/java1.2/db4o-3.0.jar"
					), WorkspaceServices.WorkspacePath("db4o.archives/java1.2/db4o-4.0-java1.1.jar")
					 };
			}
			return Db4oMigrationSuiteBuilder.CURRENT;
		}

		protected virtual Type[] TestCases()
		{
			return new Type[] { typeof(IxFreespaceMigrationTestCase), typeof(BooleanHandlerUpdateTestCase
				), typeof(ByteHandlerUpdateTestCase), typeof(CascadedDeleteFileFormatUpdateTestCase
				), typeof(CharHandlerUpdateTestCase), typeof(DateHandlerUpdateTestCase), typeof(
				DoubleHandlerUpdateTestCase), typeof(FloatHandlerUpdateTestCase), typeof(IntHandlerUpdateTestCase
				), typeof(LongHandlerUpdateTestCase), typeof(MultiDimensionalArrayHandlerUpdateTestCase
				), typeof(NestedArrayUpdateTestCase), typeof(ObjectArrayUpdateTestCase), typeof(
				ShortHandlerUpdateTestCase), typeof(StringHandlerUpdateTestCase) };
		}
	}
}
