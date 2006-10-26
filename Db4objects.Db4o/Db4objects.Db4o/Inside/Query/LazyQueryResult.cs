namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class LazyQueryResult : Db4objects.Db4o.Inside.Query.IQueryResult
	{
		private readonly Db4objects.Db4o.Transaction _transaction;

		private System.Collections.IEnumerable _iterable;

		public LazyQueryResult(Db4objects.Db4o.Transaction trans)
		{
			_transaction = trans;
		}

		public virtual object Get(int index)
		{
			throw new System.NotImplementedException();
		}

		public virtual int IndexOf(int id)
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			if (_iterable == null)
			{
				throw new System.InvalidOperationException();
			}
			return new Db4objects.Db4o.Foundation.IntIterator4Adaptor(_iterable.GetEnumerator
				());
		}

		public virtual void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			_iterable = new _AnonymousInnerClass42(this, clazz);
		}

		private sealed class _AnonymousInnerClass42 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass42(LazyQueryResult _enclosing, Db4objects.Db4o.YapClass
				 clazz)
			{
				this._enclosing = _enclosing;
				this.clazz = clazz;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return this._enclosing.ClassIndexIterator(clazz);
			}

			private readonly LazyQueryResult _enclosing;

			private readonly Db4objects.Db4o.YapClass clazz;
		}

		public virtual System.Collections.IEnumerator ClassIndexIterator(Db4objects.Db4o.YapClass
			 clazz)
		{
			return Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy.Iterate(clazz, Transaction
				());
		}

		public virtual Db4objects.Db4o.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 classCollectionIterator)
		{
			_iterable = new _AnonymousInnerClass58(this, classCollectionIterator);
		}

		private sealed class _AnonymousInnerClass58 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass58(LazyQueryResult _enclosing, Db4objects.Db4o.YapClassCollectionIterator
				 classCollectionIterator)
			{
				this._enclosing = _enclosing;
				this.classCollectionIterator = classCollectionIterator;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new Db4objects.Db4o.Foundation.CompositeIterator4(new _AnonymousInnerClass61
					(this, classCollectionIterator));
			}

			private sealed class _AnonymousInnerClass61 : Db4objects.Db4o.Foundation.MappingIterator
			{
				public _AnonymousInnerClass61(_AnonymousInnerClass58 _enclosing, Db4objects.Db4o.YapClassCollectionIterator
					 baseArg1) : base(baseArg1)
				{
					this._enclosing = _enclosing;
				}

				protected override object Map(object current)
				{
					Db4objects.Db4o.YapClass yapClass = (Db4objects.Db4o.YapClass)current;
					if (this._enclosing._enclosing.SkipClass(yapClass))
					{
						return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
					}
					return this._enclosing._enclosing.ClassIndexIterator(yapClass);
				}

				private readonly _AnonymousInnerClass58 _enclosing;
			}

			private readonly LazyQueryResult _enclosing;

			private readonly Db4objects.Db4o.YapClassCollectionIterator classCollectionIterator;
		}

		public virtual bool SkipClass(Db4objects.Db4o.YapClass yapClass)
		{
			if (yapClass.GetName() == null)
			{
				return true;
			}
			Db4objects.Db4o.Reflect.IReflectClass claxx = yapClass.ClassReflector();
			if (Stream().i_handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx))
			{
				return true;
			}
			return false;
		}

		public virtual void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
		{
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			_iterable = new _AnonymousInnerClass91(this, query);
		}

		private sealed class _AnonymousInnerClass91 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass91(LazyQueryResult _enclosing, Db4objects.Db4o.QQuery 
				query)
			{
				this._enclosing = _enclosing;
				this.query = query;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return query.ExecuteLazy();
			}

			private readonly LazyQueryResult _enclosing;

			private readonly Db4objects.Db4o.QQuery query;
		}

		public virtual Db4objects.Db4o.YapStream Stream()
		{
			return _transaction.Stream();
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer()
		{
			return Stream();
		}

		public virtual int Size()
		{
			throw new System.NotImplementedException();
		}

		public virtual void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			throw new System.NotImplementedException();
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			throw new System.NotImplementedException();
		}
	}
}
