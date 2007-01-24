namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class TreeString : Db4objects.Db4o.Foundation.Tree
	{
		public string _key;

		public TreeString(string a_key)
		{
			this._key = a_key;
		}

		protected override Db4objects.Db4o.Foundation.Tree ShallowCloneInternal(Db4objects.Db4o.Foundation.Tree
			 tree)
		{
			Db4objects.Db4o.TreeString ts = (Db4objects.Db4o.TreeString)base.ShallowCloneInternal
				(tree);
			ts._key = _key;
			return ts;
		}

		public override object ShallowClone()
		{
			return ShallowCloneInternal(new Db4objects.Db4o.TreeString(_key));
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree a_to)
		{
			return Db4objects.Db4o.YapString.Compare(Db4objects.Db4o.YapConst.stringIO.Write(
				((Db4objects.Db4o.TreeString)a_to)._key), Db4objects.Db4o.YapConst.stringIO.Write
				(_key));
		}

		public override object Key()
		{
			return _key;
		}
	}
}
