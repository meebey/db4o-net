namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class TreeStringObject : Db4objects.Db4o.TreeString
	{
		public readonly object _object;

		public TreeStringObject(string a_key, object a_object) : base(a_key)
		{
			this._object = a_object;
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.TreeStringObject tso = new Db4objects.Db4o.TreeStringObject(_key, 
				_object);
			return ShallowCloneInternal(tso);
		}
	}
}
