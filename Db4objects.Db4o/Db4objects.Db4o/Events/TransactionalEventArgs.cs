/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Events
{
	public class TransactionalEventArgs : EventArgs
	{
		private readonly Db4objects.Db4o.Internal.Transaction _transaction;

		public TransactionalEventArgs(Db4objects.Db4o.Internal.Transaction transaction)
		{
			_transaction = transaction;
		}

		public virtual object Transaction()
		{
			return _transaction;
		}
	}
}
