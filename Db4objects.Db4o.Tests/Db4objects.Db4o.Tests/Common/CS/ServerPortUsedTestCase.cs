/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ServerPortUsedTestCase : Db4oClientServerTestCase
	{
		private static readonly string DatabaseFile = "PortUsed.db";

		public static void Main(string[] args)
		{
			new ServerPortUsedTestCase().RunAll();
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Db4oTearDownBeforeClean()
		{
			File4.Delete(DatabaseFile);
		}

		public virtual void Test()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_27(port));
		}

		private sealed class _ICodeBlock_27 : ICodeBlock
		{
			public _ICodeBlock_27(int port)
			{
				this.port = port;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenServer(Db4oFactory.NewConfiguration(), ServerPortUsedTestCase.DatabaseFile
					, port);
			}

			private readonly int port;
		}
	}
}
