/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public abstract class AbstractQueryResult : IQueryResult
	{
		protected readonly Db4objects.Db4o.Internal.Transaction _transaction;

		public AbstractQueryResult(Db4objects.Db4o.Internal.Transaction transaction)
		{
			_transaction = transaction;
		}

		public object Activate(object obj)
		{
			Stream().Activate(_transaction, obj);
			return obj;
		}

		public object ActivatedObject(int id)
		{
			ObjectContainerBase stream = Stream();
			object ret = stream.GetActivatedObjectFromCache(_transaction, id);
			if (ret != null)
			{
				return ret;
			}
			return stream.ReadActivatedObjectNotInCache(_transaction, id);
		}

		public virtual object Lock()
		{
			ObjectContainerBase stream = Stream();
			stream.CheckClosed();
			return stream.Lock();
		}

		public virtual ObjectContainerBase Stream()
		{
			return _transaction.Container();
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual IExtObjectContainer ObjectContainer()
		{
			return Transaction().ObjectContainer().Ext();
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new _MappingIterator_56(this, IterateIDs());
		}

		private sealed class _MappingIterator_56 : MappingIterator
		{
			public _MappingIterator_56(AbstractQueryResult _enclosing, IIntIterator4 baseArg1
				) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				if (current == null)
				{
					return MappingIterator.Skip;
				}
				lock (this._enclosing.Lock())
				{
					object obj = this._enclosing.ActivatedObject(((int)current));
					if (obj == null)
					{
						return MappingIterator.Skip;
					}
					return obj;
				}
			}

			private readonly AbstractQueryResult _enclosing;
		}

		public virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportSize
			()
		{
			return this;
		}

		public virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportSort
			()
		{
			return this;
		}

		public virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportElementAccess
			()
		{
			return this;
		}

		protected virtual int KnownSize()
		{
			return Size();
		}

		public virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult ToIdList
			()
		{
			IdListQueryResult res = new IdListQueryResult(Transaction(), KnownSize());
			IIntIterator4 i = IterateIDs();
			while (i.MoveNext())
			{
				res.Add(i.CurrentInt());
			}
			return res;
		}

		protected virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult ToIdTree
			()
		{
			return new IdTreeQueryResult(Transaction(), IterateIDs());
		}

		public virtual Config4Impl Config()
		{
			return Stream().Config();
		}

		public virtual int Size()
		{
			throw new NotImplementedException();
		}

		public virtual void Sort(IQueryComparator cmp)
		{
			throw new NotImplementedException();
		}

		public virtual object Get(int index)
		{
			throw new NotImplementedException();
		}

		/// <param name="i"></param>
		public virtual int GetId(int i)
		{
			throw new NotImplementedException();
		}

		public virtual int IndexOf(int id)
		{
			throw new NotImplementedException();
		}

		/// <param name="c"></param>
		public virtual void LoadFromClassIndex(ClassMetadata c)
		{
			throw new NotImplementedException();
		}

		/// <param name="i"></param>
		public virtual void LoadFromClassIndexes(ClassMetadataIterator i)
		{
			throw new NotImplementedException();
		}

		/// <param name="r"></param>
		public virtual void LoadFromIdReader(ByteArrayBuffer r)
		{
			throw new NotImplementedException();
		}

		/// <param name="q"></param>
		public virtual void LoadFromQuery(QQuery q)
		{
			throw new NotImplementedException();
		}

		public abstract IIntIterator4 IterateIDs();
	}
}
