/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Drs.Inside
{
	public interface ISimpleObjectContainer
	{
		void Commit();

		void Delete(object obj);

		void DeleteAllInstances(Type clazz);

		/// <summary>Will cascade to save the whole graph of objects</summary>
		/// <param name="o"></param>
		void StoreNew(object o);

		/// <summary>It won't cascade.</summary>
		/// <remarks>It won't cascade. Use it with caution.</remarks>
		/// <param name="o"></param>
		void Update(object o);
	}
}
