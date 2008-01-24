/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IReferenceSystem
	{
		void AddNewReference(ObjectReference @ref);

		void AddExistingReference(ObjectReference @ref);

		void Commit();

		ObjectReference ReferenceForId(int id);

		ObjectReference ReferenceForObject(object obj);

		void RemoveReference(ObjectReference @ref);

		void Rollback();

		void TraverseReferences(IVisitor4 visitor);
	}
}
