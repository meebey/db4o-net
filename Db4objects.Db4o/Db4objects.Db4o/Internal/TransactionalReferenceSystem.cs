namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class TransactionalReferenceSystem : Db4objects.Db4o.Internal.IReferenceSystem
	{
		internal readonly Db4objects.Db4o.Internal.IReferenceSystem _committedReferences = 
			new Db4objects.Db4o.Internal.HashcodeReferenceSystem();

		private Db4objects.Db4o.Internal.IReferenceSystem _newReferences;

		public TransactionalReferenceSystem()
		{
			CreateNewReferences();
		}

		public virtual void AddExistingReference(Db4objects.Db4o.Internal.ObjectReference
			 @ref)
		{
			_committedReferences.AddExistingReference(@ref);
		}

		public virtual void AddExistingReferenceToIdTree(Db4objects.Db4o.Internal.ObjectReference
			 @ref)
		{
			_committedReferences.AddExistingReferenceToIdTree(@ref);
		}

		public virtual void AddExistingReferenceToObjectTree(Db4objects.Db4o.Internal.ObjectReference
			 @ref)
		{
			_committedReferences.AddExistingReferenceToObjectTree(@ref);
		}

		public virtual void AddNewReference(Db4objects.Db4o.Internal.ObjectReference @ref
			)
		{
			_newReferences.AddNewReference(@ref);
		}

		public virtual void Commit()
		{
			TraveseNewReferences(new _AnonymousInnerClass38(this));
			CreateNewReferences();
		}

		private sealed class _AnonymousInnerClass38 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass38(TransactionalReferenceSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing._committedReferences.AddExistingReference((Db4objects.Db4o.Internal.ObjectReference
					)obj);
			}

			private readonly TransactionalReferenceSystem _enclosing;
		}

		public virtual void TraveseNewReferences(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			_newReferences.TraverseReferences(visitor);
		}

		private void CreateNewReferences()
		{
			_newReferences = new Db4objects.Db4o.Internal.HashcodeReferenceSystem();
		}

		public virtual Db4objects.Db4o.Internal.ObjectReference ReferenceForId(int id)
		{
			Db4objects.Db4o.Internal.ObjectReference @ref = _newReferences.ReferenceForId(id);
			if (@ref != null)
			{
				return @ref;
			}
			return _committedReferences.ReferenceForId(id);
		}

		public virtual Db4objects.Db4o.Internal.ObjectReference ReferenceForObject(object
			 obj)
		{
			Db4objects.Db4o.Internal.ObjectReference @ref = _newReferences.ReferenceForObject
				(obj);
			if (@ref != null)
			{
				return @ref;
			}
			return _committedReferences.ReferenceForObject(obj);
		}

		public virtual void RemoveReference(Db4objects.Db4o.Internal.ObjectReference @ref
			)
		{
			_newReferences.RemoveReference(@ref);
			_committedReferences.RemoveReference(@ref);
		}

		public virtual void Rollback()
		{
			CreateNewReferences();
		}

		public virtual void TraverseReferences(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			TraveseNewReferences(visitor);
			_committedReferences.TraverseReferences(visitor);
		}
	}
}
