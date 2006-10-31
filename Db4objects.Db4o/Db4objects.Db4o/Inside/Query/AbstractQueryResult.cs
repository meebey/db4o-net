namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public abstract class AbstractQueryResult : Db4objects.Db4o.Inside.Query.IQueryResult
	{
		protected readonly Db4objects.Db4o.Transaction _transaction;

		public AbstractQueryResult(Db4objects.Db4o.Transaction transaction)
		{
			_transaction = transaction;
		}

		public object Activate(object obj)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			stream.Activate1(_transaction, obj, stream.ConfigImpl().ActivationDepth());
			return obj;
		}

		public object ActivatedObject(int id)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			object ret = stream.GetActivatedObjectFromCache(_transaction, id);
			if (ret != null)
			{
				return ret;
			}
			return stream.ReadActivatedObjectNotInCache(_transaction, id);
		}

		public virtual object StreamLock()
		{
			Db4objects.Db4o.YapStream stream = Stream();
			stream.CheckClosed();
			return stream.Lock();
		}

		public virtual Db4objects.Db4o.YapStream Stream()
		{
			return _transaction.Stream();
		}

		public virtual Db4objects.Db4o.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer()
		{
			return Stream();
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return new _AnonymousInnerClass55(this, IterateIDs());
		}

		private sealed class _AnonymousInnerClass55 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass55(AbstractQueryResult _enclosing, Db4objects.Db4o.Foundation.IIntIterator4
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

		public virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportSize()
		{
			return this;
		}

		public virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportSort()
		{
			return this;
		}

		public virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportElementAccess
			()
		{
			return this;
		}

		protected virtual int KnownSize()
		{
			return Size();
		}

		public virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult ToIdList()
		{
			Db4objects.Db4o.Inside.Query.IdListQueryResult res = new Db4objects.Db4o.Inside.Query.IdListQueryResult
				(Transaction(), KnownSize());
			Db4objects.Db4o.Foundation.IIntIterator4 i = IterateIDs();
			while (i.MoveNext())
			{
				res.Add(i.CurrentInt());
			}
			return res;
		}

		protected virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult ToIdTree()
		{
			return new Db4objects.Db4o.Inside.Query.IdTreeQueryResult(Transaction(), this);
		}

		public abstract object Get(int arg1);

		public abstract int IndexOf(int arg1);

		public abstract Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs();

		public abstract void LoadFromClassIndex(Db4objects.Db4o.YapClass arg1);

		public abstract void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 arg1);

		public abstract void LoadFromIdReader(Db4objects.Db4o.YapReader arg1);

		public abstract void LoadFromQuery(Db4objects.Db4o.QQuery arg1);

		public abstract int Size();

		public abstract void Sort(Db4objects.Db4o.Query.IQueryComparator arg1);
	}
}
