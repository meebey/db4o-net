/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class IsAliveTestCase : ITestLifeCycle
	{
		private static readonly string Username = "db4o";

		private static readonly string Password = "db4o";

		private string filePath;

		public virtual void TestIsAlive()
		{
			IObjectServer server = OpenServer();
			int port = server.Ext().Port();
			ClientObjectContainer client = OpenClient(port);
			Assert.IsTrue(client.IsAlive());
			client.Close();
			server.Close();
		}

		public virtual void TestIsNotAlive()
		{
			IObjectServer server = OpenServer();
			int port = server.Ext().Port();
			ClientObjectContainer client = OpenClient(port);
			server.Close();
			Assert.IsFalse(client.IsAlive());
			client.Close();
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			filePath = Path.GetTempFileName();
			File4.Delete(filePath);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			File4.Delete(filePath);
		}

		private IConfiguration Config()
		{
			return Db4oFactory.NewConfiguration();
		}

		private IObjectServer OpenServer()
		{
			IObjectServer server = Db4oFactory.OpenServer(Config(), filePath, -1);
			server.GrantAccess(Username, Password);
			return server;
		}

		private ClientObjectContainer OpenClient(int port)
		{
			ClientObjectContainer client = (ClientObjectContainer)Db4oFactory.OpenClient(Config
				(), "localhost", port, Username, Password);
			return client;
		}
	}
}
