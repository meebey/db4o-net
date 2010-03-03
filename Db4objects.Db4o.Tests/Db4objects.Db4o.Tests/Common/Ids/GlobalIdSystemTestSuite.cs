/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Ids;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Ids
{
	public class GlobalIdSystemTestSuite : FixtureBasedTestSuite
	{
		private const int SlotLength = 10;

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(new GlobalIdSystemTestSuite()).Run();
		}

		public class GlobalIdSystemTestUnit : AbstractDb4oTestCase, IOptOutMultiSession, 
			IDb4oTestCase
		{
			/// <exception cref="System.Exception"></exception>
			protected override void Configure(IConfiguration config)
			{
				IIdSystemConfiguration idSystemConfiguration = Db4oLegacyConfigurationBridge.AsIdSystemConfiguration
					(config);
				((IProcedure4)_fixture.Value).Apply(idSystemConfiguration);
			}

			public virtual void TestPersistence()
			{
			}

			public virtual void TestSlotForNewIdDoesNotExist()
			{
				int newId = IdSystem().NewId();
				Slot oldSlot = null;
				try
				{
					oldSlot = IdSystem().CommittedSlot(newId);
				}
				catch (InvalidIDException)
				{
				}
				Assert.IsFalse(IsValid(oldSlot));
			}

			public virtual void TestSingleNewSlot()
			{
				int id = IdSystem().NewId();
				Assert.AreEqual(AllocateNewSlot(id), IdSystem().CommittedSlot(id));
			}

			public virtual void TestSingleSlotUpdate()
			{
				int id = IdSystem().NewId();
				AllocateNewSlot(id);
				SlotChange slotChange = SlotChangeFactory.UserObjects.NewInstance(id);
				Slot updatedSlot = LocalContainer().AllocateSlot(SlotLength);
				slotChange.NotifySlotUpdated(FreespaceManager(), updatedSlot);
				Commit(new SlotChange[] { slotChange });
				Assert.AreEqual(updatedSlot, IdSystem().CommittedSlot(id));
			}

			public virtual void TestSingleSlotDelete()
			{
				int id = IdSystem().NewId();
				AllocateNewSlot(id);
				SlotChange slotChange = SlotChangeFactory.UserObjects.NewInstance(id);
				slotChange.NotifyDeleted(FreespaceManager());
				Commit(new SlotChange[] { slotChange });
				Assert.IsFalse(IsValid(IdSystem().CommittedSlot(id)));
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
				IdSystem().Commit(new _IVisitable_86(slotChanges), new _IRunnable_93());
			}

			private sealed class _IVisitable_86 : IVisitable
			{
				public _IVisitable_86(SlotChange[] slotChanges)
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

			private sealed class _IRunnable_93 : IRunnable
			{
				public _IRunnable_93()
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

			private IIdSystem IdSystem()
			{
				return LocalContainer().GlobalIdSystem();
			}
		}

		private static FixtureVariable _fixture = FixtureVariable.NewInstance("IdSystem");

		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[] { new Db4oFixtureProvider(), new SimpleFixtureProvider
				(_fixture, new IProcedure4[] { new _IProcedure4_125(), new _IProcedure4_129() })
				 };
		}

		private sealed class _IProcedure4_125 : IProcedure4
		{
			public _IProcedure4_125()
			{
			}

			public void Apply(object idSystemConfiguration)
			{
				((IIdSystemConfiguration)idSystemConfiguration).UsePointerBasedSystem();
			}
		}

		private sealed class _IProcedure4_129 : IProcedure4
		{
			public _IProcedure4_129()
			{
			}

			public void Apply(object idSystemConfiguration)
			{
				((IIdSystemConfiguration)idSystemConfiguration).UseInMemorySystem();
			}
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(GlobalIdSystemTestSuite.GlobalIdSystemTestUnit) };
		}
	}
}
