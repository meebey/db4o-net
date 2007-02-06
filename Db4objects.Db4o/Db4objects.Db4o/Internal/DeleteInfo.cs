namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class DeleteInfo : Db4objects.Db4o.Internal.TreeInt
	{
		internal int _cascade;

		public Db4objects.Db4o.Internal.ObjectReference _reference;

		public DeleteInfo(int id, Db4objects.Db4o.Internal.ObjectReference reference, int
			 cascade) : base(id)
		{
			_reference = reference;
			_cascade = cascade;
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.DeleteInfo deleteinfo = new Db4objects.Db4o.Internal.DeleteInfo
				(0, _reference, _cascade);
			return ShallowCloneInternal(deleteinfo);
		}
	}
}
