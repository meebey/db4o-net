namespace Db4objects.Db4o.Internal
{
	using System;
	using Db4objects.Db4o.Internal.Query;
	using Db4objects.Db4o.Internal.Query.Result;
	using Db4objects.Db4o.Internal.Query.Processor;
	using Db4objects.Db4o.Ext;

	/// <summary>
	/// </summary>
	/// <exclude />
	public abstract class ObjectContainerBase : Db4objects.Db4o.Internal.PartialObjectContainer, IObjectContainer, IExtObjectContainer
	{
		internal ObjectContainerBase(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Internal.ObjectContainerBase a_parent)
			: base(config, a_parent)
		{
		}

		void System.IDisposable.Dispose()
		{
			Close();
		}

		public abstract Db4objects.Db4o.Ext.Db4oDatabase Identity();

		public abstract void Backup(string path);
		
		class ComparerAdaptor : Db4objects.Db4o.Query.IQueryComparator
		{
			private System.Collections.IComparer _comparer;

			public ComparerAdaptor(System.Collections.IComparer comparer)
			{
				_comparer = comparer;
			}

			public int Compare(object first, object second)
			{
				return _comparer.Compare(first, second);
			}
		}

		public IObjectSet Query(Db4objects.Db4o.Query.Predicate match, System.Collections.IComparer comparer)
		{
			if (null == match) throw new ArgumentNullException("match");
			return Query(match, new ComparerAdaptor(comparer));
		}

#if NET_2_0 || CF_2_0
		class GenericComparerAdaptor<T> : Db4objects.Db4o.Query.IQueryComparator
		{
			private System.Collections.Generic.IComparer<T> _comparer;

			public GenericComparerAdaptor(System.Collections.Generic.IComparer<T> comparer)
			{
				_comparer = comparer;
			}

			public int Compare(object first, object second)
			{
				return _comparer.Compare((T)first, (T)second);
			}
		}

		class GenericComparisonAdaptor<T> : DelegateEnvelope, Db4objects.Db4o.Query.IQueryComparator
		{
			public GenericComparisonAdaptor(System.Comparison<T> comparer)
				: base(comparer)
			{
			}

			public int Compare(object first, object second)
			{
				System.Comparison<T> _comparer = (System.Comparison<T>)GetContent();
				return _comparer((T)first, (T)second);
			}
		}

		public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match)
		{
			if (null == match) throw new ArgumentNullException("match");
			return GetNativeQueryHandler().Execute(match, null);
		}

		public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match, System.Collections.Generic.IComparer<Extent> comparer)
		{
			if (null == match) throw new ArgumentNullException("match");
			Db4objects.Db4o.Query.IQueryComparator comparator = null != comparer
															? new GenericComparerAdaptor<Extent>(comparer)
															: null;
			return GetNativeQueryHandler().Execute(match, comparator);
		}

		public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match, System.Comparison<Extent> comparison)
		{
			if (null == match) throw new ArgumentNullException("match");
			Db4objects.Db4o.Query.IQueryComparator comparator = null != comparison
															? new GenericComparisonAdaptor<Extent>(comparison)
															: null;
			return GetNativeQueryHandler().Execute(match, comparator);
		}

		public System.Collections.Generic.IList<ElementType> Query<ElementType>(System.Type extent)
		{
			return Query<ElementType>(extent, null);
		}

		public System.Collections.Generic.IList<ElementType> Query<ElementType>(System.Type extent, System.Collections.Generic.IComparer<ElementType> comparer)
		{
			QQuery q = (QQuery)Query();
			q.Constrain(extent);
			if (null != comparer) q.SortBy(new GenericComparerAdaptor<ElementType>(comparer));
			IQueryResult qres = q.GetQueryResult();
			return new Db4objects.Db4o.Internal.Query.GenericObjectSetFacade<ElementType>(qres);
		}

		public System.Collections.Generic.IList<Extent> Query<Extent>()
		{
			return Query<Extent>(typeof(Extent));
		}

		public System.Collections.Generic.IList<Extent> Query<Extent>(System.Collections.Generic.IComparer<Extent> comparer)
		{
			return Query<Extent>(typeof(Extent), comparer);
		}
#endif

	}
}
