/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Config;

namespace Db4objects.Db4o.Tests.Common.Config
{
	public class GlobalVsNonStaticConfigurationTestCase : IDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(GlobalVsNonStaticConfigurationTestCase)).Run();
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			new Sharpen.IO.File(Filename).Delete();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			new Sharpen.IO.File(Filename).Delete();
		}

		public class Data
		{
			public int id;

			public Data(int id)
			{
				this.id = id;
			}
		}

		private static readonly string Filename = Path.GetTempFileName();

		public virtual void TestOpenWithNonStaticConfiguration()
		{
			IConfiguration config1 = Db4oFactory.NewConfiguration();
			config1.ReadOnly(true);
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_43(config1));
			config1.ReadOnly(false);
			IObjectContainer db1 = Db4oFactory.OpenFile(config1, Filename);
			config1.ReadOnly(true);
			try
			{
				Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_52(db1));
			}
			finally
			{
				db1.Close();
			}
			IConfiguration config2 = Db4oFactory.NewConfiguration();
			IObjectContainer db2 = Db4oFactory.OpenFile(config2, Filename);
			try
			{
				db2.Store(new GlobalVsNonStaticConfigurationTestCase.Data(2));
				Assert.AreEqual(1, db2.Query(typeof(GlobalVsNonStaticConfigurationTestCase.Data))
					.Size());
			}
			finally
			{
				db2.Close();
			}
		}

		private sealed class _ICodeBlock_43 : ICodeBlock
		{
			public _ICodeBlock_43(IConfiguration config1)
			{
				this.config1 = config1;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenFile(config1, GlobalVsNonStaticConfigurationTestCase.Filename);
			}

			private readonly IConfiguration config1;
		}

		private sealed class _ICodeBlock_52 : ICodeBlock
		{
			public _ICodeBlock_52(IObjectContainer db1)
			{
				this.db1 = db1;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				db1.Store(new GlobalVsNonStaticConfigurationTestCase.Data(1));
			}

			private readonly IObjectContainer db1;
		}

		public virtual void TestOpenWithStaticConfiguration()
		{
			Db4oFactory.Configure().ReadOnly(true);
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_73());
			Db4oFactory.Configure().ReadOnly(false);
			IObjectContainer db = Db4oFactory.OpenFile(Filename);
			db.Store(new GlobalVsNonStaticConfigurationTestCase.Data(1));
			db.Close();
			db = Db4oFactory.OpenFile(Filename);
			Assert.AreEqual(1, db.Query(typeof(GlobalVsNonStaticConfigurationTestCase.Data)).
				Size());
			db.Close();
		}

		private sealed class _ICodeBlock_73 : ICodeBlock
		{
			public _ICodeBlock_73()
			{
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenFile(GlobalVsNonStaticConfigurationTestCase.Filename);
			}
		}

		public virtual void TestIndependentObjectConfigs()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			IObjectClass objectConfig = config.ObjectClass(typeof(GlobalVsNonStaticConfigurationTestCase.Data
				));
			objectConfig.Translate(new TNull());
			IConfiguration otherConfig = Db4oFactory.NewConfiguration();
			Assert.AreNotSame(config, otherConfig);
			Config4Class otherObjectConfig = (Config4Class)otherConfig.ObjectClass(typeof(GlobalVsNonStaticConfigurationTestCase.Data
				));
			Assert.AreNotSame(objectConfig, otherObjectConfig);
			Assert.IsNull(otherObjectConfig.GetTranslator());
		}
	}
}
