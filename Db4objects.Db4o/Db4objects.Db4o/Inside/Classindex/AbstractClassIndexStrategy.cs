namespace Db4objects.Db4o.Inside.Classindex
{
	/// <exclude></exclude>
	public abstract class AbstractClassIndexStrategy : Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy
	{
		protected readonly Db4objects.Db4o.YapClass _yapClass;

		public AbstractClassIndexStrategy(Db4objects.Db4o.YapClass yapClass)
		{
			_yapClass = yapClass;
		}

		protected virtual int YapClassID()
		{
			return _yapClass.GetID();
		}

		public virtual int OwnLength()
		{
			return Db4objects.Db4o.YapConst.ID_LENGTH;
		}

		protected abstract void InternalAdd(Db4objects.Db4o.Transaction trans, int id);

		public void Add(Db4objects.Db4o.Transaction trans, int id)
		{
			CheckId(id);
			InternalAdd(trans, id);
		}

		protected abstract void InternalRemove(Db4objects.Db4o.Transaction ta, int id);

		public void Remove(Db4objects.Db4o.Transaction ta, int id)
		{
			CheckId(id);
			InternalRemove(ta, id);
		}

		private void CheckId(int id)
		{
		}

		public abstract System.Collections.IEnumerator AllSlotIDs(Db4objects.Db4o.Transaction
			 arg1);

		public abstract void DefragIndex(Db4objects.Db4o.ReaderPair arg1);

		public abstract void DefragReference(Db4objects.Db4o.YapClass arg1, Db4objects.Db4o.ReaderPair
			 arg2, int arg3);

		public abstract void DontDelete(Db4objects.Db4o.Transaction arg1, int arg2);

		public abstract int EntryCount(Db4objects.Db4o.Transaction arg1);

		public abstract int Id();

		public abstract void Initialize(Db4objects.Db4o.YapStream arg1);

		public abstract void Purge();

		public abstract void Read(Db4objects.Db4o.YapStream arg1, int arg2);

		public abstract void TraverseAll(Db4objects.Db4o.Transaction arg1, Db4objects.Db4o.Foundation.IVisitor4
			 arg2);

		public abstract int Write(Db4objects.Db4o.Transaction arg1);
	}
}
