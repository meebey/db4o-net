namespace Db4objects.Db4o
{
	public class DeleteInfo : Db4objects.Db4o.TreeInt
	{
		public bool _delete;

		internal int _cascade;

		public Db4objects.Db4o.YapObject _reference;

		public DeleteInfo(int id, Db4objects.Db4o.YapObject reference, bool delete, int cascade
			) : base(id)
		{
			_reference = reference;
			_delete = delete;
			_cascade = cascade;
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.DeleteInfo deleteinfo = new Db4objects.Db4o.DeleteInfo(0, _reference
				, _delete, _cascade);
			return ShallowCloneInternal(deleteinfo);
		}
	}
}
