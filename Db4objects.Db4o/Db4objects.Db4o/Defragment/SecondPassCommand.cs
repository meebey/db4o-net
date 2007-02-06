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
		private readonly int _objectCommitFrequency;

		private int _objectCount = 0;

		public SecondPassCommand(int objectCommitFrequency)
		{
			_objectCommitFrequency = objectCommitFrequency;
		}

		public void ProcessClass(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.Internal.ClassMetadata
			 yapClass, int id, int classIndexID)
		{
			if (context.MappedID(id, -1) == -1)
			{
				Sharpen.Runtime.Err.WriteLine("MAPPING NOT FOUND: " + id);
			}
			Db4objects.Db4o.Internal.ReaderPair.ProcessCopy(context, id, new _AnonymousInnerClass32
				(this, yapClass, classIndexID));
		}

		private sealed class _AnonymousInnerClass32 : Db4objects.Db4o.Internal.ISlotCopyHandler
		{
			public _AnonymousInnerClass32(SecondPassCommand _enclosing, Db4objects.Db4o.Internal.ClassMetadata
				 yapClass, int classIndexID)
			{
				this._enclosing = _enclosing;
				this.yapClass = yapClass;
				this.classIndexID = classIndexID;
			}

			public void ProcessCopy(Db4objects.Db4o.Internal.ReaderPair readers)
			{
				yapClass.DefragClass(readers, classIndexID);
			}

			private readonly SecondPassCommand _enclosing;

			private readonly Db4objects.Db4o.Internal.ClassMetadata yapClass;

			private readonly int classIndexID;
		}

		public void ProcessObjectSlot(Db4objects.Db4o.Defragment.DefragContextImpl context
			, Db4objects.Db4o.Internal.ClassMetadata yapClass, int id, bool registerAddresses
			)
		{
			Db4objects.Db4o.Internal.ReaderPair.ProcessCopy(context, id, new _AnonymousInnerClass40
				(this, context), registerAddresses);
		}

		private sealed class _AnonymousInnerClass40 : Db4objects.Db4o.Internal.ISlotCopyHandler
		{
			public _AnonymousInnerClass40(SecondPassCommand _enclosing, Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public void ProcessCopy(Db4objects.Db4o.Internal.ReaderPair readers)
			{
				Db4objects.Db4o.Internal.ClassMetadata.DefragObject(readers);
				if (this._enclosing._objectCommitFrequency > 0)
				{
					this._enclosing._objectCount++;
					if (this._enclosing._objectCount == this._enclosing._objectCommitFrequency)
					{
						context.TargetCommit();
						this._enclosing._objectCount = 0;
					}
				}
			}

			private readonly SecondPassCommand _enclosing;

			private readonly Db4objects.Db4o.Defragment.DefragContextImpl context;
		}

		public void ProcessClassCollection(Db4objects.Db4o.Defragment.DefragContextImpl context
			)
		{
			Db4objects.Db4o.Internal.ReaderPair.ProcessCopy(context, context.SourceClassCollectionID
				(), new _AnonymousInnerClass55(this));
		}

		private sealed class _AnonymousInnerClass55 : Db4objects.Db4o.Internal.ISlotCopyHandler
		{
			public _AnonymousInnerClass55(SecondPassCommand _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ProcessCopy(Db4objects.Db4o.Internal.ReaderPair readers)
			{
				Db4objects.Db4o.Internal.ClassMetadataRepository.Defrag(readers);
			}

			private readonly SecondPassCommand _enclosing;
		}

		public void ProcessBTree(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.Internal.Btree.BTree
			 btree)
		{
			btree.DefragBTree(context);
		}

		public void Flush(Db4objects.Db4o.Defragment.DefragContextImpl context)
		{
		}
	}
}
