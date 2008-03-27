/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class CollectionHolder
	{
		public string name;

		public System.Collections.IDictionary map = new System.Collections.Hashtable();

		public System.Collections.IList list = new System.Collections.ArrayList();

		public Sharpen.Util.ISet set = new Sharpen.Util.HashSet();

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
