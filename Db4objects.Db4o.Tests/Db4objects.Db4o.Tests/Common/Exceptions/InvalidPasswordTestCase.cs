using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class InvalidPasswordTestCase : ITestCase, ITestLifeCycle
	{
		private static readonly string DB_FILE = "server.db4o";

		private const int PORT = unchecked((int)(0xdb40));

		private static readonly string USER = "db4o";

		private static readonly string PASSWORD = "db4o";

		private IObjectServer _server;

		public virtual void SetUp()
		{
			new Sharpen.IO.File(DB_FILE).Delete();
			_server = Db4oFactory.OpenServer(DB_FILE, PORT);
			_server.GrantAccess(USER, PASSWORD);
		}

		public virtual void TearDown()
		{
			_server.Close();
		}

		public virtual void TestInvalidPassword()
		{
			Assert.Expect(typeof(InvalidPasswordException), new _AnonymousInnerClass34(this));
		}

		private sealed class _AnonymousInnerClass34 : ICodeBlock
		{
			public _AnonymousInnerClass34(InvalidPasswordTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", InvalidPasswordTestCase.PORT, "hello", "invalid"
					);
			}

			private readonly InvalidPasswordTestCase _enclosing;
		}

		public virtual void TestEmptyPassword()
		{
			Assert.Expect(typeof(InvalidPasswordException), new _AnonymousInnerClass43(this));
			Assert.Expect(typeof(InvalidPasswordException), new _AnonymousInnerClass49(this));
			Assert.Expect(typeof(InvalidPasswordException), new _AnonymousInnerClass55(this));
		}

		private sealed class _AnonymousInnerClass43 : ICodeBlock
		{
			public _AnonymousInnerClass43(InvalidPasswordTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", InvalidPasswordTestCase.PORT, string.Empty, string.Empty
					);
			}

			private readonly InvalidPasswordTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass49 : ICodeBlock
		{
			public _AnonymousInnerClass49(InvalidPasswordTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", InvalidPasswordTestCase.PORT, string.Empty, null
					);
			}

			private readonly InvalidPasswordTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass55 : ICodeBlock
		{
			public _AnonymousInnerClass55(InvalidPasswordTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oFactory.OpenClient("127.0.0.1", InvalidPasswordTestCase.PORT, null, null);
			}

			private readonly InvalidPasswordTestCase _enclosing;
		}
	}
}
