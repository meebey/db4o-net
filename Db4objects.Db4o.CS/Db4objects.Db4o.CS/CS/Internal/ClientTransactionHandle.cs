/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal
{
	public class ClientTransactionHandle
	{
		private readonly ClientTransactionPool _transactionPool;

		private Db4objects.Db4o.Internal.Transaction _mainTransaction;

		private Db4objects.Db4o.Internal.Transaction _transaction;

		private bool _rollbackOnClose;

		private Tree _prefetchedIDs;

		public ClientTransactionHandle(ClientTransactionPool transactionPool)
		{
			_transactionPool = transactionPool;
			_mainTransaction = _transactionPool.AcquireMain();
			_rollbackOnClose = true;
		}

		public virtual void AcquireTransactionForFile(string fileName)
		{
			CleanUpCurrentTransaction();
			_transaction = _transactionPool.Acquire(fileName);
		}

		public virtual void ReleaseTransaction(ShutdownMode mode)
		{
			if (_transaction != null)
			{
				CleanUpCurrentTransaction();
				_transactionPool.Release(mode, _transaction, _rollbackOnClose);
				_transaction = null;
			}
		}

		public virtual bool IsClosed()
		{
			return _transactionPool.IsClosed();
		}

		public virtual void Close(ShutdownMode mode)
		{
			if ((!_transactionPool.IsClosed()) && (_mainTransaction != null))
			{
				_transactionPool.Release(mode, _mainTransaction, _rollbackOnClose);
			}
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
			CleanUpCurrentTransaction();
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

		private void CleanUpCurrentTransaction()
		{
			FreePrefetchedIDs();
		}

		public virtual int PrefetchID()
		{
			int id = Container().GetPointerSlot();
			_prefetchedIDs = Tree.Add(_prefetchedIDs, new TreeInt(id));
			return id;
		}

		private LocalObjectContainer Container()
		{
			return ((LocalObjectContainer)Transaction().Container());
		}

		public virtual void PrefetchedIDConsumed(int id)
		{
			_prefetchedIDs = _prefetchedIDs.RemoveLike(new TreeIntObject(id));
		}

		internal void FreePrefetchedIDs()
		{
			if (_prefetchedIDs == null)
			{
				return;
			}
			LocalObjectContainer container = Container();
			_prefetchedIDs.Traverse(new _IVisitor4_88(container));
			_prefetchedIDs = null;
		}

		private sealed class _IVisitor4_88 : IVisitor4
		{
			public _IVisitor4_88(LocalObjectContainer container)
			{
				this.container = container;
			}

			public void Visit(object node)
			{
				TreeInt intNode = (TreeInt)node;
				container.Free(intNode._key, Const4.PointerLength);
			}

			private readonly LocalObjectContainer container;
		}
	}
}
