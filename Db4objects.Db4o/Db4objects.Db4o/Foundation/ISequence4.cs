/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public interface ISequence4 : IEnumerable
	{
		bool Add(object element);

		bool IsEmpty();

		object Get(int index);

		int Size();

		void Clear();

		bool Remove(object obj);

		bool Contains(object obj);

		object[] ToArray();
	}
}
