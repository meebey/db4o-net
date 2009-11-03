/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class WithTransactionTestCase : AbstractDb4oTestCase
	{
		public virtual void Test()
		{
			Transaction originalTransaction = Container().Transaction;
			Transaction transaction = Container().NewUserTransaction();
			Container().WithTransaction(transaction, new _IRunnable_15(this, transaction));
			Assert.AreSame(originalTransaction, Container().Transaction);
		}

		private sealed class _IRunnable_15 : IRunnable
		{
			public _IRunnable_15(WithTransactionTestCase _enclosing, Transaction transaction)
			{
				this._enclosing = _enclosing;
				this.transaction = transaction;
			}

			public void Run()
			{
				Assert.AreSame(transaction, this._enclosing.Container().Transaction);
			}

			private readonly WithTransactionTestCase _enclosing;

			private readonly Transaction transaction;
		}
	}
}
