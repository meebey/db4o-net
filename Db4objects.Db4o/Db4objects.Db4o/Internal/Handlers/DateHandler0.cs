/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;
using Sharpen.Util;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class DateHandler0 : DateHandler
	{
		public DateHandler0(ObjectContainerBase container) : base(container)
		{
		}

		public override object Read(IReadContext context)
		{
			long value = context.ReadLong();
			if (value == long.MaxValue)
			{
				return null;
			}
			return new Date(value);
		}
	}
}
