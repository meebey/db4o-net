/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Internal.Collections;

namespace Db4objects.Db4o.Internal.Collections
{
	/// <exclude></exclude>
	public interface IPersistentList
	{
		bool Add(object o);

		void Add(int index, object element);

		bool AddAll(IEnumerable i);

		bool AddAll(int index, IEnumerable i);

		void Clear();

		bool Contains(object o);

		bool ContainsAll(IEnumerable i);

		object Get(int index);

		int IndexOf(object o);

		bool IsEmpty();

		IEnumerator Iterator();

		int LastIndexOf(object o);

		bool Remove(object o);

		object Remove(int index);

		bool RemoveAll(IEnumerable i);

		bool RetainAll(IEnumerable i);

		object Set(int index, object element);

		int Size();

		IPersistentList SubList(int fromIndex, int toIndex);

		object[] ToArray();

		object[] ToArray(object[] a);
	}
}
