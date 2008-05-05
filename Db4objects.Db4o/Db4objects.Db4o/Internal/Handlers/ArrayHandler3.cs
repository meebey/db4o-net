/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class ArrayHandler3 : ArrayHandler
	{
		protected override bool IsPrimitive(IReflectClass claxx)
		{
			return false;
			return claxx.IsPrimitive();
		}

		protected sealed override bool UseJavaHandling()
		{
			return !Deploy.csharp;
		}

		protected override bool HasNullBitmap()
		{
			return false;
		}

		protected override bool ReadingDotNetBeforeVersion4()
		{
			if (NullableArrayHandling.Enabled() && Deploy.csharp)
			{
				return true;
			}
			return false;
		}
	}
}
