/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Api;
using Db4objects.Db4o.Tests.Common.Ext;

namespace Db4objects.Db4o.Tests.Common.Ext
{
	public class UnavailableClassesWithTypeHandlerTestCase : TestWithTempFile, IOptOutNetworkingCS
	{
		public class HolderForClassWithTypeHandler
		{
			public HolderForClassWithTypeHandler(Stack stack)
			{
				_fieldWithTypeHandler = stack;
			}

			public Stack _fieldWithTypeHandler;
		}

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(UnavailableClassesWithTypeHandlerTestCase)).Run();
		}

		public virtual void TestStoredClassesWithTypeHandler()
		{
			Store(TempFile(), new UnavailableClassesWithTypeHandlerTestCase.HolderForClassWithTypeHandler
				(new Stack()));
			AssertStoredClasses(TempFile());
		}

		private void AssertStoredClasses(string databaseFileName)
		{
			IObjectContainer db = Db4oEmbedded.OpenFile(ConfigExcludingStack(), databaseFileName
				);
			try
			{
				Assert.IsGreater(2, db.Ext().StoredClasses().Length);
			}
			finally
			{
				db.Close();
			}
		}

		private void Store(string databaseFileName, object obj)
		{
			IObjectContainer db = Db4oEmbedded.OpenFile(Db4oEmbedded.NewConfiguration(), databaseFileName
				);
			try
			{
				db.Store(obj);
			}
			finally
			{
				db.Close();
			}
		}

		private IEmbeddedConfiguration ConfigExcludingStack()
		{
			IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
			config.Common.ReflectWith(new ExcludingReflector(new Type[] { typeof(Stack) }));
			return config;
		}
	}
}
