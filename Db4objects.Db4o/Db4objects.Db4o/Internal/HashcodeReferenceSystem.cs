namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class HashcodeReferenceSystem : Db4objects.Db4o.Internal.IReferenceSystem
	{
		private Db4objects.Db4o.Internal.ObjectReference _hashCodeTree;

		private Db4objects.Db4o.Internal.ObjectReference _idTree;

		public virtual void AddNewReference(Db4objects.Db4o.Internal.ObjectReference @ref
			)
		{
			AddReference(@ref);
		}

		public virtual void AddExistingReference(Db4objects.Db4o.Internal.ObjectReference
			 @ref)
		{
			AddReference(@ref);
		}

		private void AddReference(Db4objects.Db4o.Internal.ObjectReference @ref)
		{
			IdAdd(@ref);
			HashCodeAdd(@ref);
		}

		public virtual void AddExistingReferenceToObjectTree(Db4objects.Db4o.Internal.ObjectReference
			 @ref)
		{
			HashCodeAdd(@ref);
		}

		public virtual void AddExistingReferenceToIdTree(Db4objects.Db4o.Internal.ObjectReference
			 @ref)
		{
			IdAdd(@ref);
		}

		public virtual void Commit()
		{
		}

		private void HashCodeAdd(Db4objects.Db4o.Internal.ObjectReference @ref)
		{
			if (_hashCodeTree == null)
			{
				@ref.Hc_init();
				_hashCodeTree = @ref;
				return;
			}
			_hashCodeTree = _hashCodeTree.Hc_add(@ref);
		}

		private void IdAdd(Db4objects.Db4o.Internal.ObjectReference @ref)
		{
			if (_idTree == null)
			{
				@ref.Hc_init();
				_idTree = @ref;
				return;
			}
			_idTree = _idTree.Id_add(@ref);
		}

		public virtual Db4objects.Db4o.Internal.ObjectReference ReferenceForId(int id)
		{
			if (_idTree == null)
			{
				return null;
			}
			if (!Db4objects.Db4o.Internal.ObjectReference.IsValidId(id))
			{
				return null;
			}
			return _idTree.Id_find(id);
		}

		public virtual Db4objects.Db4o.Internal.ObjectReference ReferenceForObject(object
			 obj)
		{
			if (_hashCodeTree == null)
			{
				return null;
			}
			return _hashCodeTree.Hc_find(obj);
		}

		public virtual void RemoveReference(Db4objects.Db4o.Internal.ObjectReference @ref
			)
		{
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

		public virtual void TraverseReferences(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			if (_hashCodeTree == null)
			{
				return;
			}
			_hashCodeTree.Hc_traverse(visitor);
		}
	}
}
