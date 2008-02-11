/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class LongHandler0 : LongHandler
	{
		public LongHandler0(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Read(IReadContext context)
		{
			long value = (long)base.Read(context);
			if (value == long.MaxValue)
			{
				return null;
			}
			return value;
		}
	}
}
