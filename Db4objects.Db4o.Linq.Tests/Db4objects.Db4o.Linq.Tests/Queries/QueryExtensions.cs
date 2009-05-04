/* Copyright (C) 2007  Versant Inc.  http://www.db4o.com */

using System;

using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Tests.Queries
{
	public static class QueryExtensions
	{
		public static string ToQueryString(this IQuery query)
		{
			return new QueryPrettyPrinter(query).ToString();
		}
	}
}
