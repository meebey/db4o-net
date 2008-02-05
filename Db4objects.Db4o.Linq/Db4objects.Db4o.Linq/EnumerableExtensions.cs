/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Db4objects.Db4o.Linq
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> Cast<T>(this IEnumerable enumerable)
		{
			return new ObjectSequence<T>(enumerable);
		}
	}
}
