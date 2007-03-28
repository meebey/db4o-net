namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class Transaction
	{
		protected Db4objects.Db4o.Foundation.Tree i_delete;

		private Db4objects.Db4o.Foundation.List4 i_dirtyFieldIndexes;

		protected readonly Db4objects.Db4o.Internal.Transaction _systemTransaction;

		private readonly Db4objects.Db4o.Internal.ObjectContainerBase _container;

		private Db4objects.Db4o.Foundation.List4 i_transactionListeners;

		public Transaction(Db4objects.Db4o.Internal.ObjectContainerBase container, Db4objects.Db4o.Internal.Transaction
			 systemTransaction)
		{
			_container = container;
			_systemTransaction = systemTransaction;
		}

		public virtual void AddDirtyFieldIndex(Db4objects.Db4o.Internal.IX.IndexTransaction
			 a_xft)
		{
			i_dirtyFieldIndexes = new Db4objects.Db4o.Foundation.List4(i_dirtyFieldIndexes, a_xft
				);
		}

		public void CheckSynchronization()
		{
		}

		public virtual void AddTransactionListener(Db4objects.Db4o.ITransactionListener a_listener
			)
		{
			i_transactionListeners = new Db4objects.Db4o.Foundation.List4(i_transactionListeners
				, a_listener);
		}

		protected void ClearAll()
		{
			Clear();
			i_dirtyFieldIndexes = null;
			i_transactionListeners = null;
		}

		protected abstract void Clear();

		public virtual void Close(bool a_rollbackOnClose)
		{
			try
			{
				if (Stream() != null)
				{
					CheckSynchronization();
					Stream().ReleaseSemaphores(this);
				}
			}
			catch (System.Exception e)
			{
			}
			if (a_rollbackOnClose)
			{
				try
				{
					Rollback();
				}
				catch (System.Exception e)
				{
				}
			}
		}

		public abstract void Commit();

		protected virtual void FreeOnCommit()
		{
		}

		protected virtual void Commit4FieldIndexes()
		{
			if (_systemTransaction != null)
			{
				_systemTransaction.Commit4FieldIndexes();
			}
			if (i_dirtyFieldIndexes != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_dirtyFieldIndexes
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Internal.IX.IndexTransaction)i.Current).Commit();
				}
			}
		}

		protected virtual void CommitTransactionListeners()
		{
			CheckSynchronization();
			if (i_transactionListeners != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_transactionListeners
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.ITransactionListener)i.Current).PreCommit();
				}
				i_transactionListeners = null;
			}
		}

		public abstract bool IsDeleted(int id);

		protected virtual bool IsSystemTransaction()
		{
			return _systemTransaction == null;
		}

		public virtual bool Delete(Db4objects.Db4o.Internal.ObjectReference @ref, int id, 
			int cascade)
		{
			CheckSynchronization();
			if (@ref != null)
			{
				if (!_container.FlagForDelete(@ref))
				{
					return false;
				}
			}
			Db4objects.Db4o.Internal.DeleteInfo info = (Db4objects.Db4o.Internal.DeleteInfo)Db4objects.Db4o.Internal.TreeInt
				.Find(i_delete, id);
			if (info == null)
			{
				info = new Db4objects.Db4o.Internal.DeleteInfo(id, @ref, cascade);
				i_delete = Db4objects.Db4o.Foundation.Tree.Add(i_delete, info);
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
			i_delete = Db4objects.Db4o.Internal.TreeInt.RemoveLike((Db4objects.Db4o.Internal.TreeInt
				)i_delete, a_id);
		}

		internal virtual void DontRemoveFromClassIndex(int a_yapClassID, int a_id)
		{
			CheckSynchronization();
			Db4objects.Db4o.Internal.ClassMetadata yapClass = Stream().ClassMetadataForId(a_yapClassID
				);
			yapClass.Index().Add(this, a_id);
		}

		public virtual Db4objects.Db4o.Internal.HardObjectReference GetHardReferenceBySignature
			(long a_uuid, byte[] a_signature)
		{
			CheckSynchronization();
			return Stream().GetUUIDIndex().GetHardObjectReferenceBySignature(this, a_uuid, a_signature
				);
		}

		public abstract void ProcessDeletes();

		public virtual Db4objects.Db4o.Reflect.IReflector Reflector()
		{
			return Stream().Reflector();
		}

		public abstract void Rollback();

		protected virtual void RollbackFieldIndexes()
		{
			if (i_dirtyFieldIndexes != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_dirtyFieldIndexes
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Internal.IX.IndexTransaction)i.Current).Rollback();
				}
			}
		}

		protected virtual void RollBackTransactionListeners()
		{
			CheckSynchronization();
			if (i_transactionListeners != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_transactionListeners
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.ITransactionListener)i.Current).PostRollback();
				}
				i_transactionListeners = null;
			}
		}

		public abstract void SetPointer(int a_id, int a_address, int a_length);

		public virtual void SlotDelete(int a_id, int a_address, int a_length)
		{
		}

		public virtual void SlotFreeOnCommit(int a_id, int a_address, int a_length)
		{
		}

		public virtual void SlotFreeOnRollback(int a_id, int a_address, int a_length)
		{
		}

		internal virtual void SlotFreeOnRollbackCommitSetPointer(int a_id, int newAddress
			, int newLength)
		{
		}

		internal virtual void ProduceUpdateSlotChange(int a_id, int a_address, int a_length
			)
		{
		}

		public virtual void SlotFreePointerOnCommit(int a_id)
		{
		}

		internal virtual void SlotFreePointerOnCommit(int a_id, int a_address, int a_length
			)
		{
		}

		public virtual void SlotFreePointerOnRollback(int a_id)
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

		public abstract void WriteUpdateDeleteMembers(int id, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, int typeInfo, int cascade);

		public Db4objects.Db4o.Internal.ObjectContainerBase Stream()
		{
			return _container;
		}

		public virtual Db4objects.Db4o.Internal.Transaction ParentTransaction()
		{
			return _systemTransaction;
		}
	}
}
