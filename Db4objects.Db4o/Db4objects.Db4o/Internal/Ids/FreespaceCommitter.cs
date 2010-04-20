/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class FreespaceCommitter
	{
		public static readonly Db4objects.Db4o.Internal.Ids.FreespaceCommitter DoNothing = 
			new FreespaceCommitter.NullFreespaceCommitter();

		private readonly IList _idSystems = new ArrayList();

		private readonly IList _toFree = new ArrayList();

		private readonly IFreespaceManager _freespaceManager;

		public FreespaceCommitter(IFreespaceManager freespaceManager)
		{
			_freespaceManager = freespaceManager == null ? NullFreespaceManager.Instance : freespaceManager;
		}

		public virtual void Commit()
		{
			Apply();
			_freespaceManager.BeginCommit();
			_freespaceManager.Commit();
			Accumulate(true);
			Apply();
			_freespaceManager.EndCommit();
		}

		private void Apply()
		{
			for (IEnumerator slotIter = _toFree.GetEnumerator(); slotIter.MoveNext(); )
			{
				Slot slot = ((Slot)slotIter.Current);
				_freespaceManager.Free(slot);
			}
			_toFree.Clear();
		}

		private void Accumulate(bool forFreespace)
		{
			for (IEnumerator idSystemIter = _idSystems.GetEnumerator(); idSystemIter.MoveNext
				(); )
			{
				ITransactionalIdSystem idSystem = ((ITransactionalIdSystem)idSystemIter.Current);
				idSystem.AccumulateFreeSlots(this, forFreespace);
			}
		}

		public virtual void Register(ITransactionalIdSystem transactionalIdSystem)
		{
			_idSystems.Add(transactionalIdSystem);
		}

		private class NullFreespaceCommitter : FreespaceCommitter
		{
			public NullFreespaceCommitter() : base(NullFreespaceManager.Instance)
			{
			}

			public override void Register(ITransactionalIdSystem transactionalIdSystem)
			{
			}

			// do nothing
			public override void Commit()
			{
			}
			// do nothing
		}

		public virtual void DelayedFree(Slot slot)
		{
			_toFree.Add(slot);
		}
	}
}
