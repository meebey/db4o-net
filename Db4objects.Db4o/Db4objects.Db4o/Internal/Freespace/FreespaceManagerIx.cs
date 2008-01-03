/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <summary>Old freespacemanager, before version 7.0.</summary>
	/// <remarks>
	/// Old freespacemanager, before version 7.0.
	/// If it is still in use freespace is dropped.
	/// <see cref="BTreeFreespaceManager">BTreeFreespaceManager</see>
	/// should be used instead.
	/// </remarks>
	public class FreespaceManagerIx : AbstractFreespaceManager
	{
		public FreespaceManagerIx(LocalObjectContainer file) : base(file)
		{
		}

		public override Slot AllocateTransactionLogSlot(int length)
		{
			throw new InvalidOperationException();
		}

		public override void FreeTransactionLogSlot(Slot slot)
		{
			throw new InvalidOperationException();
		}

		public override void BeginCommit()
		{
		}

		public override void EndCommit()
		{
		}

		public override int SlotCount()
		{
			throw new InvalidOperationException();
		}

		public override void Free(Slot slot)
		{
			throw new InvalidOperationException();
		}

		public override void FreeSelf()
		{
		}

		public override Slot GetSlot(int length)
		{
			throw new InvalidOperationException();
		}

		public override void MigrateTo(IFreespaceManager fm)
		{
		}

		public override void Traverse(IVisitor4 visitor)
		{
			throw new InvalidOperationException();
		}

		public override int OnNew(LocalObjectContainer file)
		{
			return file.EnsureFreespaceSlot();
		}

		public override void Read(int freespaceID)
		{
		}

		public override void Start(int slotAddress)
		{
		}

		public override byte SystemType()
		{
			return FmIx;
		}

		public override int Write()
		{
			return 0;
		}

		public override void Commit()
		{
		}
	}
}
