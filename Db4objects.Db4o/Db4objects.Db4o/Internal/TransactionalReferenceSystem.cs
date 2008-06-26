/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class TransactionalReferenceSystem : IReferenceSystem
	{
		private readonly IReferenceSystem _committedReferences;

		private IReferenceSystem _newReferences;

		public TransactionalReferenceSystem()
		{
			CreateNewReferences();
			_committedReferences = NewReferenceSystem();
		}

		private IReferenceSystem NewReferenceSystem()
		{
			return new HashcodeReferenceSystem();
		}

		// An alternative reference system using a hashtable: 
		// return new HashtableReferenceSystem();
		public virtual void AddExistingReference(ObjectReference @ref)
		{
			_committedReferences.AddExistingReference(@ref);
		}

		public virtual void AddNewReference(ObjectReference @ref)
		{
			_newReferences.AddNewReference(@ref);
		}

		public virtual void Commit()
		{
			TraveseNewReferences(new _IVisitor4_38(this));
			CreateNewReferences();
		}

		private sealed class _IVisitor4_38 : IVisitor4
		{
			public _IVisitor4_38(TransactionalReferenceSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				ObjectReference oref = (ObjectReference)obj;
				object referent = oref.GetObject();
				if (referent != null)
				{
					this._enclosing._committedReferences.AddExistingReference(oref);
				}
			}

			private readonly TransactionalReferenceSystem _enclosing;
		}

		public virtual void TraveseNewReferences(IVisitor4 visitor)
		{
			_newReferences.TraverseReferences(visitor);
		}

		private void CreateNewReferences()
		{
			_newReferences = NewReferenceSystem();
		}

		public virtual ObjectReference ReferenceForId(int id)
		{
			ObjectReference @ref = _newReferences.ReferenceForId(id);
			if (@ref != null)
			{
				return @ref;
			}
			return _committedReferences.ReferenceForId(id);
		}

		public virtual ObjectReference ReferenceForObject(object obj)
		{
			ObjectReference @ref = _newReferences.ReferenceForObject(obj);
			if (@ref != null)
			{
				return @ref;
			}
			return _committedReferences.ReferenceForObject(obj);
		}

		public virtual void RemoveReference(ObjectReference @ref)
		{
			_newReferences.RemoveReference(@ref);
			_committedReferences.RemoveReference(@ref);
		}

		public virtual void Rollback()
		{
			CreateNewReferences();
		}

		public virtual void TraverseReferences(IVisitor4 visitor)
		{
			TraveseNewReferences(visitor);
			_committedReferences.TraverseReferences(visitor);
		}
	}
}
