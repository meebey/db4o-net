/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;

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
	internal sealed class SecondPassCommand : IPassCommand
	{
		protected readonly int _objectCommitFrequency;

		protected int _objectCount = 0;

		public SecondPassCommand(int objectCommitFrequency)
		{
			_objectCommitFrequency = objectCommitFrequency;
		}

		public void ProcessClass(DefragContextImpl context, ClassMetadata yapClass, int id
			, int classIndexID)
		{
			if (context.MappedID(id, -1) == -1)
			{
				Sharpen.Runtime.Err.WriteLine("MAPPING NOT FOUND: " + id);
			}
			ReaderPair.ProcessCopy(context, id, new _ISlotCopyHandler_34(this, yapClass, classIndexID
				));
		}

		private sealed class _ISlotCopyHandler_34 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_34(SecondPassCommand _enclosing, ClassMetadata yapClass, 
				int classIndexID)
			{
				this._enclosing = _enclosing;
				this.yapClass = yapClass;
				this.classIndexID = classIndexID;
			}

			public void ProcessCopy(ReaderPair readers)
			{
				yapClass.DefragClass(readers, classIndexID);
			}

			private readonly SecondPassCommand _enclosing;

			private readonly ClassMetadata yapClass;

			private readonly int classIndexID;
		}

		public void ProcessObjectSlot(DefragContextImpl context, ClassMetadata yapClass, 
			int id, bool registerAddresses)
		{
			ReaderPair.ProcessCopy(context, id, new _ISlotCopyHandler_42(this, context), registerAddresses
				);
		}

		private sealed class _ISlotCopyHandler_42 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_42(SecondPassCommand _enclosing, DefragContextImpl context
				)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public void ProcessCopy(ReaderPair readers)
			{
				ClassMetadata.DefragObject(readers);
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

			private readonly DefragContextImpl context;
		}

		public void ProcessClassCollection(DefragContextImpl context)
		{
			ReaderPair.ProcessCopy(context, context.SourceClassCollectionID(), new _ISlotCopyHandler_57
				(this));
		}

		private sealed class _ISlotCopyHandler_57 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_57(SecondPassCommand _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ProcessCopy(ReaderPair readers)
			{
				ClassMetadataRepository.Defrag(readers);
			}

			private readonly SecondPassCommand _enclosing;
		}

		public void ProcessBTree(DefragContextImpl context, BTree btree)
		{
			btree.DefragBTree(context);
		}

		public void Flush(DefragContextImpl context)
		{
		}
	}
}
