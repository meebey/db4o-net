/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o
{
	[Obsolete("since 7.0")]
	internal class P2Collections : IDb4oCollections
	{
		internal Transaction _transaction;

		internal P2Collections(Transaction transaction)
			: base()
		{
			_transaction = transaction;
		}

		public IDb4oList NewLinkedList()
		{
			lock (Lock())
			{
				if (CanCreateCollection(Container()))
				{
					IDb4oList l = new P2LinkedList();
					Container().Store(_transaction, l);
					return l;
				}
				return null;
			}
		}

		public IDb4oMap NewHashMap(int size)
		{
			lock (Lock())
			{
				if (CanCreateCollection(Container()))
				{
					return new P2HashMap(size);
				}
				return null;
			}
		}

		public IDb4oMap NewIdentityHashMap(int size)
		{
			lock (Lock())
			{
				if (CanCreateCollection(Container()))
				{
					P2HashMap m = new P2HashMap(size);
					m.i_type = 1;
					Container().Store(_transaction, m);
					return m;
				}
				return null;
			}
		}

		private Object Lock()
		{
			return Container().Lock();
		}

		private ObjectContainerBase Container()
		{
			return _transaction.Container();
		}

		private bool CanCreateCollection(ObjectContainerBase container)
		{
			container.CheckClosed();
			return !container.IsInstantiating();
		}
	}
}