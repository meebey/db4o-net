/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class EmbeddedClientObjectContainer : PartialEmbeddedClientObjectContainer
		, IInternalObjectContainer
	{
		public EmbeddedClientObjectContainer(LocalObjectContainer server) : base(server)
		{
		}

		public EmbeddedClientObjectContainer(LocalObjectContainer server, Transaction trans
			) : base(server, trans)
		{
		}
        void System.IDisposable.Dispose()
        {
            Close();
        }

		public IObjectSet Query(Db4objects.Db4o.Query.Predicate match, IQueryComparator comparator)
		{
			return GetNativeQueryHandler().Execute(Query(), match, comparator);
		}

        public IObjectSet Query(Db4objects.Db4o.Query.Predicate match, System.Collections.IComparer comparer)
        {
            return _server.Query(_transaction, match, new ComparerAdaptor(comparer));
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match)
        {
            return _server.Query(_transaction, match);
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match, System.Collections.Generic.IComparer<Extent> comparer)
        {
            return _server.Query(_transaction, match, comparer);
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(Predicate<Extent> match, System.Comparison<Extent> comparison)
        {
            return _server.Query(_transaction, match, comparison);
        }

        public System.Collections.Generic.IList<ElementType> Query<ElementType>(System.Type extent)
        {
            return _server.Query<ElementType>(_transaction, extent, null);
        }

        public System.Collections.Generic.IList<ElementType> Query<ElementType>(System.Type extent, System.Collections.Generic.IComparer<ElementType> comparer)
        {
            return _server.Query(_transaction, extent, comparer);
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>()
        {
            return Query<Extent>(typeof(Extent));
        }

        public System.Collections.Generic.IList<Extent> Query<Extent>(System.Collections.Generic.IComparer<Extent> comparer)
        {
            return Query<Extent>(typeof(Extent), comparer);
        }
    }
}
