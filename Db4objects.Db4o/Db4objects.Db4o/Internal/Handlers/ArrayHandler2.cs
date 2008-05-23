/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler2 : ArrayHandler3
	{
		/// <summary>FIXME: We are not changing any behaviour, why do we override?</summary>
		protected override int PreparePayloadRead(IDefragmentContext context)
		{
			return context.Offset();
		}
	}
}
