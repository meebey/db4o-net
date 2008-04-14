/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class Item : ListContent
	{
		private readonly ListHolder _parent;

		private Db4objects.Drs.Tests.Item _child;

		public Item(string name) : this(name, null, null)
		{
		}

		public Item(string name, Db4objects.Drs.Tests.Item child, ListHolder parent) : base
			(name)
		{
			_parent = parent;
			_child = child;
		}

		public Item(string name, Db4objects.Drs.Tests.Item child) : this(name, child, null
			)
		{
		}

		public Item(string name, ListHolder list) : this(name, null, list)
		{
		}

		public virtual Db4objects.Drs.Tests.Item Child()
		{
			return Child(1);
		}

		public virtual Db4objects.Drs.Tests.Item Child(int level)
		{
			Db4objects.Drs.Tests.Item tbr = _child;
			while (--level > 0 && tbr != null)
			{
				tbr = tbr._child;
			}
			return tbr;
		}

		public virtual ListHolder Parent()
		{
			return _parent;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Db4objects.Drs.Tests.Item))
			{
				return false;
			}
			Db4objects.Drs.Tests.Item rhs = (Db4objects.Drs.Tests.Item)obj;
			return rhs.GetName().Equals(GetName());
		}

		public virtual void SetChild(Db4objects.Drs.Tests.Item child)
		{
			_child = child;
		}
	}
}
