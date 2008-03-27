/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class SlotFormatCurrent : SlotFormat
	{
		protected override int HandlerVersion()
		{
			return HandlerRegistry.HandlerVersion;
		}

		public override bool IsIndirectedWithinSlot(ITypeHandler4 handler)
		{
			return IsVariableLength(handler);
		}
	}
}
