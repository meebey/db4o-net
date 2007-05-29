/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class CallConstructorsConfigTestCase : ITestCase
	{
		public sealed class Item
		{
		}

		public virtual void Test()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.CallConstructors(true);
			config.ExceptionsOnNotStorable(true);
			IObjectServer server = Db4oFactory.OpenServer(config, DatabaseFile(), 1022);
			try
			{
				server.GrantAccess("db4o", "db4o");
				WithClient(new _IClientBlock_26(this));
				WithClient(new _IClientBlock_32(this));
			}
			finally
			{
				server.Close();
				File4.Delete(DatabaseFile());
			}
		}

		private sealed class _IClientBlock_26 : CallConstructorsConfigTestCase.IClientBlock
		{
			public _IClientBlock_26(CallConstructorsConfigTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run(IObjectContainer client)
			{
				client.Set(new CallConstructorsConfigTestCase.Item());
			}

			private readonly CallConstructorsConfigTestCase _enclosing;
		}

		private sealed class _IClientBlock_32 : CallConstructorsConfigTestCase.IClientBlock
		{
			public _IClientBlock_32(CallConstructorsConfigTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run(IObjectContainer client)
			{
				Assert.AreEqual(1, client.Query(typeof(CallConstructorsConfigTestCase.Item)).Size
					());
			}

			private readonly CallConstructorsConfigTestCase _enclosing;
		}

		private string DatabaseFile()
		{
			return Path.Combine(Path.GetTempPath(), "cc.db4o");
		}

		public interface IClientBlock
		{
			void Run(IObjectContainer client);
		}

		private void WithClient(CallConstructorsConfigTestCase.IClientBlock block)
		{
			IObjectContainer client = Db4oFactory.OpenClient("localhost", 1022, "db4o", "db4o"
				);
			try
			{
				block.Run(client);
			}
			finally
			{
				client.Close();
			}
		}
	}
}
