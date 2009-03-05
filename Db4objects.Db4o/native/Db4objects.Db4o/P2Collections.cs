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
			return Container().SyncExec<IDb4oList>(delegate
			{
				IDb4oList l = new P2LinkedList();
				Container().Store(_transaction, l);
				return l;
			});
		}

		public IDb4oMap NewHashMap(int size)
		{
			return Container().SyncExec<IDb4oMap>(delegate
			{
				return new P2HashMap(size);
			});
		}

		public IDb4oMap NewIdentityHashMap(int size)
		{
			return Container().SyncExec<IDb4oMap>(delegate
			{
				P2HashMap m = new P2HashMap(size);
				m.i_type = 1;
				Container().Store(_transaction, m);
				return m;
			});
		}

		private ObjectContainerBase Container()
		{
			return _transaction.Container();
		}
	}
}