/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
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
		private const int IdBatchSize = 4096;

		private TreeInt _ids;

		internal void Process(DefragmentServicesImpl context, int objectID, bool isClassID
			)
		{
			if (BatchFull())
			{
				Flush(context);
			}
			_ids = TreeInt.Add(_ids, (isClassID ? -objectID : objectID));
		}

		private bool BatchFull()
		{
			return _ids != null && _ids.Size() == IdBatchSize;
		}

		public void ProcessClass(DefragmentServicesImpl context, ClassMetadata yapClass, 
			int id, int classIndexID)
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

		public void ProcessObjectSlot(DefragmentServicesImpl context, ClassMetadata yapClass
			, int sourceID)
		{
			Process(context, sourceID, false);
		}

		/// <exception cref="CorruptionException"></exception>
		public void ProcessClassCollection(DefragmentServicesImpl context)
		{
			Process(context, context.SourceClassCollectionID(), false);
		}

		public void ProcessBTree(DefragmentServicesImpl context, BTree btree)
		{
			Process(context, btree.GetID(), false);
			context.TraverseAllIndexSlots(btree, new _IVisitor4_54(this, context));
		}

		private sealed class _IVisitor4_54 : IVisitor4
		{
			public _IVisitor4_54(FirstPassCommand _enclosing, DefragmentServicesImpl context)
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

			private readonly DefragmentServicesImpl context;
		}

		public void Flush(DefragmentServicesImpl context)
		{
			if (_ids == null)
			{
				return;
			}
			int blockSize = context.BlockSize();
			bool overlapping = (Const4.PointerLength % blockSize > 0);
			int blocksPerPointer = Const4.PointerLength / blockSize;
			if (overlapping)
			{
				blocksPerPointer++;
			}
			int bytesPerPointer = blocksPerPointer * blockSize;
			int batchSize = _ids.Size() * bytesPerPointer;
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
				// seen object ids don't come by here anymore - any other candidates?
				context.MapIDs(objectID, pointerAddress, isClassID);
				pointerAddress += blocksPerPointer;
			}
			_ids = null;
		}
	}
}
