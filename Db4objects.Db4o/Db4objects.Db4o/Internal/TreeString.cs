/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class TreeString : Tree
	{
		public string _key;

		public TreeString(string a_key)
		{
			this._key = a_key;
		}

		protected override Tree ShallowCloneInternal(Tree tree)
		{
			Db4objects.Db4o.Internal.TreeString ts = (Db4objects.Db4o.Internal.TreeString)base
				.ShallowCloneInternal(tree);
			ts._key = _key;
			return ts;
		}

		public override object ShallowClone()
		{
			return ShallowCloneInternal(new Db4objects.Db4o.Internal.TreeString(_key));
		}

		public override int Compare(Tree a_to)
		{
			return StringHandler.Compare(Const4.stringIO.Write(((Db4objects.Db4o.Internal.TreeString
				)a_to)._key), Const4.stringIO.Write(_key));
		}

		public override object Key()
		{
			return _key;
		}
	}
}
