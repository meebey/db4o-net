/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Config;

namespace Db4objects.Db4o.Tests.Common.Config
{
	public class ConfigurationItemTestCase : ITestCase
	{
		internal sealed class ConfigurationItemStub : IConfigurationItem
		{
			private IInternalObjectContainer _container;

			private IConfiguration _configuration;

			public void Apply(IInternalObjectContainer container)
			{
				Assert.IsNotNull(container);
				_container = container;
			}

			public void Prepare(IConfiguration configuration)
			{
				Assert.IsNotNull(configuration);
				_configuration = configuration;
			}

			public IConfiguration PreparedConfiguration()
			{
				return _configuration;
			}

			public IInternalObjectContainer AppliedContainer()
			{
				return _container;
			}
		}

		public virtual void Test()
		{
			IConfiguration configuration = Db4oFactory.NewConfiguration();
			ConfigurationItemTestCase.ConfigurationItemStub item = new ConfigurationItemTestCase.ConfigurationItemStub
				();
			configuration.Add(item);
			Assert.AreSame(configuration, item.PreparedConfiguration());
			Assert.IsNull(item.AppliedContainer());
			File4.Delete(DatabaseFile());
			IObjectContainer container = Db4oFactory.OpenFile(configuration, DatabaseFile());
			container.Close();
			Assert.AreSame(container, item.AppliedContainer());
		}

		private string DatabaseFile()
		{
			return Path.Combine(Path.GetTempPath(), GetType().FullName);
		}
	}
}
