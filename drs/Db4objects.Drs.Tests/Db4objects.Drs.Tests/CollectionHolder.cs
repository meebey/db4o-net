/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Sharpen.Util;

namespace Db4objects.Drs.Tests
{
	public class CollectionHolder
	{
		public string name;

		public IDictionary map = new Hashtable();

		public IList list = new ArrayList();

		public ISet set = new HashSet();

		public CollectionHolder()
		{
		}

		public CollectionHolder(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name + ", hashcode = " + GetHashCode();
		}
	}
}
