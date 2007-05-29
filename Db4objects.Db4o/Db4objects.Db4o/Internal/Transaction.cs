/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.IX;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class Transaction
	{
		protected Tree i_delete;

		private List4 i_dirtyFieldIndexes;

		protected readonly Db4objects.Db4o.Internal.Transaction _systemTransaction;

		private readonly ObjectContainerBase _container;

		private List4 i_transactionListeners;

		public Transaction(ObjectContainerBase container, Db4objects.Db4o.Internal.Transaction
			 systemTransaction)
		{
			_container = container;
			_systemTransaction = systemTransaction;
		}

		public virtual void AddDirtyFieldIndex(IndexTransaction a_xft)
		{
			i_dirtyFieldIndexes = new List4(i_dirtyFieldIndexes, a_xft);
		}

		public void CheckSynchronization()
		{
		}

		public virtual void AddTransactionListener(ITransactionListener a_listener)
		{
			i_transactionListeners = new List4(i_transactionListeners, a_listener);
		}

		protected void ClearAll()
		{
			Clear();
			i_dirtyFieldIndexes = null;
			i_transactionListeners = null;
		}

		protected abstract void Clear();

		public virtual void Close(bool rollbackOnClose)
		{
			if (Stream() != null)
			{
				CheckSynchronization();
				Stream().ReleaseSemaphores(this);
			}
			if (rollbackOnClose)
			{
				Rollback();
			}
		}

		public abstract void Commit();

		protected virtual void Commit4FieldIndexes()
		{
			if (_systemTransaction != null)
			{
				_systemTransaction.Commit4FieldIndexes();
			}
			if (i_dirtyFieldIndexes != null)
			{
				IEnumerator i = new Iterator4Impl(i_dirtyFieldIndexes);
				while (i.MoveNext())
				{
					((IndexTransaction)i.Current).Commit();
				}
			}
		}

		protected virtual void CommitTransactionListeners()
		{
			CheckSynchronization();
			if (i_transactionListeners != null)
			{
				IEnumerator i = new Iterator4Impl(i_transactionListeners);
				while (i.MoveNext())
				{
					((ITransactionListener)i.Current).PreCommit();
				}
				i_transactionListeners = null;
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
			DeleteInfo info = (DeleteInfo)TreeInt.Find(i_delete, id);
			if (info == null)
			{
				info = new DeleteInfo(id, @ref, cascade);
				i_delete = Tree.Add(i_delete, info);
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
			if (i_delete == null)
			{
				return;
			}
			i_delete = TreeInt.RemoveLike((TreeInt)i_delete, a_id);
		}

		internal virtual void DontRemoveFromClassIndex(int a_yapClassID, int a_id)
		{
			CheckSynchronization();
			ClassMetadata yapClass = Stream().ClassMetadataForId(a_yapClassID);
			yapClass.Index().Add(this, a_id);
		}

		public virtual HardObjectReference GetHardReferenceBySignature(long a_uuid, byte[]
			 a_signature)
		{
			CheckSynchronization();
			return Stream().GetUUIDIndex().GetHardObjectReferenceBySignature(this, a_uuid, a_signature
				);
		}

		public abstract void ProcessDeletes();

		public virtual IReflector Reflector()
		{
			return Stream().Reflector();
		}

		public abstract void Rollback();

		protected virtual void RollbackFieldIndexes()
		{
			if (i_dirtyFieldIndexes != null)
			{
				IEnumerator i = new Iterator4Impl(i_dirtyFieldIndexes);
				while (i.MoveNext())
				{
					((IndexTransaction)i.Current).Rollback();
				}
			}
		}

		protected virtual void RollBackTransactionListeners()
		{
			CheckSynchronization();
			if (i_transactionListeners != null)
			{
				IEnumerator i = new Iterator4Impl(i_transactionListeners);
				while (i.MoveNext())
				{
					((ITransactionListener)i.Current).PostRollback();
				}
				i_transactionListeners = null;
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
			return Stream().ToString();
		}

		public abstract void WriteUpdateDeleteMembers(int id, ClassMetadata clazz, int typeInfo
			, int cascade);

		public ObjectContainerBase Stream()
		{
			return _container;
		}

		public virtual Db4objects.Db4o.Internal.Transaction ParentTransaction()
		{
			return _systemTransaction;
		}
	}
}
