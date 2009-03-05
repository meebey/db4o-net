/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Internal
{
    using System;
    using Db4objects.Db4o.Internal.Query;
    using Db4objects.Db4o.Internal.Query.Result;
    using Db4objects.Db4o.Internal.Query.Processor;
    using Db4objects.Db4o.Ext;

    public partial class ObjectContainerBase : System.IDisposable
    {
        void System.IDisposable.Dispose()
        {
            Close();
        }

        public IObjectSet Query(Db4objects.Db4o.Query.Predicate match, System.Collections.IComparer comparer)
        {
            if (null == match) throw new ArgumentNullException("match");
            return Query(null, match, new ComparerAdaptor(comparer));
        }

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
            return Query(null, match);
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(Transaction trans, Predicate<Extent> match)
        {
            return ExecuteNativeQuery(trans, match, null);
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match, System.Collections.Generic.IComparer<Extent> comparer)
        {
            return Query(null, match, comparer);
        }


        public System.Collections.Generic.IList<Extent> Query<Extent>(Transaction trans, Predicate<Extent> match, System.Collections.Generic.IComparer<Extent> comparer)
        {
            Db4objects.Db4o.Query.IQueryComparator comparator = null != comparer
                                                            ? new GenericComparerAdaptor<Extent>(comparer)
                                                            : null;
            return ExecuteNativeQuery(trans, match, comparator);
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match, System.Comparison<Extent> comparison)
        {
            return Query(null, match, comparison);
        }


        public System.Collections.Generic.IList<Extent> Query<Extent>(Transaction trans, Predicate<Extent> match, System.Comparison<Extent> comparison)
        {
            Db4objects.Db4o.Query.IQueryComparator comparator = null != comparison
                                                        ? new GenericComparisonAdaptor<Extent>(comparison)
                                                        : null;
            return ExecuteNativeQuery(trans, match, comparator);
        }

        public System.Collections.Generic.IList<ElementType> Query<ElementType>(System.Type extent)
        {
            return Query<ElementType>(null, extent, null);
        }


        public System.Collections.Generic.IList<ElementType> Query<ElementType>(Transaction trans, System.Type extent)
        {
            return Query<ElementType>(trans, extent, null);
        }

        public System.Collections.Generic.IList<ElementType> Query<ElementType>(System.Type extent, System.Collections.Generic.IComparer<ElementType> comparer)
        {
            return Query(null, extent, comparer);
        }


        public System.Collections.Generic.IList<ElementType> Query<ElementType>(Transaction trans, System.Type extent, System.Collections.Generic.IComparer<ElementType> comparer)
        {
            lock (Lock())
            {
                trans = CheckTransaction(trans);
                QQuery query = (QQuery)Query(trans);
                query.Constrain(extent);
                if (null != comparer) query.SortBy(new GenericComparerAdaptor<ElementType>(comparer));
                IQueryResult queryResult = query.GetQueryResult();
                return new Db4objects.Db4o.Internal.Query.GenericObjectSetFacade<ElementType>(queryResult);
            }
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>()
        {
            return Query<Extent>(typeof(Extent));
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(System.Collections.Generic.IComparer<Extent> comparer)
        {
            return Query<Extent>(typeof(Extent), comparer);
        }

        private System.Collections.Generic.IList<Extent> ExecuteNativeQuery<Extent>(Transaction trans, Predicate<Extent> match, Db4objects.Db4o.Query.IQueryComparator comparator)
        {
            if (null == match) throw new ArgumentNullException("match");
            lock (Lock())
            {
                return GetNativeQueryHandler().Execute(Query(CheckTransaction(trans)), match, comparator);
            }
        }

    	public delegate R SyncExecClosure<R>();

		public R SyncExec<R>(SyncExecClosure<R> closure)
		{
			return (R)SyncExec(new SyncExecClosure4<R>(closure));
		}

    	public class SyncExecClosure4<R> : IClosure4
    	{
    		private readonly SyncExecClosure<R> _closure;

    		public SyncExecClosure4(SyncExecClosure<R> closure)
    		{
    			_closure = closure;
    		}

    		#region Implementation of IClosure4

    		public object Run()
    		{
    			return _closure.Invoke();
    		}

    		#endregion
    	}
    }
}