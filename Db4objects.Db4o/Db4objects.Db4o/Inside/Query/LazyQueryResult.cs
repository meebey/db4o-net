namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class LazyQueryResult : Db4objects.Db4o.Inside.Query.AbstractQueryResult
	{
		private System.Collections.IEnumerable _iterable;

		public LazyQueryResult(Db4objects.Db4o.Transaction trans) : base(trans)
		{
		}

		public override object Get(int index)
		{
			throw new System.NotImplementedException();
		}

		public override int GetId(int index)
		{
			throw new System.NotImplementedException();
		}

		public override int IndexOf(int id)
		{
			throw new System.NotImplementedException();
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			if (_iterable == null)
			{
				throw new System.InvalidOperationException();
			}
			return new Db4objects.Db4o.Foundation.IntIterator4Adaptor(_iterable.GetEnumerator
				());
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			_iterable = new _AnonymousInnerClass43(this, clazz);
		}

		private sealed class _AnonymousInnerClass43 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass43(LazyQueryResult _enclosing, Db4objects.Db4o.YapClass
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

		public override void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 classCollectionIterator)
		{
			_iterable = new _AnonymousInnerClass55(this, classCollectionIterator);
		}

		private sealed class _AnonymousInnerClass55 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass55(LazyQueryResult _enclosing, Db4objects.Db4o.YapClassCollectionIterator
				 classCollectionIterator)
			{
				this._enclosing = _enclosing;
				this.classCollectionIterator = classCollectionIterator;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new Db4objects.Db4o.Foundation.CompositeIterator4(new _AnonymousInnerClass58
					(this, classCollectionIterator));
			}

			private sealed class _AnonymousInnerClass58 : Db4objects.Db4o.Foundation.MappingIterator
			{
				public _AnonymousInnerClass58(_AnonymousInnerClass55 _enclosing, Db4objects.Db4o.YapClassCollectionIterator
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

				private readonly _AnonymousInnerClass55 _enclosing;
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

		public override void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
		{
			throw new System.NotImplementedException();
		}

		public override void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			_iterable = new _AnonymousInnerClass88(this, query);
		}

		private sealed class _AnonymousInnerClass88 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass88(LazyQueryResult _enclosing, Db4objects.Db4o.QQuery 
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

		public override int Size()
		{
			throw new System.NotImplementedException();
		}

		public override void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			throw new System.NotImplementedException();
		}

		public override Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportSize()
		{
			return ToIdTree();
		}

		public override Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportSort()
		{
			return ToIdList();
		}

		public override Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportElementAccess
			()
		{
			return ToIdList();
		}

		protected override int KnownSize()
		{
			return 0;
		}

		public override Db4objects.Db4o.Inside.Query.AbstractQueryResult ToIdList()
		{
			return ToIdTree().ToIdList();
		}
	}
}
