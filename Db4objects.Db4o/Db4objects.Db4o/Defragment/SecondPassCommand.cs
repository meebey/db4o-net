namespace Db4objects.Db4o.Defragment
{
	/// <summary>
	/// Second step in the defragmenting process: Fills in target file pointer slots, copies
	/// content slots from source to target and triggers ID remapping therein by calling the
	/// appropriate yap/marshaller defrag() implementations.
	/// </summary>
	/// <remarks>
	/// Second step in the defragmenting process: Fills in target file pointer slots, copies
	/// content slots from source to target and triggers ID remapping therein by calling the
	/// appropriate yap/marshaller defrag() implementations. During the process, the actual address
	/// mappings for the content slots are registered for use with string indices.
	/// </remarks>
	/// <exclude></exclude>
	internal sealed class SecondPassCommand : Db4objects.Db4o.Defragment.IPassCommand
	{
		public void ProcessClass(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.YapClass
			 yapClass, int id, int classIndexID)
		{
			if (context.MappedID(id, -1) == -1)
			{
				Sharpen.Runtime.Err.WriteLine("MAPPING NOT FOUND: " + id);
			}
			Db4objects.Db4o.ReaderPair.ProcessCopy(context, id, new _AnonymousInnerClass22(this
				, yapClass, classIndexID));
		}

		private sealed class _AnonymousInnerClass22 : Db4objects.Db4o.ISlotCopyHandler
		{
			public _AnonymousInnerClass22(SecondPassCommand _enclosing, Db4objects.Db4o.YapClass
				 yapClass, int classIndexID)
			{
				this._enclosing = _enclosing;
				this.yapClass = yapClass;
				this.classIndexID = classIndexID;
			}

			public void ProcessCopy(Db4objects.Db4o.ReaderPair readers)
			{
				yapClass.DefragClass(readers, classIndexID);
			}

			private readonly SecondPassCommand _enclosing;

			private readonly Db4objects.Db4o.YapClass yapClass;

			private readonly int classIndexID;
		}

		public void ProcessObjectSlot(Db4objects.Db4o.Defragment.DefragContextImpl context
			, Db4objects.Db4o.YapClass yapClass, int id, bool registerAddresses)
		{
			Db4objects.Db4o.ReaderPair.ProcessCopy(context, id, new _AnonymousInnerClass30(this
				), registerAddresses);
		}

		private sealed class _AnonymousInnerClass30 : Db4objects.Db4o.ISlotCopyHandler
		{
			public _AnonymousInnerClass30(SecondPassCommand _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ProcessCopy(Db4objects.Db4o.ReaderPair readers)
			{
				Db4objects.Db4o.YapClass.DefragObject(readers);
			}

			private readonly SecondPassCommand _enclosing;
		}

		public void ProcessClassCollection(Db4objects.Db4o.Defragment.DefragContextImpl context
			)
		{
			Db4objects.Db4o.ReaderPair.ProcessCopy(context, context.SourceClassCollectionID()
				, new _AnonymousInnerClass38(this));
		}

		private sealed class _AnonymousInnerClass38 : Db4objects.Db4o.ISlotCopyHandler
		{
			public _AnonymousInnerClass38(SecondPassCommand _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ProcessCopy(Db4objects.Db4o.ReaderPair readers)
			{
				Db4objects.Db4o.YapClassCollection.Defrag(readers);
			}

			private readonly SecondPassCommand _enclosing;
		}

		public void ProcessBTree(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.Inside.Btree.BTree
			 btree)
		{
			btree.DefragBTree(context);
		}

		public void Flush(Db4objects.Db4o.Defragment.DefragContextImpl context)
		{
		}
	}
}
