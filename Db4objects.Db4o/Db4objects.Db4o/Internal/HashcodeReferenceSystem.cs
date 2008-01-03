/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class HashcodeReferenceSystem : IReferenceSystem
	{
		private ObjectReference _hashCodeTree;

		private ObjectReference _idTree;

		public virtual void AddNewReference(ObjectReference @ref)
		{
			AddReference(@ref);
		}

		public virtual void AddExistingReference(ObjectReference @ref)
		{
			AddReference(@ref);
		}

		private void AddReference(ObjectReference @ref)
		{
			IdAdd(@ref);
			HashCodeAdd(@ref);
		}

		public virtual void AddExistingReferenceToObjectTree(ObjectReference @ref)
		{
			HashCodeAdd(@ref);
		}

		public virtual void AddExistingReferenceToIdTree(ObjectReference @ref)
		{
			IdAdd(@ref);
		}

		public virtual void Commit()
		{
		}

		private void HashCodeAdd(ObjectReference @ref)
		{
			if (_hashCodeTree == null)
			{
				@ref.Hc_init();
				_hashCodeTree = @ref;
				return;
			}
			_hashCodeTree = _hashCodeTree.Hc_add(@ref);
		}

		private void IdAdd(ObjectReference @ref)
		{
			if (DTrace.enabled)
			{
				DTrace.IdTreeAdd.Log(@ref.GetID());
			}
			if (_idTree == null)
			{
				@ref.Hc_init();
				_idTree = @ref;
				return;
			}
			_idTree = _idTree.Id_add(@ref);
		}

		public virtual ObjectReference ReferenceForId(int id)
		{
			if (DTrace.enabled)
			{
				DTrace.GetYapobject.Log(id);
			}
			if (_idTree == null)
			{
				return null;
			}
			if (!ObjectReference.IsValidId(id))
			{
				return null;
			}
			return _idTree.Id_find(id);
		}

		public virtual ObjectReference ReferenceForObject(object obj)
		{
			if (_hashCodeTree == null)
			{
				return null;
			}
			return _hashCodeTree.Hc_find(obj);
		}

		public virtual void RemoveReference(ObjectReference @ref)
		{
			if (DTrace.enabled)
			{
				DTrace.ReferenceRemoved.Log(@ref.GetID());
			}
			if (_hashCodeTree != null)
			{
				_hashCodeTree = _hashCodeTree.Hc_remove(@ref);
			}
			if (_idTree != null)
			{
				_idTree = _idTree.Id_remove(@ref.GetID());
			}
		}

		public virtual void Rollback()
		{
		}

		public virtual void TraverseReferences(IVisitor4 visitor)
		{
			if (_hashCodeTree == null)
			{
				return;
			}
			_hashCodeTree.Hc_traverse(visitor);
		}
	}
}
