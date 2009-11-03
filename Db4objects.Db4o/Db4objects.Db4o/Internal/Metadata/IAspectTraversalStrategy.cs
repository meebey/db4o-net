/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Metadata;

namespace Db4objects.Db4o.Internal.Metadata
{
	/// <exclude></exclude>
	public interface IAspectTraversalStrategy
	{
		void TraverseAllAspects(ITraverseAspectCommand command);
	}
}
