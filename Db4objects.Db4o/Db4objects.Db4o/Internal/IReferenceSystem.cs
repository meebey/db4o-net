namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IReferenceSystem
	{
		void AddNewReference(Db4objects.Db4o.Internal.ObjectReference @ref);

		void AddExistingReference(Db4objects.Db4o.Internal.ObjectReference @ref);

		void AddExistingReferenceToObjectTree(Db4objects.Db4o.Internal.ObjectReference @ref
			);

		void AddExistingReferenceToIdTree(Db4objects.Db4o.Internal.ObjectReference @ref);

		void Commit();

		Db4objects.Db4o.Internal.ObjectReference ReferenceForId(int id);

		Db4objects.Db4o.Internal.ObjectReference ReferenceForObject(object obj);

		void RemoveReference(Db4objects.Db4o.Internal.ObjectReference @ref);

		void Rollback();

		void TraverseReferences(Db4objects.Db4o.Foundation.IVisitor4 visitor);
	}
}
