/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Internal
{
	public partial class Config4Impl
	{
		private static Type[] IgnoredClasses()
		{
			return new Type[] { typeof(P1Object), typeof(StaticClass), typeof(StaticField) };
		}
	}
}
