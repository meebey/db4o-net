/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Collections;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Collections
{
	/// <exclude></exclude>
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public partial class BigSet<E> : ISet<E>, IBigSetPersistence
	{
		private Db4objects.Db4o.Internal.Btree.BTree _bTree;

		private Db4objects.Db4o.Internal.Transaction _transaction;

		public BigSet(LocalObjectContainer db)
		{
			if (db == null)
			{
				return;
			}
			_transaction = db.Transaction();
			_bTree = NewBTree(0);
		}

		private Db4objects.Db4o.Internal.Btree.BTree NewBTree(int id)
		{
			return new Db4objects.Db4o.Internal.Btree.BTree(SystemTransaction(), id, new IntHandler
				());
		}

		private ObjectContainerBase Container()
		{
			return Transaction().Container();
		}

		public virtual bool Add(E obj)
		{
			int id = GetID(obj);
			if (id == 0)
			{
				Add(Store(obj));
				return true;
			}
			if (Contains(id))
			{
				return false;
			}
			Add(id);
			return true;
		}

		private int Store(E obj)
		{
			return Container().Store(_transaction, obj, Const4.Unspecified);
		}

		private void Add(int id)
		{
			BTree().Add(_transaction, id);
		}

		private int GetID(object obj)
		{
			return (int)Container().GetID(obj);
		}

		public virtual bool AddAll(IEnumerable<E> iterable)
		{
			bool result = false;
			foreach (E element in iterable)
			{
				if (Add(element))
				{
					result = true;
				}
			}
			return result;
		}

		public virtual void Clear()
		{
			BTree().Clear(Transaction());
		}

		public virtual bool Contains(object obj)
		{
			int id = GetID(obj);
			if (id == 0)
			{
				return false;
			}
			return Contains(id);
		}

		private bool Contains(int id)
		{
			IBTreeRange range = BTree().Search(Transaction(), id);
			return !range.IsEmpty();
		}

		public virtual bool IsEmpty
		{
			get
			{
				return Count == 0;
			}
		}

		private IEnumerator BTreeIterator()
		{
			return BTree().Iterator(Transaction());
		}

		public virtual bool Remove(object obj)
		{
			if (!Contains(obj))
			{
				return false;
			}
			int id = GetID(obj);
			BTree().Remove(Transaction(), id);
			return true;
		}

		public virtual int Count
		{
			get
			{
				return BTree().Size(Transaction());
			}
		}

		public virtual object[] ToArray()
		{
			throw new NotSupportedException();
		}

		public virtual T[] ToArray<T>(T[] a)
		{
			throw new NotSupportedException();
		}

		public virtual void Write(IWriteContext context)
		{
			int id = BTree().GetID();
			if (id == 0)
			{
				BTree().Write(Container().SystemTransaction());
			}
			context.WriteInt(BTree().GetID());
		}

		public virtual void Read(IReadContext context)
		{
			int id = context.ReadInt();
			if (_transaction == null)
			{
				_transaction = context.Transaction();
			}
			if (_bTree == null)
			{
				_bTree = NewBTree(id);
			}
		}

		private Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		private Db4objects.Db4o.Internal.Transaction SystemTransaction()
		{
			return Container().SystemTransaction();
		}

		public virtual void Invalidate()
		{
			_bTree = null;
		}

		private Db4objects.Db4o.Internal.Btree.BTree BTree()
		{
			if (_bTree == null)
			{
				throw new InvalidOperationException();
			}
			return _bTree;
		}

		private object Element(int id)
		{
			object obj = Container().GetByID(Transaction(), id);
			Container().Activate(obj);
			return obj;
		}
	}
}
