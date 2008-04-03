/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class ListHolder
	{
		private string name;

		private IList list;

		public ListHolder()
		{
		}

		public ListHolder(string name)
		{
			this.name = name;
		}

		public virtual void Add(ListContent obj)
		{
			list.Add(obj);
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual IList GetList()
		{
			return list;
		}

		public virtual void SetList(IList list)
		{
			this.list = list;
		}

		public override string ToString()
		{
			return "name = " + name + ", list = " + list;
		}
	}
}
