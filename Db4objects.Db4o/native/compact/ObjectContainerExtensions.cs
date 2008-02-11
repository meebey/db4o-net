/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

#if CF_3_5

using System.Collections.Generic;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Linq
{
	public static class ObjectContainerExtensions
	{
		/// <summary>
		/// This is the entry point of Linq to db4o.
		/// It allows the compiler to call the standard query operators.
		/// 
		/// As Compact Framework doesn't suport Expression Trees, Linq queries
		/// over an ObjectContainer will run unoptimized.
		/// </summary>
		/// <typeparam name="T">The type to query the ObjectContainer</typeparam>
		/// <param name="self">An ObjectContainer</param>
		/// <returns>A <see cref="Db4objects.Db4o.Linq.IDb4oLinqQuery">IDb4oLinqQuery</see> marker interface</returns>
		public static IEnumerable<T> Cast<T>(this IObjectContainer container)
		{
			return container.Query<T>();
		}
	}
}
#endif