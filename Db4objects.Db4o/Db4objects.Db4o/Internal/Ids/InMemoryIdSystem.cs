/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class InMemoryIdSystem : IIdSystem
	{
		private readonly LocalObjectContainer _container;

		private IdSlotMapping _ids;

		private int _idGenerator;

		private Slot _slot;

		public InMemoryIdSystem(LocalObjectContainer container)
		{
			_container = container;
			_idGenerator = _container.Handlers.LowestValidId();
			SystemData systemData = _container.SystemData();
			_slot = new Slot(systemData.TransactionPointer1(), systemData.TransactionPointer2
				());
			if (!_slot.IsNull())
			{
				ByteArrayBuffer buffer = _container.ReadBufferBySlot(_slot);
				_idGenerator = buffer.ReadInt();
				_ids = (IdSlotMapping)new TreeReader(buffer, new IdSlotMapping(0, null)).Read();
			}
		}

		public virtual void Close()
		{
		}

		// TODO Auto-generated method stub
		public virtual void Commit(IVisitable slotChanges, IRunnable commitBlock)
		{
			commitBlock.Run();
			if (_slot != null && !_slot.IsNull())
			{
				_container.Free(_slot);
			}
			slotChanges.Accept(new _IVisitor4_45(this));
			WriteThis();
		}

		private sealed class _IVisitor4_45 : IVisitor4
		{
			public _IVisitor4_45(InMemoryIdSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object slotChange)
			{
				if (!((SlotChange)slotChange).SlotModified())
				{
					return;
				}
				if (((SlotChange)slotChange).RemoveId())
				{
					this._enclosing._ids = (IdSlotMapping)Tree.RemoveLike(this._enclosing._ids, new TreeInt
						(((TreeInt)slotChange)._key));
					return;
				}
				this._enclosing._ids = ((IdSlotMapping)Tree.Add(this._enclosing._ids, new IdSlotMapping
					(((TreeInt)slotChange)._key, ((SlotChange)slotChange).NewSlot())));
			}

			private readonly InMemoryIdSystem _enclosing;
		}

		private void WriteThis()
		{
			int slotLength = TreeInt.MarshalledLength(_ids) + Const4.IntLength;
			_slot = _container.AllocateSlot(slotLength);
			ByteArrayBuffer buffer = new ByteArrayBuffer(_slot.Length());
			buffer.WriteInt(_idGenerator);
			TreeInt.Write(buffer, _ids);
			_container.WriteBytes(buffer, _slot.Address(), 0);
			_container.SyncFiles();
			_container.WriteTransactionPointer(_slot.Address(), _slot.Length());
			_container.SyncFiles();
			_container.SystemData().TransactionPointer1(_slot.Address());
			_container.SystemData().TransactionPointer2(_slot.Length());
		}

		public virtual Slot CommittedSlot(int id)
		{
			IdSlotMapping idSlotMapping = (IdSlotMapping)Tree.Find(_ids, new TreeInt(id));
			if (idSlotMapping == null)
			{
				throw new InvalidIDException(id);
			}
			return idSlotMapping.Slot();
		}

		public virtual void CompleteInterruptedTransaction(int address, int length)
		{
		}

		// do nothing
		public virtual int NewId()
		{
			return ++_idGenerator;
		}

		public virtual void ReturnUnusedIds(IVisitable visitable)
		{
		}
		// TODO Auto-generated method stub
	}
}
