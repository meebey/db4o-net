/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class InvalidPasswordTestCase : Db4oClientServerTestCase, IOptOutAllButNetworkingCS
	{
		public static void Main(string[] args)
		{
			new InvalidPasswordTestCase().RunAll();
		}

		public virtual void TestInvalidPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_21(port));
		}

		private sealed class _ICodeBlock_21 : ICodeBlock
		{
			public _ICodeBlock_21(int port)
			{
				this.port = port;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, "strangeusername", "invalidPassword");
			}

			private readonly int port;
		}

		public virtual void TestEmptyUserPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_31(port));
		}

		private sealed class _ICodeBlock_31 : ICodeBlock
		{
			public _ICodeBlock_31(int port)
			{
				this.port = port;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, string.Empty, string.Empty);
			}

			private readonly int port;
		}

		public virtual void TestEmptyUserNullPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_40(port));
		}

		private sealed class _ICodeBlock_40 : ICodeBlock
		{
			public _ICodeBlock_40(int port)
			{
				this.port = port;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, string.Empty, null);
			}

			private readonly int port;
		}

		public virtual void TestNullPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_49(port));
		}

		private sealed class _ICodeBlock_49 : ICodeBlock
		{
			public _ICodeBlock_49(int port)
			{
				this.port = port;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, null, null);
			}

			private readonly int port;
		}
	}
}
