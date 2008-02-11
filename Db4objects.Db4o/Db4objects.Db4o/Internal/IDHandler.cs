/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class IDHandler : IntHandler
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
