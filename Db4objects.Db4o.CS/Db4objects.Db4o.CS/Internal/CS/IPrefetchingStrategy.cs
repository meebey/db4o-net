/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Internal.CS
{
	/// <summary>Defines a strategy on how to prefetch objects from the server.</summary>
	/// <remarks>Defines a strategy on how to prefetch objects from the server.</remarks>
	public interface IPrefetchingStrategy
	{
		int PrefetchObjects(ClientObjectContainer container, IIntIterator4 ids, object[] 
			prefetched, int prefetchCount);
	}
}
