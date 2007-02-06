namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public abstract class AbstractQueryResult : Db4objects.Db4o.Internal.Query.Result.IQueryResult
	{
		protected readonly Db4objects.Db4o.Internal.Transaction _transaction;

		public AbstractQueryResult(Db4objects.Db4o.Internal.Transaction transaction)
		{
			_transaction = transaction;
		}

		public object Activate(object obj)
		{
			Stream().Activate1(_transaction, obj, Config().ActivationDepth());
			return obj;
		}

		public object ActivatedObject(int id)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			object ret = stream.GetActivatedObjectFromCache(_transaction, id);
			if (ret != null)
			{
				return ret;
			}
			return stream.ReadActivatedObjectNotInCache(_transaction, id);
		}

		public virtual object StreamLock()
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			stream.CheckClosed();
			return stream.Lock();
		}

		public virtual Db4objects.Db4o.Internal.ObjectContainerBase Stream()
		{
			return _transaction.Stream();
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer()
		{
			return Stream();
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return new _AnonymousInnerClass56(this, IterateIDs());
		}

		private sealed class _AnonymousInnerClass56 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass56(AbstractQueryResult _enclosing, Db4objects.Db4o.Foundation.IIntIterator4
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				if (current == null)
				{
					return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
				}
				lock (this._enclosing.StreamLock())
				{
					object obj = this._enclosing.ActivatedObject(((int)current));
					if (obj == null)
					{
						return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
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
			Db4objects.Db4o.Internal.Query.Result.IdListQueryResult res = new Db4objects.Db4o.Internal.Query.Result.IdListQueryResult
				(Transaction(), KnownSize());
			Db4objects.Db4o.Foundation.IIntIterator4 i = IterateIDs();
			while (i.MoveNext())
			{
				res.Add(i.CurrentInt());
			}
			return res;
		}

		protected virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult ToIdTree
			()
		{
			return new Db4objects.Db4o.Internal.Query.Result.IdTreeQueryResult(Transaction(), 
				IterateIDs());
		}

		public virtual Db4objects.Db4o.Internal.Config4Impl Config()
		{
			return Stream().Config();
		}

		public virtual int Size()
		{
			throw new System.NotImplementedException();
		}

		public virtual void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			throw new System.NotImplementedException();
		}

		public virtual object Get(int index)
		{
			throw new System.NotImplementedException();
		}

		public virtual int GetId(int i)
		{
			throw new System.NotImplementedException();
		}

		public virtual int IndexOf(int id)
		{
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromClassIndex(Db4objects.Db4o.Internal.ClassMetadata c)
		{
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromClassIndexes(Db4objects.Db4o.Internal.ClassMetadataIterator
			 i)
		{
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromIdReader(Db4objects.Db4o.Internal.Buffer r)
		{
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromQuery(Db4objects.Db4o.Internal.Query.Processor.QQuery
			 q)
		{
			throw new System.NotImplementedException();
		}

		public abstract Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs();
	}
}
