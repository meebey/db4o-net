/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.Exceptions.Propagation.CS
{
	public class MsgExceptionHandlingTestCase : ClientServerTestCaseBase, IOptOutAllButNetworkingCS
	{
		private static readonly string ExceptionMessage = "exc";

		public virtual void TestRecoverableExceptionWithResponse()
		{
			Client().Write(Msg.RequestExceptionWithResponse.GetWriterForSingleObject(Trans(), 
				new Db4oRecoverableException(ExceptionMessage)));
			try
			{
				Client().ExpectedResponse(Msg.Ok);
				Assert.Fail();
			}
			catch (Db4oRecoverableException exc)
			{
				AssertExceptionMessage(exc);
			}
			Assert.IsTrue(Client().IsAlive());
			AssertServerContainerStateClosed(false);
		}

		public virtual void TestNonRecoverableExceptionWithResponse()
		{
			Client().Write(Msg.RequestExceptionWithResponse.GetWriterForSingleObject(Trans(), 
				new Db4oException(ExceptionMessage)));
			AssertDatabaseClosedException();
			AssertServerContainerStateClosed(true);
		}

		public virtual void TestRecoverableExceptionWithoutResponse()
		{
			Client().Write(Msg.RequestExceptionWithoutResponse.GetWriterForSingleObject(Trans
				(), new Db4oRecoverableException(ExceptionMessage)));
			AssertDatabaseClosedException();
			AssertServerContainerStateClosed(false);
		}

		public virtual void TestNonRecoverableExceptionWithoutResponse()
		{
			Client().Write(Msg.RequestExceptionWithoutResponse.GetWriterForSingleObject(Trans
				(), new Db4oException(ExceptionMessage)));
			AssertDatabaseClosedException();
			AssertServerContainerStateClosed(true);
		}

		private void AssertDatabaseClosedException()
		{
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_50(this));
			Assert.IsFalse(Client().IsAlive());
		}

		private sealed class _ICodeBlock_50 : ICodeBlock
		{
			public _ICodeBlock_50(MsgExceptionHandlingTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				this._enclosing.Client().ExpectedResponse(Msg.Ok);
			}

			private readonly MsgExceptionHandlingTestCase _enclosing;
		}

		private void AssertExceptionMessage(Db4oRecoverableException exc)
		{
			Assert.AreEqual(ExceptionMessage, exc.Message);
		}

		private void AssertServerContainerStateClosed(bool expectedClosed)
		{
		}
		//		Assert.areEqual(expectedClosed, server().objectContainer().ext().isClosed());
		//		ExtObjectContainer otherClient = openNewClient();
		//		otherClient.close();
	}
}
#endif // !SILVERLIGHT
