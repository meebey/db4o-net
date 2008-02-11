/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Config
{
	/// <exclude></exclude>
	public class Entry : ICompare, ISecondClass
	{
		public object key;

		public object value;

		public virtual object Compare()
		{
			return key;
		}
	}
}
