/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class UntypedFieldHandler2 : Db4objects.Db4o.Internal.UntypedFieldHandler
	{
		public UntypedFieldHandler2(ObjectContainerBase container) : base(container)
		{
		}

		protected override void SeekSecondaryOffset(IReadBuffer buffer, ITypeHandler4 typeHandler
			)
		{
			if (IsPrimitiveArray(typeHandler))
			{
				buffer.SeekCurrentInt();
			}
		}
	}
}
