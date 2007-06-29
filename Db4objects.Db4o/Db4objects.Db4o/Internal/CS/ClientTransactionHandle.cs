/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS
{
	public class ClientTransactionHandle
	{
		private readonly ClientTransactionPool _transactionPool;

		private Db4objects.Db4o.Internal.Transaction _mainTransaction;

		private LocalObjectContainer _stream;

		private Db4objects.Db4o.Internal.Transaction _transaction;

		private bool _rollbackOnClose;

		public ClientTransactionHandle(ClientTransactionPool transactionPool)
		{
			_transactionPool = transactionPool;
			_mainTransaction = _transactionPool.AcquireMain();
			_rollbackOnClose = false;
		}

		public virtual void AcquireTransactionForFile(string fileName)
		{
			_transaction = _transactionPool.Acquire(fileName);
		}

		public virtual void ReleaseTransaction()
		{
			if (_transaction != null)
			{
				_transactionPool.Release(_transaction, _rollbackOnClose);
				_transaction = null;
			}
		}

		public virtual void Write(Msg message, ISocket4 socket)
		{
			if (_stream != null)
			{
				message.Write(_stream, socket);
			}
			else
			{
				_transactionPool.Write(message, socket);
			}
		}

		public virtual bool IsClosed()
		{
			return _transactionPool.IsClosed();
		}

		public virtual void Close()
		{
			if ((!_transactionPool.IsClosed()) && (_mainTransaction != null))
			{
				_mainTransaction.Close(_rollbackOnClose);
			}
		}

		public virtual object Lock()
		{
			return _transactionPool.StreamLock();
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			if (_transaction != null)
			{
				return _transaction;
			}
			return _mainTransaction;
		}

		public virtual void Transaction(Db4objects.Db4o.Internal.Transaction transaction)
		{
			if (_transaction != null)
			{
				_transaction = transaction;
			}
			else
			{
				_mainTransaction = transaction;
			}
			_rollbackOnClose = false;
		}
	}
}
