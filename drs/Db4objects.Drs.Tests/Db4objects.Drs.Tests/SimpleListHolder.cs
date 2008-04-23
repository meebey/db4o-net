/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class SimpleListHolder
	{
		private IList list = new ArrayList();

		public virtual IList GetList()
		{
			return list;
		}

		public virtual void SetList(IList list)
		{
			this.list = list;
		}

		public virtual void Add(SimpleItem item)
		{
			list.Add(item);
		}
	}
}
