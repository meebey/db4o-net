/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class DoubleHandler0 : DoubleHandler
	{
		public DoubleHandler0(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Read(IReadContext context)
		{
			double value = (double)base.Read(context);
			if (double.IsNaN(value))
			{
				return null;
			}
			return value;
		}
	}
}
