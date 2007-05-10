using System;
using System.Collections;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>
	/// First step in the defragmenting process: Allocates pointer slots in the target file for
	/// each ID (but doesn't fill them in, yet) and registers the mapping from source pointer address
	/// to target pointer address.
	/// </summary>
	/// <remarks>
	/// First step in the defragmenting process: Allocates pointer slots in the target file for
	/// each ID (but doesn't fill them in, yet) and registers the mapping from source pointer address
	/// to target pointer address.
	/// </remarks>
	/// <exclude></exclude>
	internal sealed class FirstPassCommand : IPassCommand
	{
		private const int ID_BATCH_SIZE = 4096;

		private TreeInt _ids;

		internal void Process(DefragContextImpl context, int objectID, bool isClassID)
		{
			if (BatchFull())
			{
				Flush(context);
			}
			_ids = TreeInt.Add(_ids, (isClassID ? -objectID : objectID));
		}

		private bool BatchFull()
		{
			return _ids != null && _ids.Size() == ID_BATCH_SIZE;
		}

		public void ProcessClass(DefragContextImpl context, ClassMetadata yapClass, int id
			, int classIndexID)
		{
			Process(context, id, true);
			for (int fieldIdx = 0; fieldIdx < yapClass.i_fields.Length; fieldIdx++)
			{
				FieldMetadata field = yapClass.i_fields[fieldIdx];
				if (!field.IsVirtual() && field.HasIndex())
				{
					ProcessBTree(context, field.GetIndex(context.SystemTrans()));
				}
			}
		}

		public void ProcessObjectSlot(DefragContextImpl context, ClassMetadata yapClass, 
			int sourceID, bool registerAddresses)
		{
			Process(context, sourceID, false);
		}

		public void ProcessClassCollection(DefragContextImpl context)
		{
			Process(context, context.SourceClassCollectionID(), false);
		}

		public void ProcessBTree(DefragContextImpl context, BTree btree)
		{
			Process(context, btree.GetID(), false);
			context.TraverseAllIndexSlots(btree, new _AnonymousInnerClass55(this, context));
		}

		private sealed class _AnonymousInnerClass55 : IVisitor4
		{
			public _AnonymousInnerClass55(FirstPassCommand _enclosing, DefragContextImpl context
				)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				this._enclosing.Process(context, id, false);
			}

			private readonly FirstPassCommand _enclosing;

			private readonly DefragContextImpl context;
		}

		public void Flush(DefragContextImpl context)
		{
			if (_ids == null)
			{
				return;
			}
			int blockSize = context.BlockSize();
			int blockLength = Math.Max(Const4.POINTER_LENGTH, blockSize);
			bool overlapping = (Const4.POINTER_LENGTH % blockSize > 0);
			int blocksPerPointer = Const4.POINTER_LENGTH / blockSize;
			if (overlapping)
			{
				blocksPerPointer++;
			}
			int batchSize = _ids.Size() * blockLength;
			Slot pointerSlot = context.AllocateTargetSlot(batchSize);
			int pointerAddress = pointerSlot.Address();
			IEnumerator idIter = new TreeKeyIterator(_ids);
			while (idIter.MoveNext())
			{
				int objectID = ((int)idIter.Current);
				bool isClassID = false;
				if (objectID < 0)
				{
					objectID = -objectID;
					isClassID = true;
				}
				context.MapIDs(objectID, pointerAddress, isClassID);
				pointerAddress += blocksPerPointer;
			}
			_ids = null;
		}
	}
}
