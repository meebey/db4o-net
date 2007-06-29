/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class InvalidPasswordTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new InvalidPasswordTestCase().RunClientServer();
		}

		public virtual void TestInvalidPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_17(this, port));
		}

		private sealed class _ICodeBlock_17 : ICodeBlock
		{
			public _ICodeBlock_17(InvalidPasswordTestCase _enclosing, int port)
			{
				this._enclosing = _enclosing;
				this.port = port;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, "strangeusername", "invalidPassword");
			}

			private readonly InvalidPasswordTestCase _enclosing;

			private readonly int port;
		}

		public virtual void TestEmptyUserPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_27(this, port));
		}

		private sealed class _ICodeBlock_27 : ICodeBlock
		{
			public _ICodeBlock_27(InvalidPasswordTestCase _enclosing, int port)
			{
				this._enclosing = _enclosing;
				this.port = port;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, string.Empty, string.Empty);
			}

			private readonly InvalidPasswordTestCase _enclosing;

			private readonly int port;
		}

		public virtual void TestEmptyPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_36(this, port));
		}

		private sealed class _ICodeBlock_36 : ICodeBlock
		{
			public _ICodeBlock_36(InvalidPasswordTestCase _enclosing, int port)
			{
				this._enclosing = _enclosing;
				this.port = port;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, string.Empty, null);
			}

			private readonly InvalidPasswordTestCase _enclosing;

			private readonly int port;
		}

		public virtual void TestNullPassword()
		{
			int port = ClientServerFixture().ServerPort();
			Assert.Expect(typeof(InvalidPasswordException), new _ICodeBlock_45(this, port));
		}

		private sealed class _ICodeBlock_45 : ICodeBlock
		{
			public _ICodeBlock_45(InvalidPasswordTestCase _enclosing, int port)
			{
				this._enclosing = _enclosing;
				this.port = port;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", port, null, null);
			}

			private readonly InvalidPasswordTestCase _enclosing;

			private readonly int port;
		}
	}
}
