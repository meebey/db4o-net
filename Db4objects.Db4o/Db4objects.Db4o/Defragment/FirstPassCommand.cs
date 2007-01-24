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
	internal sealed class FirstPassCommand : Db4objects.Db4o.Defragment.IPassCommand
	{
		private const int ID_BATCH_SIZE = 4096;

		private Db4objects.Db4o.TreeInt _ids;

		internal void Process(Db4objects.Db4o.Defragment.DefragContextImpl context, int objectID
			, bool isClassID)
		{
			if (BatchFull())
			{
				Flush(context);
			}
			_ids = Db4objects.Db4o.TreeInt.Add(_ids, (isClassID ? -objectID : objectID));
		}

		private bool BatchFull()
		{
			return _ids != null && _ids.Size() == ID_BATCH_SIZE;
		}

		public void ProcessClass(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.YapClass
			 yapClass, int id, int classIndexID)
		{
			Process(context, id, true);
			for (int fieldIdx = 0; fieldIdx < yapClass.i_fields.Length; fieldIdx++)
			{
				Db4objects.Db4o.YapField field = yapClass.i_fields[fieldIdx];
				if (!field.IsVirtual() && field.HasIndex())
				{
					ProcessBTree(context, field.GetIndex(context.SystemTrans()));
				}
			}
		}

		public void ProcessObjectSlot(Db4objects.Db4o.Defragment.DefragContextImpl context
			, Db4objects.Db4o.YapClass yapClass, int sourceID, bool registerAddresses)
		{
			Process(context, sourceID, false);
		}

		public void ProcessClassCollection(Db4objects.Db4o.Defragment.DefragContextImpl context
			)
		{
			Process(context, context.SourceClassCollectionID(), false);
		}

		public void ProcessBTree(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.Inside.Btree.BTree
			 btree)
		{
			Process(context, btree.GetID(), false);
			context.TraverseAllIndexSlots(btree, new _AnonymousInnerClass53(this, context));
		}

		private sealed class _AnonymousInnerClass53 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass53(FirstPassCommand _enclosing, Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
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

			private readonly Db4objects.Db4o.Defragment.DefragContextImpl context;
		}

		public void Flush(Db4objects.Db4o.Defragment.DefragContextImpl context)
		{
			if (_ids == null)
			{
				return;
			}
			int pointerAddress = context.AllocateTargetSlot(_ids.Size() * Db4objects.Db4o.YapConst
				.POINTER_LENGTH);
			System.Collections.IEnumerator idIter = new Db4objects.Db4o.Foundation.TreeKeyIterator
				(_ids);
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
				pointerAddress += Db4objects.Db4o.YapConst.POINTER_LENGTH;
			}
			_ids = null;
		}
	}
}
