/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal.IX
{
	/// <exclude></exclude>
	internal class IxDeprecationHelper
	{
		internal static object ComparableObject(IIndexable4 handler, Transaction trans, object
			 indexEntry)
		{
			if (handler is StringHandler)
			{
				return ((StringHandler)handler).Val(indexEntry, trans.Container());
			}
			return indexEntry;
		}
	}
}
