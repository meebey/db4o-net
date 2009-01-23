/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class Transaction
	{
		protected Tree _delete;

		protected readonly Db4objects.Db4o.Internal.Transaction _systemTransaction;

		/// <summary>
		/// This is the inside representation to operate against, the actual
		/// file-based ObjectContainerBase or the client.
		/// </summary>
		/// <remarks>
		/// This is the inside representation to operate against, the actual
		/// file-based ObjectContainerBase or the client. For all calls
		/// against this ObjectContainerBase the method signatures that take
		/// a transaction have to be used.
		/// </remarks>
		private readonly ObjectContainerBase _container;

		/// <summary>This is the outside representation to the user.</summary>
		/// <remarks>
		/// This is the outside representation to the user. This ObjectContainer
		/// should use this transaction as it's main user transation, so it also
		/// allows using the method signatures on ObjectContainer without a
		/// transaction.
		/// </remarks>
		private IObjectContainer _objectContainer;

		private List4 _transactionListeners;

		private readonly TransactionalReferenceSystem _referenceSystem;

		private readonly IDictionary _locals = new Hashtable();

		public Transaction(ObjectContainerBase container, Db4objects.Db4o.Internal.Transaction
			 systemTransaction, TransactionalReferenceSystem referenceSystem)
		{
			// contains DeleteInfo nodes
			_container = container;
			_systemTransaction = systemTransaction;
			_referenceSystem = referenceSystem;
		}

		/// <summary>Retrieves the value of a transaction local variables.</summary>
		/// <remarks>
		/// Retrieves the value of a transaction local variables.
		/// If this is the first time the variable is accessed
		/// <see cref="Db4objects.Db4o.Internal.TransactionLocal.InitialValueFor">Db4objects.Db4o.Internal.TransactionLocal.InitialValueFor
		/// 	</see>
		/// will provide the initial value.
		/// </remarks>
		public virtual ByRef Get(TransactionLocal local)
		{
			ByRef existing = (ByRef)_locals[local];
			if (null != existing)
			{
				return existing;
			}
			ByRef initialValue = ByRef.NewInstance(local.InitialValueFor(this));
			_locals.Add(local, initialValue);
			return initialValue;
		}

		public void CheckSynchronization()
		{
		}

		public virtual void AddTransactionListener(ITransactionListener listener)
		{
			_transactionListeners = new List4(_transactionListeners, listener);
		}

		protected void ClearAll()
		{
			Clear();
			_transactionListeners = null;
			_locals.Clear();
		}

		protected abstract void Clear();

		public virtual void Close(bool rollbackOnClose)
		{
			if (Container() != null)
			{
				CheckSynchronization();
				Container().ReleaseSemaphores(this);
				if (_referenceSystem != null)
				{
					Container().ReferenceSystemRegistry().RemoveReferenceSystem(_referenceSystem);
				}
			}
			if (rollbackOnClose)
			{
				Rollback();
			}
		}

		public abstract void Commit();

		protected virtual void CommitTransactionListeners()
		{
			CheckSynchronization();
			if (_transactionListeners != null)
			{
				IEnumerator i = new Iterator4Impl(_transactionListeners);
				while (i.MoveNext())
				{
					((ITransactionListener)i.Current).PreCommit();
				}
				_transactionListeners = null;
			}
		}

		public abstract bool IsDeleted(int id);

		protected virtual bool IsSystemTransaction()
		{
			return _systemTransaction == null;
		}

		public virtual bool Delete(ObjectReference @ref, int id, int cascade)
		{
			CheckSynchronization();
			if (@ref != null)
			{
				if (!_container.FlagForDelete(@ref))
				{
					return false;
				}
			}
			if (DTrace.enabled)
			{
				DTrace.TransDelete.Log(id);
			}
			DeleteInfo info = (DeleteInfo)TreeInt.Find(_delete, id);
			if (info == null)
			{
				info = new DeleteInfo(id, @ref, cascade);
				_delete = Tree.Add(_delete, info);
				return true;
			}
			info._reference = @ref;
			if (cascade > info._cascade)
			{
				info._cascade = cascade;
			}
			return true;
		}

		public virtual void DontDelete(int a_id)
		{
			if (DTrace.enabled)
			{
				DTrace.TransDontDelete.Log(a_id);
			}
			if (_delete == null)
			{
				return;
			}
			_delete = TreeInt.RemoveLike((TreeInt)_delete, a_id);
		}

		public virtual HardObjectReference GetHardReferenceBySignature(long a_uuid, byte[]
			 a_signature)
		{
			CheckSynchronization();
			return Container().UUIDIndex().GetHardObjectReferenceBySignature(this, a_uuid, a_signature
				);
		}

		public abstract void ProcessDeletes();

		public virtual IReferenceSystem ReferenceSystem()
		{
			if (_referenceSystem != null)
			{
				return _referenceSystem;
			}
			return ParentTransaction().ReferenceSystem();
		}

		public virtual IReflector Reflector()
		{
			return Container().Reflector();
		}

		public abstract void Rollback();

		protected virtual void RollBackTransactionListeners()
		{
			CheckSynchronization();
			if (_transactionListeners != null)
			{
				IEnumerator i = new Iterator4Impl(_transactionListeners);
				while (i.MoveNext())
				{
					((ITransactionListener)i.Current).PostRollback();
				}
				_transactionListeners = null;
			}
		}

		public void SetPointer(Pointer4 pointer)
		{
			SetPointer(pointer._id, pointer._slot);
		}

		/// <param name="id"></param>
		/// <param name="slot"></param>
		public virtual void SetPointer(int id, Slot slot)
		{
		}

		/// <param name="id"></param>
		/// <param name="slot"></param>
		public virtual void SlotDelete(int id, Slot slot)
		{
		}

		/// <param name="id"></param>
		/// <param name="slot"></param>
		public virtual void SlotFreeOnCommit(int id, Slot slot)
		{
		}

		/// <param name="id"></param>
		/// <param name="slot"></param>
		public virtual void SlotFreeOnRollback(int id, Slot slot)
		{
		}

		/// <param name="id"></param>
		/// <param name="slot"></param>
		/// <param name="forFreespace"></param>
		internal virtual void SlotFreeOnRollbackCommitSetPointer(int id, Slot slot, bool 
			forFreespace)
		{
		}

		/// <param name="id"></param>
		/// <param name="slot"></param>
		internal virtual void ProduceUpdateSlotChange(int id, Slot slot)
		{
		}

		/// <param name="id"></param>
		public virtual void SlotFreePointerOnCommit(int id)
		{
		}

		/// <param name="id"></param>
		/// <param name="slot"></param>
		internal virtual void SlotFreePointerOnCommit(int id, Slot slot)
		{
		}

		/// <param name="id"></param>
		public virtual void SlotFreePointerOnRollback(int id)
		{
		}

		internal virtual bool SupportsVirtualFields()
		{
			return true;
		}

		public virtual Db4objects.Db4o.Internal.Transaction SystemTransaction()
		{
			if (_systemTransaction != null)
			{
				return _systemTransaction;
			}
			return this;
		}

		public override string ToString()
		{
			return Container().ToString();
		}

		public abstract void WriteUpdateDeleteMembers(int id, ClassMetadata clazz, int typeInfo
			, int cascade);

		public ObjectContainerBase Container()
		{
			return _container;
		}

		public virtual Db4objects.Db4o.Internal.Transaction ParentTransaction()
		{
			return _systemTransaction;
		}

		public virtual void RollbackReferenceSystem()
		{
			ReferenceSystem().Rollback();
		}

		public virtual void CommitReferenceSystem()
		{
			ReferenceSystem().Commit();
		}

		public virtual void AddNewReference(ObjectReference @ref)
		{
			ReferenceSystem().AddNewReference(@ref);
		}

		public object ObjectForIdFromCache(int id)
		{
			ObjectReference @ref = ReferenceForId(id);
			if (@ref == null)
			{
				return null;
			}
			object candidate = @ref.GetObject();
			if (candidate == null)
			{
				RemoveReference(@ref);
			}
			return candidate;
		}

		public ObjectReference ReferenceForId(int id)
		{
			ObjectReference @ref = ReferenceSystem().ReferenceForId(id);
			if (@ref != null)
			{
				return @ref;
			}
			if (ParentTransaction() != null)
			{
				return ParentTransaction().ReferenceForId(id);
			}
			return null;
		}

		public ObjectReference ReferenceForObject(object obj)
		{
			ObjectReference @ref = ReferenceSystem().ReferenceForObject(obj);
			if (@ref != null)
			{
				return @ref;
			}
			if (ParentTransaction() != null)
			{
				return ParentTransaction().ReferenceForObject(obj);
			}
			return null;
		}

		public void RemoveReference(ObjectReference @ref)
		{
			ReferenceSystem().RemoveReference(@ref);
			// setting the ID to minus 1 ensures that the
			// gc mechanism does not kill the new YapObject
			@ref.SetID(-1);
			Platform4.KillYapRef(@ref.GetObjectReference());
		}

		public void RemoveObjectFromReferenceSystem(object obj)
		{
			ObjectReference @ref = ReferenceForObject(obj);
			if (@ref != null)
			{
				RemoveReference(@ref);
			}
		}

		public virtual void SetOutSideRepresentation(IObjectContainer objectContainer)
		{
			_objectContainer = objectContainer;
		}

		public virtual IObjectContainer ObjectContainer()
		{
			if (_objectContainer != null)
			{
				return _objectContainer;
			}
			return _container;
		}

		public virtual IContext Context()
		{
			return new _IContext_348(this);
		}

		private sealed class _IContext_348 : IContext
		{
			public _IContext_348(Transaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public IObjectContainer ObjectContainer()
			{
				return this._enclosing.ObjectContainer();
			}

			public Db4objects.Db4o.Internal.Transaction Transaction()
			{
				return this._enclosing;
			}

			private readonly Transaction _enclosing;
		}

		public virtual void Deactivate(int id, IActivationDepth activationDepth)
		{
			//FIXME: JavaServerCrossplatformTestCase crashes with we remove
			//		  null test.
			ObjectReference reference = ReferenceForId(id);
			if (null != reference)
			{
				reference.Deactivate(this, activationDepth);
			}
		}
	}
}
