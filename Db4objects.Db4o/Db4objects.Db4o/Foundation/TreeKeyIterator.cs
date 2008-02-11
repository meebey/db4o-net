/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class TreeKeyIterator : AbstractTreeIterator
	{
		public TreeKeyIterator(Tree tree) : base(tree)
		{
		}

		protected override object CurrentValue(Tree tree)
		{
			return tree.Key();
		}
	}
}
