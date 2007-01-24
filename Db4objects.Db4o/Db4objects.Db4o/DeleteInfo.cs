namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class DeleteInfo : Db4objects.Db4o.TreeInt
	{
		internal int _cascade;

		public Db4objects.Db4o.YapObject _reference;

		public DeleteInfo(int id, Db4objects.Db4o.YapObject reference, int cascade) : base
			(id)
		{
			_reference = reference;
			_cascade = cascade;
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.DeleteInfo deleteinfo = new Db4objects.Db4o.DeleteInfo(0, _reference
				, _cascade);
			return ShallowCloneInternal(deleteinfo);
		}
	}
}
