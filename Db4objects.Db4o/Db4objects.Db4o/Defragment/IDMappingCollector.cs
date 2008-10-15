/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Defragment
{
	public class IDMappingCollector
	{
		private const int IdBatchSize = 4096;

		private TreeInt _ids;

		internal virtual void CreateIDMapping(DefragmentServicesImpl context, int objectID
			, bool isClassID)
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

		public virtual void Flush(DefragmentServicesImpl context)
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
