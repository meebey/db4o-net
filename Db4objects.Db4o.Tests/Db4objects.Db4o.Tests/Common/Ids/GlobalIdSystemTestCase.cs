/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Ids;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Ids
{
	public class GlobalIdSystemTestCase : AbstractDb4oTestCase, IOptOutMultiSession
	{
		private const int SlotLength = 10;

		public static void Main(string[] args)
		{
			new GlobalIdSystemTestCase().RunSolo();
		}

		private IGlobalIdSystem _idSystem;

		/// <exception cref="System.Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			_idSystem = new PointerBasedIdSystem(LocalContainer());
		}

		public virtual void TestSlotForNewIdDoesNotExist()
		{
			int newId = _idSystem.NewId();
			Slot oldSlot = _idSystem.CommittedSlot(newId);
			Assert.IsFalse(IsValid(oldSlot));
		}

		public virtual void TestSingleNewSlot()
		{
			int id = _idSystem.NewId();
			Assert.AreEqual(AllocateNewSlot(id), _idSystem.CommittedSlot(id));
		}

		public virtual void TestSingleSlotUpdate()
		{
			int id = _idSystem.NewId();
			AllocateNewSlot(id);
			SlotChange slotChange = SlotChangeFactory.UserObjects.NewInstance(id);
			Slot updatedSlot = LocalContainer().AllocateSlot(SlotLength);
			slotChange.NotifySlotUpdated(FreespaceManager(), updatedSlot);
			Commit(new SlotChange[] { slotChange });
			Assert.AreEqual(updatedSlot, _idSystem.CommittedSlot(id));
		}

		public virtual void TestSingleSlotDelete()
		{
			int id = _idSystem.NewId();
			AllocateNewSlot(id);
			SlotChange slotChange = SlotChangeFactory.UserObjects.NewInstance(id);
			slotChange.NotifyDeleted(FreespaceManager());
			Commit(new SlotChange[] { slotChange });
			Assert.IsFalse(IsValid(_idSystem.CommittedSlot(id)));
		}

		private Slot AllocateNewSlot(int newId)
		{
			SlotChange slotChange = SlotChangeFactory.UserObjects.NewInstance(newId);
			Slot allocatedSlot = LocalContainer().AllocateSlot(SlotLength);
			slotChange.NotifySlotCreated(allocatedSlot);
			Commit(new SlotChange[] { slotChange });
			return allocatedSlot;
		}

		private void Commit(SlotChange[] slotChanges)
		{
			_idSystem.Commit(new _IVisitable_73(slotChanges), new _IRunnable_80());
		}

		private sealed class _IVisitable_73 : IVisitable
		{
			public _IVisitable_73(SlotChange[] slotChanges)
			{
				this.slotChanges = slotChanges;
			}

			public void Accept(IVisitor4 visitor)
			{
				for (int slotChangeIndex = 0; slotChangeIndex < slotChanges.Length; ++slotChangeIndex)
				{
					SlotChange slotChange = slotChanges[slotChangeIndex];
					visitor.Visit(slotChange);
				}
			}

			private readonly SlotChange[] slotChanges;
		}

		private sealed class _IRunnable_80 : IRunnable
		{
			public _IRunnable_80()
			{
			}

			public void Run()
			{
			}
		}

		// do nothing
		private LocalObjectContainer LocalContainer()
		{
			return (LocalObjectContainer)Container();
		}

		private bool IsValid(Slot slot)
		{
			return slot != null && !slot.IsNull();
		}

		private IFreespaceManager FreespaceManager()
		{
			return LocalContainer().FreespaceManager();
		}
	}
}
