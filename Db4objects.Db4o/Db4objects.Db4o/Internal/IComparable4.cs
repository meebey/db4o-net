/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IComparable4
	{
		IComparable4 PrepareComparison(object obj);

		int CompareTo(object obj);

		IPreparedComparison NewPrepareCompare(object obj);
	}
}
