/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public interface IQueryResult : IEnumerable
	{
		object Get(int index);

		IIntIterator4 IterateIDs();

		object Lock();

		IExtObjectContainer ObjectContainer();

		int IndexOf(int id);

		int Size();

		void Sort(IQueryComparator cmp);
	}
}
