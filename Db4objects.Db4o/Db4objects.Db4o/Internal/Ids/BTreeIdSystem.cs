/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class BTreeIdSystem : IIdSystem
	{
		private readonly LocalObjectContainer _container;

		private readonly ITransactionalIdSystem _transactionalIdSystem;

		private readonly SequentialIdGenerator _idGenerator;

		private BTree _bTree;

		private PersistentIntegerArray _persistentState;

		public BTreeIdSystem(LocalObjectContainer container, ITransactionalIdSystem transactionalIdSystem
			, int maxValidId)
		{
			_container = container;
			_transactionalIdSystem = transactionalIdSystem;
			int persistentArrayId = SystemData().IdSystemID();
			if (persistentArrayId == 0)
			{
				InitializeNew();
			}
			else
			{
				InitializeExisting(persistentArrayId);
			}
			_idGenerator = new SequentialIdGenerator(new _IFunction4_39(this), IdGeneratorValue
				(), _container.Handlers.LowestValidId(), maxValidId);
		}

		private sealed class _IFunction4_39 : IFunction4
		{
			public _IFunction4_39(BTreeIdSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object start)
			{
				return this._enclosing.FindFreeId((((int)start)));
			}

			private readonly BTreeIdSystem _enclosing;
		}

		public BTreeIdSystem(LocalObjectContainer container, IIdSystem idSystem) : this(container
			, container.NewTransactionalIdSystem(null, new _IClosure4_47(idSystem)), int.MaxValue
			)
		{
		}

		private sealed class _IClosure4_47 : IClosure4
		{
			public _IClosure4_47(IIdSystem idSystem)
			{
				this.idSystem = idSystem;
			}

			public object Run()
			{
				return idSystem;
			}

			private readonly IIdSystem idSystem;
		}

		private void InitializeExisting(int persistentArrayId)
		{
			_persistentState = new PersistentIntegerArray(_transactionalIdSystem, persistentArrayId
				);
			_persistentState.Read(Transaction());
			_bTree = new BTree(Transaction(), BTreeConfiguration(), BTreeId(), new BTreeIdSystem.IdSlotMappingHandler
				());
		}

		private Db4objects.Db4o.Internal.Btree.BTreeConfiguration BTreeConfiguration()
		{
			return new Db4objects.Db4o.Internal.Btree.BTreeConfiguration(_transactionalIdSystem
				, SlotChangeFactory.FreeSpace, 64, false);
		}

		private int IdGeneratorValue()
		{
			return _persistentState.Array()[1];
		}

		private void IdGeneratorValue(int value)
		{
			_persistentState.Array()[1] = value;
		}

		private int BTreeId()
		{
			return _persistentState.Array()[0];
		}

		private Db4objects.Db4o.Internal.SystemData SystemData()
		{
			return _container.SystemData();
		}

		private void InitializeNew()
		{
			_bTree = new BTree(Transaction(), BTreeConfiguration(), new BTreeIdSystem.IdSlotMappingHandler
				());
			int idGeneratorValue = _container.Handlers.LowestValidId() - 1;
			_persistentState = new PersistentIntegerArray(_transactionalIdSystem, new int[] { 
				_bTree.GetID(), idGeneratorValue });
			_persistentState.Write(Transaction());
			SystemData().IdSystemID(_persistentState.GetID());
		}

		private int FindFreeId(int start)
		{
			throw new NotImplementedException();
		}

		public virtual void Close()
		{
		}

		public virtual Slot CommittedSlot(int id)
		{
			IdSlotMapping mapping = (IdSlotMapping)_bTree.Search(Transaction(), new IdSlotMapping
				(id, 0, 0));
			if (mapping == null)
			{
				throw new InvalidIDException(id);
			}
			return mapping.Slot();
		}

		public virtual void CompleteInterruptedTransaction(int transactionId1, int transactionId2
			)
		{
		}

		// do nothing
		public virtual int NewId()
		{
			int id = _idGenerator.NewId();
			_bTree.Add(Transaction(), new IdSlotMapping(id, 0, 0));
			return id;
		}

		private Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _container.SystemTransaction();
		}

		public virtual void Commit(IVisitable slotChanges, FreespaceCommitter freespaceCommitter
			)
		{
			_container.FreespaceManager().BeginCommit();
			slotChanges.Accept(new _IVisitor4_126(this));
			// TODO: Maybe we want a BTree that doesn't allow duplicates.
			_bTree.Commit(Transaction());
			IdGeneratorValue(_idGenerator.PersistentGeneratorValue());
			if (_idGenerator.IsDirty())
			{
				_idGenerator.SetClean();
				_persistentState.SetStateDirty();
				_persistentState.Write(Transaction());
			}
			_container.FreespaceManager().EndCommit();
			_transactionalIdSystem.Commit(freespaceCommitter);
			_transactionalIdSystem.Clear();
		}

		private sealed class _IVisitor4_126 : IVisitor4
		{
			public _IVisitor4_126(BTreeIdSystem _enclosing)
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
					this._enclosing._bTree.Remove(this._enclosing.Transaction(), new IdSlotMapping(((
						TreeInt)slotChange)._key, 0, 0));
					return;
				}
				this._enclosing._bTree.Remove(this._enclosing.Transaction(), new IdSlotMapping(((
					TreeInt)slotChange)._key, 0, 0));
				this._enclosing._bTree.Add(this._enclosing.Transaction(), new IdSlotMapping(((TreeInt
					)slotChange)._key, ((SlotChange)slotChange).NewSlot()));
				if (DTrace.enabled)
				{
					DTrace.SlotMapped.LogLength(((TreeInt)slotChange)._key, ((SlotChange)slotChange).
						NewSlot());
				}
			}

			private readonly BTreeIdSystem _enclosing;
		}

		public virtual void ReturnUnusedIds(IVisitable visitable)
		{
			visitable.Accept(new _IVisitor4_162(this));
		}

		private sealed class _IVisitor4_162 : IVisitor4
		{
			public _IVisitor4_162(BTreeIdSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object id)
			{
				this._enclosing._bTree.Remove(this._enclosing.Transaction(), new IdSlotMapping(((
					(int)id)), 0, 0));
			}

			private readonly BTreeIdSystem _enclosing;
		}

		public class IdSlotMappingHandler : IIndexable4
		{
			public virtual void DefragIndexEntry(DefragmentContextImpl context)
			{
				throw new NotImplementedException();
			}

			public virtual object ReadIndexEntry(IContext context, ByteArrayBuffer buffer)
			{
				return IdSlotMapping.Read(buffer);
			}

			public virtual void WriteIndexEntry(IContext context, ByteArrayBuffer buffer, object
				 mapping)
			{
				((IdSlotMapping)mapping).Write(buffer);
			}

			public virtual IPreparedComparison PrepareComparison(IContext context, object sourceMapping
				)
			{
				return new _IPreparedComparison_185(sourceMapping);
			}

			private sealed class _IPreparedComparison_185 : IPreparedComparison
			{
				public _IPreparedComparison_185(object sourceMapping)
				{
					this.sourceMapping = sourceMapping;
				}

				public int CompareTo(object targetMapping)
				{
					return ((IdSlotMapping)sourceMapping)._id == ((IdSlotMapping)targetMapping)._id ? 
						0 : (((IdSlotMapping)sourceMapping)._id < ((IdSlotMapping)targetMapping)._id ? -
						1 : 1);
				}

				private readonly object sourceMapping;
			}

			public int LinkLength()
			{
				return Const4.IntLength * 3;
			}
		}

		public virtual ITransactionalIdSystem FreespaceIdSystem()
		{
			return _transactionalIdSystem;
		}
	}
}
