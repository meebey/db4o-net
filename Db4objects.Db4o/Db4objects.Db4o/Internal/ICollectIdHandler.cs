/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// allows collection of IDs if a query is executed on
	/// a field node
	/// </summary>
	public interface ICollectIdHandler : ITypeHandler4
	{
		void CollectIDs(CollectIdContext context);
	}
}
