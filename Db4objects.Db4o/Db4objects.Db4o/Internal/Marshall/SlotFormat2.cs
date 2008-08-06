/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class SlotFormat2 : SlotFormat
	{
		protected override int HandlerVersion()
		{
			return 2;
		}

		public override bool IsIndirectedWithinSlot(ITypeHandler4 handler)
		{
			return IsVariableLength(handler);
		}
	}
}
