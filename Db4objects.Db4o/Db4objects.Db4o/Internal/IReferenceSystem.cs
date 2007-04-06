using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IReferenceSystem
	{
		void AddNewReference(ObjectReference @ref);

		void AddExistingReference(ObjectReference @ref);

		void AddExistingReferenceToObjectTree(ObjectReference @ref);

		void AddExistingReferenceToIdTree(ObjectReference @ref);

		void Commit();

		ObjectReference ReferenceForId(int id);

		ObjectReference ReferenceForObject(object obj);

		void RemoveReference(ObjectReference @ref);

		void Rollback();

		void TraverseReferences(IVisitor4 visitor);
	}
}
