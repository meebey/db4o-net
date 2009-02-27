/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
	public class GlobalVsNonStaticConfigurationTestCase : IDb4oTestCase, ITestLifeCycle
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(GlobalVsNonStaticConfigurationTestCase)).Run();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			new Sharpen.IO.File(Filename).Delete();
		}

		/// <exception cref="System.Exception"></exception>
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
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_44(config1));
			IConfiguration config2 = Db4oFactory.NewConfiguration();
			IObjectContainer db2 = Db4oFactory.OpenFile(config2, Filename);
			try
			{
				db2.Store(new GlobalVsNonStaticConfigurationTestCase.Data(2));
				Assert.AreEqual(1, db2.Query(typeof(GlobalVsNonStaticConfigurationTestCase.Data))
					.Count);
			}
			finally
			{
				db2.Close();
			}
		}

		private sealed class _ICodeBlock_44 : ICodeBlock
		{
			public _ICodeBlock_44(IConfiguration config1)
			{
				this.config1 = config1;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenFile(config1, GlobalVsNonStaticConfigurationTestCase.Filename);
			}

			private readonly IConfiguration config1;
		}

		[System.ObsoleteAttribute(@"using deprecated api")]
		public virtual void TestOpenWithStaticConfiguration()
		{
			Db4oFactory.Configure().ReadOnly(true);
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_65());
			Db4oFactory.Configure().ReadOnly(false);
			IObjectContainer db = Db4oFactory.OpenFile(Filename);
			db.Store(new GlobalVsNonStaticConfigurationTestCase.Data(1));
			db.Close();
			db = Db4oFactory.OpenFile(Filename);
			Assert.AreEqual(1, db.Query(typeof(GlobalVsNonStaticConfigurationTestCase.Data)).
				Count);
			db.Close();
		}

		private sealed class _ICodeBlock_65 : ICodeBlock
		{
			public _ICodeBlock_65()
			{
			}

			/// <exception cref="System.Exception"></exception>
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
