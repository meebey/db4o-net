namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapFileTransaction : Db4objects.Db4o.Transaction
	{
		private Db4objects.Db4o.Foundation.Tree _slotChanges;

		public YapFileTransaction(Db4objects.Db4o.YapStream a_stream, Db4objects.Db4o.Transaction
			 a_parent) : base(a_stream, a_parent)
		{
		}

		protected override void ClearAll()
		{
			_slotChanges = null;
			base.ClearAll();
		}

		protected override void RollbackSlotChanges()
		{
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass26(this));
			}
		}

		private sealed class _AnonymousInnerClass26 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass26(YapFileTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Inside.Slots.SlotChange)a_object).Rollback(this._enclosing.i_file
					);
			}

			private readonly YapFileTransaction _enclosing;
		}

		public override bool IsDeleted(int id)
		{
			return SlotChangeIsFlaggedDeleted(id);
		}

		protected override void Commit6WriteChanges()
		{
			CheckSynchronization();
			int slotSetPointerCount = CountSlotChanges();
			if (slotSetPointerCount > 0)
			{
				int length = (((slotSetPointerCount * 3) + 2) * Db4objects.Db4o.YapConst.INT_LENGTH
					);
				int address = i_file.GetSlot(length);
				Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(this, address, length
					);
				bytes.WriteInt(length);
				bytes.WriteInt(slotSetPointerCount);
				AppendSlotChanges(bytes);
				bytes.Write();
				FlushFile();
				Stream().WriteTransactionPointer(address);
				FlushFile();
				if (WriteSlots())
				{
					FlushFile();
				}
				Stream().WriteTransactionPointer(0);
				FlushFile();
				i_file.Free(address, length);
			}
		}

		private bool WriteSlots()
		{
			CheckSynchronization();
			bool ret = false;
			if (i_parentTransaction != null)
			{
				if (ParentFileTransaction().WriteSlots())
				{
					ret = true;
				}
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass83(this));
				ret = true;
			}
			return ret;
		}

		private sealed class _AnonymousInnerClass83 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass83(YapFileTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Inside.Slots.SlotChange)a_object).WritePointer(this._enclosing);
			}

			private readonly YapFileTransaction _enclosing;
		}

		protected virtual void FlushFile()
		{
			if (i_file.ConfigImpl().FlushFileBuffers())
			{
				i_file.SyncFiles();
			}
		}

		private Db4objects.Db4o.Inside.Slots.SlotChange ProduceSlotChange(int id)
		{
			Db4objects.Db4o.Inside.Slots.SlotChange slot = new Db4objects.Db4o.Inside.Slots.SlotChange
				(id);
			_slotChanges = Db4objects.Db4o.Foundation.Tree.Add(_slotChanges, slot);
			return (Db4objects.Db4o.Inside.Slots.SlotChange)slot.DuplicateOrThis();
		}

		private Db4objects.Db4o.Inside.Slots.SlotChange FindSlotChange(int a_id)
		{
			CheckSynchronization();
			return (Db4objects.Db4o.Inside.Slots.SlotChange)Db4objects.Db4o.TreeInt.Find(_slotChanges
				, a_id);
		}

		public virtual Db4objects.Db4o.Inside.Slots.Slot GetCurrentSlotOfID(int id)
		{
			CheckSynchronization();
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Inside.Slots.SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				if (change.IsSetPointer())
				{
					return change.NewSlot();
				}
			}
			if (i_parentTransaction != null)
			{
				Db4objects.Db4o.Inside.Slots.Slot parentSlot = ParentFileTransaction().GetCurrentSlotOfID
					(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		public virtual Db4objects.Db4o.Inside.Slots.Slot GetCommittedSlotOfID(int id)
		{
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Inside.Slots.SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				Db4objects.Db4o.Inside.Slots.Slot slot = change.OldSlot();
				if (slot != null)
				{
					return slot;
				}
			}
			if (i_parentTransaction != null)
			{
				Db4objects.Db4o.Inside.Slots.Slot parentSlot = ParentFileTransaction().GetCommittedSlotOfID
					(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		private Db4objects.Db4o.Inside.Slots.Slot ReadCommittedSlotOfID(int id)
		{
			try
			{
				i_file.ReadBytes(_pointerBuffer, id, Db4objects.Db4o.YapConst.POINTER_LENGTH);
			}
			catch
			{
				return null;
			}
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			return new Db4objects.Db4o.Inside.Slots.Slot(address, length);
		}

		public override void SetPointer(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).SetPointer(a_address, a_length);
		}

		private bool SlotChangeIsFlaggedDeleted(int id)
		{
			Db4objects.Db4o.Inside.Slots.SlotChange slot = FindSlotChange(id);
			if (slot != null)
			{
				return slot.IsDeleted();
			}
			if (i_parentTransaction != null)
			{
				return ParentFileTransaction().SlotChangeIsFlaggedDeleted(id);
			}
			return false;
		}

		private int CountSlotChanges()
		{
			int count = 0;
			if (i_parentTransaction != null)
			{
				count += ParentFileTransaction().CountSlotChanges();
			}
			int[] slotSetPointerCount = { count };
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass220(this, slotSetPointerCount));
			}
			return slotSetPointerCount[0];
		}

		private sealed class _AnonymousInnerClass220 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass220(YapFileTransaction _enclosing, int[] slotSetPointerCount
				)
			{
				this._enclosing = _enclosing;
				this.slotSetPointerCount = slotSetPointerCount;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Inside.Slots.SlotChange slot = (Db4objects.Db4o.Inside.Slots.SlotChange
					)obj;
				if (slot.IsSetPointer())
				{
					slotSetPointerCount[0]++;
				}
			}

			private readonly YapFileTransaction _enclosing;

			private readonly int[] slotSetPointerCount;
		}

		internal virtual void WriteOld()
		{
			lock (Stream().i_lock)
			{
				i_pointerIo.UseSlot(i_address);
				i_pointerIo.Read();
				int length = i_pointerIo.ReadInt();
				if (length > 0)
				{
					Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(this, i_address, 
						length);
					bytes.Read();
					bytes.IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
					_slotChanges = new Db4objects.Db4o.TreeReader(bytes, new Db4objects.Db4o.Inside.Slots.SlotChange
						(0)).Read();
					if (WriteSlots())
					{
						FlushFile();
					}
					Stream().WriteTransactionPointer(0);
					FlushFile();
					FreeOnCommit();
				}
				else
				{
					Stream().WriteTransactionPointer(0);
					FlushFile();
				}
			}
		}

		protected sealed override void FreeOnCommit()
		{
			CheckSynchronization();
			if (i_parentTransaction != null)
			{
				ParentFileTransaction().FreeOnCommit();
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass262(this));
			}
		}

		private sealed class _AnonymousInnerClass262 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass262(YapFileTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.Inside.Slots.SlotChange)obj).FreeDuringCommit(this._enclosing.i_file
					);
			}

			private readonly YapFileTransaction _enclosing;
		}

		private void AppendSlotChanges(Db4objects.Db4o.YapReader writer)
		{
			if (i_parentTransaction != null)
			{
				ParentFileTransaction().AppendSlotChanges(writer);
			}
			Db4objects.Db4o.Foundation.Tree.Traverse(_slotChanges, new _AnonymousInnerClass276
				(this, writer));
		}

		private sealed class _AnonymousInnerClass276 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass276(YapFileTransaction _enclosing, Db4objects.Db4o.YapReader
				 writer)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.TreeInt)obj).Write(writer);
			}

			private readonly YapFileTransaction _enclosing;

			private readonly Db4objects.Db4o.YapReader writer;
		}

		internal override void SlotDelete(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			if (a_id == 0)
			{
				return;
			}
			Db4objects.Db4o.Inside.Slots.SlotChange slot = ProduceSlotChange(a_id);
			slot.FreeOnCommit(i_file, new Db4objects.Db4o.Inside.Slots.Slot(a_address, a_length
				));
			slot.SetPointer(0, 0);
		}

		public override void SlotFreeOnCommit(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			if (a_id == 0)
			{
				return;
			}
			ProduceSlotChange(a_id).FreeOnCommit(i_file, new Db4objects.Db4o.Inside.Slots.Slot
				(a_address, a_length));
		}

		internal override void SlotFreeOnRollback(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).FreeOnRollback(a_address, a_length);
		}

		internal override void SlotFreeOnRollbackCommitSetPointer(int a_id, int newAddress
			, int newLength)
		{
			Db4objects.Db4o.Inside.Slots.Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			CheckSynchronization();
			Db4objects.Db4o.Inside.Slots.SlotChange change = ProduceSlotChange(a_id);
			change.FreeOnRollbackSetPointer(newAddress, newLength);
			change.FreeOnCommit(i_file, slot);
		}

		internal override void SlotFreeOnRollbackSetPointer(int a_id, int a_address, int 
			a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).FreeOnRollbackSetPointer(a_address, a_length);
		}

		public override void SlotFreePointerOnCommit(int a_id)
		{
			CheckSynchronization();
			Db4objects.Db4o.Inside.Slots.Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			SlotFreeOnCommit(a_id, slot._address, slot._length);
		}

		internal override void SlotFreePointerOnCommit(int a_id, int a_address, int a_length
			)
		{
			CheckSynchronization();
			SlotFreeOnCommit(a_address, a_address, a_length);
			SlotFreeOnCommit(a_id, a_id, Db4objects.Db4o.YapConst.POINTER_LENGTH);
		}

		private Db4objects.Db4o.YapFileTransaction ParentFileTransaction()
		{
			return (Db4objects.Db4o.YapFileTransaction)i_parentTransaction;
		}

		public override void ProcessDeletes()
		{
			if (i_delete == null)
			{
				i_writtenUpdateDeletedMembers = null;
				return;
			}
			while (i_delete != null)
			{
				Db4objects.Db4o.Foundation.Tree delete = i_delete;
				i_delete = null;
				delete.Traverse(new _AnonymousInnerClass383(this));
			}
			i_writtenUpdateDeletedMembers = null;
		}

		private sealed class _AnonymousInnerClass383 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass383(YapFileTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.DeleteInfo info = (Db4objects.Db4o.DeleteInfo)a_object;
				if (this._enclosing.IsDeleted(info._key))
				{
					return;
				}
				object obj = null;
				if (info._reference != null)
				{
					obj = info._reference.GetObject();
				}
				if (obj == null)
				{
					object[] arr = this._enclosing.Stream().GetObjectAndYapObjectByID(this._enclosing
						, info._key);
					info._reference = (Db4objects.Db4o.YapObject)arr[1];
					info._reference.FlagForDelete(this._enclosing.Stream().TopLevelCallId());
				}
				this._enclosing.Stream().Delete3(this._enclosing, info._reference, info._cascade, 
					false);
			}

			private readonly YapFileTransaction _enclosing;
		}
	}
}
