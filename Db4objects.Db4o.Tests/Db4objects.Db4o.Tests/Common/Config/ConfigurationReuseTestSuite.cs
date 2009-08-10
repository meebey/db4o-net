/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

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
					Assert.Expect(typeof(ArgumentException), new _ICodeBlock_74(config));
				}
				finally
				{
					tearDownBlock.Run();
				}
			}

			private sealed class _ICodeBlock_74 : ICodeBlock
			{
				public _ICodeBlock_74(IConfiguration config)
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
					, new object[] { new _IFunction4_21(), new _IFunction4_26(), new _IFunction4_31(
					) }), new SimpleFixtureProvider(ConfigurationReuseProcedure, new object[] { new 
					_IProcedure4_44(), new _IProcedure4_46(), new _IProcedure4_48(), new _IProcedure4_56
					() }) });
				TestUnits(new Type[] { typeof(ConfigurationReuseTestSuite.ConfigurationReuseTestUnit
					) });
			}
		}

		private sealed class _IFunction4_21 : IFunction4
		{
			public _IFunction4_21()
			{
			}

			public object Apply(object config)
			{
				IObjectContainer container = Db4oFactory.OpenFile(((IConfiguration)config), ".");
				return new _IRunnable_23(container);
			}

			private sealed class _IRunnable_23 : IRunnable
			{
				public _IRunnable_23(IObjectContainer container)
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

		private sealed class _IFunction4_26 : IFunction4
		{
			public _IFunction4_26()
			{
			}

			public object Apply(object config)
			{
				IObjectServer server = Db4oFactory.OpenServer(((IConfiguration)config), ".", 0);
				return new _IRunnable_28(server);
			}

			private sealed class _IRunnable_28 : IRunnable
			{
				public _IRunnable_28(IObjectServer server)
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

		private sealed class _IFunction4_31 : IFunction4
		{
			public _IFunction4_31()
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
				return new _IRunnable_37(client, server);
			}

			private sealed class _IRunnable_37 : IRunnable
			{
				public _IRunnable_37(IObjectContainer client, IObjectServer server)
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

		private sealed class _IProcedure4_44 : IProcedure4
		{
			public _IProcedure4_44()
			{
			}

			public void Apply(object config)
			{
				Db4oFactory.OpenFile(((IConfiguration)config), "..");
			}
		}

		private sealed class _IProcedure4_46 : IProcedure4
		{
			public _IProcedure4_46()
			{
			}

			public void Apply(object config)
			{
				Db4oFactory.OpenServer(((IConfiguration)config), "..", 0);
			}
		}

		private sealed class _IProcedure4_48 : IProcedure4
		{
			public _IProcedure4_48()
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

		private sealed class _IProcedure4_56 : IProcedure4
		{
			public _IProcedure4_56()
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
