/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Test.Foundation
{
	public class Set4
	{
		public static readonly Db4objects.Drs.Test.Foundation.Set4 EMPTY_SET = new Db4objects.Drs.Test.Foundation.Set4
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

		public virtual void Add(object element)
		{
			_table.Put(element, element);
		}

		public virtual void AddAll(Db4objects.Drs.Test.Foundation.Set4 other)
		{
			System.Collections.IEnumerator i = other._table.Iterator();
			while (i.MoveNext())
			{
				Add(((Db4objects.Db4o.Foundation.IEntry4)i.Current).Key());
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

		public virtual bool ContainsAll(Db4objects.Drs.Test.Foundation.Set4 other)
		{
			System.Collections.IEnumerator i = other.Iterator();
			while (i.MoveNext())
			{
				if (!Contains(i.Current))
				{
					return false;
				}
			}
			return true;
		}

		public virtual System.Collections.IEnumerator Iterator()
		{
			return _table.Keys();
		}

		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder("[");
			bool first = true;
			for (System.Collections.IEnumerator iter = Iterator(); iter.MoveNext(); )
			{
				if (!first)
				{
					buf.Append(',');
				}
				else
				{
					first = false;
				}
				buf.Append(iter.Current.ToString());
			}
			buf.Append(']');
			return buf.ToString();
		}
	}
}
