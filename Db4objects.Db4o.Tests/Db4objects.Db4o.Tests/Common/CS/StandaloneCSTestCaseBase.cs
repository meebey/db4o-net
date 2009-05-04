/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public abstract class StandaloneCSTestCaseBase : ITestCase
	{
		private int _port;

		public sealed class Item
		{
			// TODO fix db4ounit call logic - this should actually be run in C/S mode
		}

		/// <exception cref="System.Exception">System.Exception</exception>
		[System.ObsoleteAttribute(@"using deprecated api")]
		public virtual void Test()
		{
			string fileName = DatabaseFile();
			File4.Delete(fileName);
			IObjectServer server = Db4oFactory.OpenServer(CreateConfiguration(), fileName, -1
				);
			_port = server.Ext().Port();
			try
			{
				server.GrantAccess("db4o", "db4o");
				RunTest();
			}
			finally
			{
				server.Close();
				File4.Delete(fileName);
			}
		}

		private IConfiguration CreateConfiguration()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			Configure(config);
			return config;
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void WithClient(IContainerBlock block)
		{
			ContainerServices.WithContainer(OpenClient(), block);
		}

		protected virtual ClientObjectContainer OpenClient()
		{
			return (ClientObjectContainer)Db4oFactory.OpenClient(CreateConfiguration(), "localhost"
				, _port, "db4o", "db4o");
		}

		protected virtual int Port()
		{
			return _port;
		}

		/// <exception cref="System.Exception"></exception>
		protected abstract void RunTest();

		protected abstract void Configure(IConfiguration config);

		private string DatabaseFile()
		{
			return Path.Combine(Path.GetTempPath(), "cc.db4o");
		}
	}
}
