/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Config;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Config
{
	/// <summary>Tests all combinations of configuration use/reuse scenarios.</summary>
	/// <remarks>Tests all combinations of configuration use/reuse scenarios.</remarks>
	[System.ObsoleteAttribute(@"tests deprecated api")]
	public class ConfigurationReuseTestSuite : FixtureTestSuiteDescription
	{
		internal static readonly FixtureVariable ConfigurationUseFunction = FixtureVariable
			.NewInstance("Successul configuration use");

		internal static readonly FixtureVariable ConfigurationReuseProcedure = FixtureVariable
			.NewInstance("Configuration reuse attempt");

		public class ConfigurationReuseTestUnit : ITestCase
		{
			// each function returns a block that disposes of any containers
			public virtual void Test()
			{
				IConfiguration config = NewInMemoryConfiguration();
				IRunnable tearDownBlock = ((IRunnable)((IFunction4)ConfigurationUseFunction.Value
					).Apply(config));
				try
				{
					Assert.Expect(typeof(ArgumentException), new _ICodeBlock_75(config));
				}
				finally
				{
					tearDownBlock.Run();
				}
			}

			private sealed class _ICodeBlock_75 : ICodeBlock
			{
				public _ICodeBlock_75(IConfiguration config)
				{
					this.config = config;
				}

				/// <exception cref="System.Exception"></exception>
				public void Run()
				{
					((IProcedure4)ConfigurationReuseTestSuite.ConfigurationReuseProcedure.Value).Apply
						(config);
				}

				private readonly IConfiguration config;
			}
		}

		internal static IConfiguration NewInMemoryConfiguration()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.Storage = new MemoryStorage();
			return config;
		}

		public ConfigurationReuseTestSuite()
		{
			{
				FixtureProviders(new IFixtureProvider[] { new SimpleFixtureProvider(ConfigurationUseFunction
					, new IFunction4[] { new _IFunction4_22(), new _IFunction4_27(), new _IFunction4_32
					() }), new SimpleFixtureProvider(ConfigurationReuseProcedure, new IProcedure4[] 
					{ new _IProcedure4_45(), new _IProcedure4_47(), new _IProcedure4_49(), new _IProcedure4_57
					() }) });
				TestUnits(new Type[] { typeof(ConfigurationReuseTestSuite.ConfigurationReuseTestUnit
					) });
			}
		}

		private sealed class _IFunction4_22 : IFunction4
		{
			public _IFunction4_22()
			{
			}

			public object Apply(object config)
			{
				IObjectContainer container = Db4oFactory.OpenFile(((IConfiguration)config), ".");
				return new _IRunnable_24(container);
			}

			private sealed class _IRunnable_24 : IRunnable
			{
				public _IRunnable_24(IObjectContainer container)
				{
					this.container = container;
				}

				public void Run()
				{
					container.Close();
				}

				private readonly IObjectContainer container;
			}
		}

		private sealed class _IFunction4_27 : IFunction4
		{
			public _IFunction4_27()
			{
			}

			public object Apply(object config)
			{
				IObjectServer server = Db4oFactory.OpenServer(((IConfiguration)config), ".", 0);
				return new _IRunnable_29(server);
			}

			private sealed class _IRunnable_29 : IRunnable
			{
				public _IRunnable_29(IObjectServer server)
				{
					this.server = server;
				}

				public void Run()
				{
					server.Close();
				}

				private readonly IObjectServer server;
			}
		}

		private sealed class _IFunction4_32 : IFunction4
		{
			public _IFunction4_32()
			{
			}

			public object Apply(object config)
			{
				IConfiguration serverConfig = Db4oFactory.NewConfiguration();
				serverConfig.Storage = new MemoryStorage();
				IObjectServer server = Db4oFactory.OpenServer(serverConfig, ".", -1);
				server.GrantAccess("user", "password");
				IObjectContainer client = Db4oFactory.OpenClient(((IConfiguration)config), "localhost"
					, server.Ext().Port(), "user", "password");
				return new _IRunnable_38(client, server);
			}

			private sealed class _IRunnable_38 : IRunnable
			{
				public _IRunnable_38(IObjectContainer client, IObjectServer server)
				{
					this.client = client;
					this.server = server;
				}

				public void Run()
				{
					client.Close();
					server.Close();
				}

				private readonly IObjectContainer client;

				private readonly IObjectServer server;
			}
		}

		private sealed class _IProcedure4_45 : IProcedure4
		{
			public _IProcedure4_45()
			{
			}

			public void Apply(object config)
			{
				Db4oFactory.OpenFile(((IConfiguration)config), "..");
			}
		}

		private sealed class _IProcedure4_47 : IProcedure4
		{
			public _IProcedure4_47()
			{
			}

			public void Apply(object config)
			{
				Db4oFactory.OpenServer(((IConfiguration)config), "..", 0);
			}
		}

		private sealed class _IProcedure4_49 : IProcedure4
		{
			public _IProcedure4_49()
			{
			}

			public void Apply(object config)
			{
				IObjectServer server = Db4oFactory.OpenServer(ConfigurationReuseTestSuite.NewInMemoryConfiguration
					(), "..", 0);
				try
				{
					Db4oFactory.OpenClient(((IConfiguration)config), "localhost", server.Ext().Port()
						, "user", "password");
				}
				finally
				{
					server.Close();
				}
			}
		}

		private sealed class _IProcedure4_57 : IProcedure4
		{
			public _IProcedure4_57()
			{
			}

			public void Apply(object config)
			{
				Db4oFactory.OpenClient(((IConfiguration)config), "localhost", unchecked((int)(0xdb40
					)), "user", "password");
			}
		}
	}
}
