/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests.Foundation
{
	public class Set4 : System.Collections.IEnumerable
	{
		public static readonly Db4objects.Drs.Tests.Foundation.Set4 EmptySet = new Db4objects.Drs.Tests.Foundation.Set4
			(0);

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _table;

		public Set4()
		{
			_table = new Db4objects.Db4o.Foundation.Hashtable4();
		}

		public Set4(int size)
		{
			_table = new Db4objects.Db4o.Foundation.Hashtable4(size);
		}

		public Set4(System.Collections.IEnumerable keys) : this()
		{
			AddAll(keys);
		}

		public virtual void Add(object element)
		{
			_table.Put(element, element);
		}

		public virtual void AddAll(System.Collections.IEnumerable other)
		{
			System.Collections.IEnumerator i = other.GetEnumerator();
			while (i.MoveNext())
			{
				Add(i.Current);
			}
		}

		public virtual bool IsEmpty()
		{
			return _table.Size() == 0;
		}

		public virtual int Size()
		{
			return _table.Size();
		}

		public virtual bool Contains(object element)
		{
			return _table.Get(element) != null;
		}

		public virtual bool ContainsAll(Db4objects.Drs.Tests.Foundation.Set4 other)
		{
			return _table.ContainsAllKeys(other);
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return _table.Keys();
		}

		public override string ToString()
		{
			return Db4objects.Db4o.Foundation.Iterators.Join(GetEnumerator(), "[", "]", ", ");
		}
	}
}
