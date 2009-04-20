/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Sharpen.Util;

namespace Db4objects.Drs.Tests
{
	public class CollectionHolder
	{
		public string name;

		public IDictionary map;

		public IList list;

		public ISet set;

		public CollectionHolder() : this(new Hashtable(), new HashSet(), new ArrayList())
		{
		}

		public CollectionHolder(string name) : this()
		{
			this.name = name;
		}

		public CollectionHolder(IDictionary theMap, ISet theSet, IList theList)
		{
			map = theMap;
			set = theSet;
			list = theList;
		}

		public override string ToString()
		{
			return name + ", hashcode = " + GetHashCode();
		}
	}
}
