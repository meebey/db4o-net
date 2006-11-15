namespace Db4objects.Db4o.Inside.Slots
{
	/// <exclude></exclude>
	public class ReferencedSlot : Db4objects.Db4o.TreeInt
	{
		private Db4objects.Db4o.Inside.Slots.Slot _slot;

		private int _references;

		public ReferencedSlot(int a_key) : base(a_key)
		{
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Inside.Slots.ReferencedSlot rs = new Db4objects.Db4o.Inside.Slots.ReferencedSlot
				(_key);
			rs._slot = _slot;
			rs._references = _references;
			return base.ShallowCloneInternal(rs);
		}

		public virtual void PointTo(Db4objects.Db4o.Inside.Slots.Slot slot)
		{
			_slot = slot;
		}

		public virtual Db4objects.Db4o.Foundation.Tree Free(Db4objects.Db4o.YapFile file, 
			Db4objects.Db4o.Foundation.Tree treeRoot, Db4objects.Db4o.Inside.Slots.Slot slot
			)
		{
			file.Free(_slot._address, _slot._length);
			if (RemoveReferenceIsLast())
			{
				return treeRoot.RemoveNode(this);
			}
			PointTo(slot);
			return treeRoot;
		}

		public virtual bool AddReferenceIsFirst()
		{
			_references++;
			return (_references == 1);
		}

		public virtual bool RemoveReferenceIsLast()
		{
			_references--;
			return _references < 1;
		}

		public virtual Db4objects.Db4o.Inside.Slots.Slot Slot()
		{
			return _slot;
		}
	}
}
