/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Tests.Common.Api;
using Db4objects.Db4o.Tests.Common.Migration;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class MigrationHopsTestCase : TestWithTempFile
	{
		private Db4oLibraryEnvironmentProvider _environmentProvider;

		public class Item
		{
			public string version;

			public Item()
			{
			}

			public Item(string version)
			{
				this.version = version;
			}
		}

		public class Tester
		{
			public virtual void CreateDatabase(string filename)
			{
				WithContainer(filename, new _IFunction4_33());
			}

			private sealed class _IFunction4_33 : IFunction4
			{
				public _IFunction4_33()
				{
				}

				public object Apply(object container)
				{
					((IObjectContainer)container).Set(new MigrationHopsTestCase.Item(Sharpen.Runtime.Substring
						(Db4oFactory.Version(), 5)));
					return null;
				}
			}

			public virtual string CurrentVersion(string filename)
			{
				return ((string)WithContainer(filename, new _IFunction4_40(this)));
			}

			private sealed class _IFunction4_40 : IFunction4
			{
				public _IFunction4_40(Tester _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public object Apply(object container)
				{
					return this._enclosing.CurrentVersion(((IObjectContainer)container));
				}

				private readonly Tester _enclosing;
			}

			public virtual string CurrentVersion(IObjectContainer container)
			{
				return ((MigrationHopsTestCase.Item)container.Get(typeof(MigrationHopsTestCase.Item
					)).Next()).version;
			}

			private static object WithContainer(string filename, IFunction4 block)
			{
				IObjectContainer container = Db4oFactory.OpenFile(filename);
				try
				{
					return block.Apply(container);
				}
				finally
				{
					container.Close();
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			Db4oLibraryEnvironment originalEnv = EnvironmentForVersion("5.7");
			originalEnv.InvokeInstanceMethod(typeof(MigrationHopsTestCase.Tester), "createDatabase"
				, new object[] { TempFile() });
			string[] hopArray = new string[] { "6.4", "7.4", "7.7" };
			for (int hopIndex = 0; hopIndex < hopArray.Length; ++hopIndex)
			{
				string hop = hopArray[hopIndex];
				Db4oLibraryEnvironment hopEnvironment = EnvironmentForVersion(hop);
				Assert.AreEqual(originalEnv.Version(), InvokeTesterMethodOn(hopEnvironment, "currentVersion"
					));
			}
			IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
			config.Common.ReflectWith(new ExcludingReflector(new Type[] { typeof(MigrationHopsTestCase.Item
				) }));
			IEmbeddedObjectContainer container = Db4oEmbedded.OpenFile(config, TempFile());
			try
			{
				Assert.AreEqual(originalEnv.Version(), ((GenericObject)container.Get(typeof(MigrationHopsTestCase.Item
					)).Next()).Get(0));
			}
			finally
			{
				container.Close();
			}
		}

		/// <exception cref="System.Exception"></exception>
		private object InvokeTesterMethodOn(Db4oLibraryEnvironment env74, string methodName
			)
		{
			return env74.InvokeInstanceMethod(typeof(MigrationHopsTestCase.Tester), methodName
				, new object[] { TempFile() });
		}

		/// <exception cref="System.IO.IOException"></exception>
		private Db4oLibraryEnvironment EnvironmentForVersion(string version)
		{
			return new Db4oLibrarian(_environmentProvider).ForVersion(version).environment;
		}

		/// <exception cref="System.Exception"></exception>
		public override void SetUp()
		{
			base.SetUp();
			_environmentProvider = new Db4oLibraryEnvironmentProvider(PathProvider.TestCasePath
				());
		}

		/// <exception cref="System.Exception"></exception>
		public override void TearDown()
		{
			_environmentProvider.DisposeAll();
			base.TearDown();
		}
	}
}
