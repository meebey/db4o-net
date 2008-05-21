/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
