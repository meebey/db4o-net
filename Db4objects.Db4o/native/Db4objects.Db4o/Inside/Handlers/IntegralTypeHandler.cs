/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside.Handlers
{
	abstract public class IntegralTypeHandler : NetTypeHandler
	{
		public IntegralTypeHandler(ObjectContainerBase containerBase)
			: base(containerBase)
		{
		}

		public override bool IsEqual(Object compare, Object with)
		{
			// sheesh, it would have been nice to call ==,
			// but it doesn't seem to work 
			return compare.Equals(with);
		}
	}
}
