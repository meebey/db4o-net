/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS
{
	public class ClientTransactionPool
	{
		private Hashtable4 _transaction2Container;

		private Hashtable4 _fileName2Container;

		private readonly LocalObjectContainer _mainContainer;

		private readonly object _lock;

		public ClientTransactionPool(LocalObjectContainer mainContainer, object Lock)
		{
			ClientTransactionPool.ContainerCount mainEntry = new ClientTransactionPool.ContainerCount
				(mainContainer, 1);
			_transaction2Container = new Hashtable4();
			_fileName2Container = new Hashtable4();
			_fileName2Container.Put(mainContainer.FileName(), mainEntry);
			_mainContainer = mainContainer;
			_lock = Lock;
		}

		public virtual Transaction AcquireMain()
		{
			return Acquire(_mainContainer.FileName());
		}

		public virtual Transaction Acquire(string fileName)
		{
			lock (_lock)
			{
				ClientTransactionPool.ContainerCount entry = (ClientTransactionPool.ContainerCount
					)_fileName2Container.Get(fileName);
				if (entry == null)
				{
					LocalObjectContainer container = (LocalObjectContainer)Db4oFactory.OpenFile(fileName
						);
					container.ConfigImpl().SetMessageRecipient(_mainContainer.ConfigImpl().MessageRecipient
						());
					entry = new ClientTransactionPool.ContainerCount(container);
					_fileName2Container.Put(fileName, entry);
				}
				Transaction transaction = entry.NewTransaction();
				_transaction2Container.Put(transaction, entry);
				return transaction;
			}
		}

		public virtual void Release(Transaction transaction, bool rollbackOnClose)
		{
			transaction.Close(rollbackOnClose);
			lock (_lock)
			{
				ClientTransactionPool.ContainerCount entry = (ClientTransactionPool.ContainerCount
					)_transaction2Container.Get(transaction);
				_transaction2Container.Remove(transaction);
				entry.Release();
				if (entry.IsEmpty())
				{
					_fileName2Container.Remove(entry.FileName());
					entry.Close();
				}
			}
		}

		public virtual void Close()
		{
			lock (_lock)
			{
				IEnumerator entryIter = _fileName2Container.Iterator();
				while (entryIter.MoveNext())
				{
					HashtableObjectEntry hashEntry = (HashtableObjectEntry)entryIter.Current;
					((ClientTransactionPool.ContainerCount)hashEntry.Value()).Close();
				}
				_transaction2Container = null;
				_fileName2Container = null;
			}
		}

		public virtual int OpenFileCount()
		{
			return (_fileName2Container == null ? 0 : _fileName2Container.Size());
		}

		public virtual bool IsClosed()
		{
			return _mainContainer == null || _mainContainer.IsClosed();
		}

		public virtual object StreamLock()
		{
			return _mainContainer.Lock();
		}

		public virtual void Write(Msg message, ISocket4 socket)
		{
			message.Write(_mainContainer, socket);
		}

		private class ContainerCount
		{
			private LocalObjectContainer _container;

			private int _count;

			public ContainerCount(LocalObjectContainer container) : this(container, 0)
			{
			}

			public ContainerCount(LocalObjectContainer container, int count)
			{
				_container = container;
				_count = count;
			}

			public virtual bool IsEmpty()
			{
				return _count <= 0;
			}

			public virtual Transaction NewTransaction()
			{
				_count++;
				return _container.NewTransaction();
			}

			public virtual void Release()
			{
				if (_count == 0)
				{
					throw new InvalidOperationException();
				}
				_count--;
			}

			public virtual string FileName()
			{
				return _container.FileName();
			}

			public virtual void Close()
			{
				_container.Close();
				_container = null;
			}

			public override int GetHashCode()
			{
				return FileName().GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				ClientTransactionPool.ContainerCount other = (ClientTransactionPool.ContainerCount
					)obj;
				return FileName().Equals(other.FileName());
			}
		}
	}
}
