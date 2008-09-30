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
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class SimpleItem
	{
		private string value;

		private Db4objects.Drs.Tests.SimpleItem child;

		private SimpleListHolder parent;

		public SimpleItem()
		{
		}

		public SimpleItem(string value_) : this(null, value_)
		{
		}

		public SimpleItem(SimpleListHolder parent_, string value_) : this(parent_, value_
			, null)
		{
		}

		public SimpleItem(SimpleListHolder parent_, string value_, Db4objects.Drs.Tests.SimpleItem
			 child_)
		{
			parent = parent_;
			value = value_;
			child = child_;
		}

		public virtual string GetValue()
		{
			return value;
		}

		public virtual void SetValue(string value_)
		{
			value = value_;
		}

		public virtual Db4objects.Drs.Tests.SimpleItem GetChild()
		{
			return GetChild(0);
		}

		public virtual Db4objects.Drs.Tests.SimpleItem GetChild(int level)
		{
			Db4objects.Drs.Tests.SimpleItem tbr = child;
			while (--level > 0 && tbr != null)
			{
				tbr = tbr.child;
			}
			return tbr;
		}

		public virtual void SetChild(Db4objects.Drs.Tests.SimpleItem child_)
		{
			child = child_;
		}

		public virtual SimpleListHolder GetParent()
		{
			return parent;
		}

		public virtual void SetParent(SimpleListHolder parent_)
		{
			parent = parent_;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Db4objects.Drs.Tests.SimpleItem))
			{
				return false;
			}
			Db4objects.Drs.Tests.SimpleItem rhs = (Db4objects.Drs.Tests.SimpleItem)obj;
			return rhs.GetValue().Equals(GetValue());
		}

		public override string ToString()
		{
			string childString;
			if (child != null)
			{
				childString = child != this ? child.ToString() : "this";
			}
			else
			{
				childString = "null";
			}
			return value + "[" + childString + "]";
		}
	}
}
