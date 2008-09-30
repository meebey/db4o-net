/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Drs.Tests.Foundation
{
	public class Set4 : IEnumerable
	{
		public static readonly Db4objects.Drs.Tests.Foundation.Set4 EmptySet = new Db4objects.Drs.Tests.Foundation.Set4
			(0);

		private readonly Hashtable4 _table;

		public Set4()
		{
			_table = new Hashtable4();
		}

		public Set4(int size)
		{
			_table = new Hashtable4(size);
		}

		public Set4(IEnumerable keys) : this()
		{
			AddAll(keys);
		}

		public virtual void Add(object element)
		{
			_table.Put(element, element);
		}

		public virtual void AddAll(IEnumerable other)
		{
			IEnumerator i = other.GetEnumerator();
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

		public virtual IEnumerator GetEnumerator()
		{
			return _table.Keys();
		}

		public override string ToString()
		{
			return Iterators.Join(GetEnumerator(), "[", "]", ", ");
		}
	}
}
