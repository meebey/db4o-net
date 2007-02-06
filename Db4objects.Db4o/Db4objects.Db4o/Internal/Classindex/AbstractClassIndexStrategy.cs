namespace Db4objects.Db4o.Internal.Classindex
{
	/// <exclude></exclude>
	public abstract class AbstractClassIndexStrategy : Db4objects.Db4o.Internal.Classindex.IClassIndexStrategy
	{
		protected readonly Db4objects.Db4o.Internal.ClassMetadata _yapClass;

		public AbstractClassIndexStrategy(Db4objects.Db4o.Internal.ClassMetadata yapClass
			)
		{
			_yapClass = yapClass;
		}

		protected virtual int YapClassID()
		{
			return _yapClass.GetID();
		}

		public virtual int OwnLength()
		{
			return Db4objects.Db4o.Internal.Const4.ID_LENGTH;
		}

		protected abstract void InternalAdd(Db4objects.Db4o.Internal.Transaction trans, int
			 id);

		public void Add(Db4objects.Db4o.Internal.Transaction trans, int id)
		{
			CheckId(id);
			InternalAdd(trans, id);
		}

		protected abstract void InternalRemove(Db4objects.Db4o.Internal.Transaction ta, int
			 id);

		public void Remove(Db4objects.Db4o.Internal.Transaction ta, int id)
		{
			CheckId(id);
			InternalRemove(ta, id);
		}

		private void CheckId(int id)
		{
		}

		public abstract System.Collections.IEnumerator AllSlotIDs(Db4objects.Db4o.Internal.Transaction
			 arg1);

		public abstract void DefragIndex(Db4objects.Db4o.Internal.ReaderPair arg1);

		public abstract void DefragReference(Db4objects.Db4o.Internal.ClassMetadata arg1, 
			Db4objects.Db4o.Internal.ReaderPair arg2, int arg3);

		public abstract void DontDelete(Db4objects.Db4o.Internal.Transaction arg1, int arg2
			);

		public abstract int EntryCount(Db4objects.Db4o.Internal.Transaction arg1);

		public abstract int Id();

		public abstract void Initialize(Db4objects.Db4o.Internal.ObjectContainerBase arg1
			);

		public abstract void Purge();

		public abstract void Read(Db4objects.Db4o.Internal.ObjectContainerBase arg1, int 
			arg2);

		public abstract void TraverseAll(Db4objects.Db4o.Internal.Transaction arg1, Db4objects.Db4o.Foundation.IVisitor4
			 arg2);

		public abstract int Write(Db4objects.Db4o.Internal.Transaction arg1);
	}
}
