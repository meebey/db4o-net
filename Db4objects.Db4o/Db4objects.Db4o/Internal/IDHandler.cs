/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class IDHandler : PrimitiveIntHandler
	{
		public IDHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override void DefragIndexEntry(DefragmentContextImpl context)
		{
			context.CopyID(true, false);
		}
	}
}
