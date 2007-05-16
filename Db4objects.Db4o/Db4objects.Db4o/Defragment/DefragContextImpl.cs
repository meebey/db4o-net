/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.IO;

namespace Db4objects.Db4o.Defragment
{
	/// <exclude></exclude>
	public class DefragContextImpl : IDefragContext
	{
		public abstract class DbSelector
		{
			internal DbSelector()
			{
			}

			internal abstract LocalObjectContainer Db(DefragContextImpl context);

			internal virtual Db4objects.Db4o.Internal.Transaction Transaction(DefragContextImpl
				 context)
			{
				return Db(context).SystemTransaction();
			}
		}

		private sealed class _AnonymousInnerClass33 : DefragContextImpl.DbSelector
		{
			public _AnonymousInnerClass33()
			{
			}

			internal override LocalObjectContainer Db(DefragContextImpl context)
			{
				return context._sourceDb;
			}
		}

		public static readonly DefragContextImpl.DbSelector SOURCEDB = new _AnonymousInnerClass33
			();

		private sealed class _AnonymousInnerClass39 : DefragContextImpl.DbSelector
		{
			public _AnonymousInnerClass39()
			{
			}

			internal override LocalObjectContainer Db(DefragContextImpl context)
			{
				return context._targetDb;
			}
		}

		public static readonly DefragContextImpl.DbSelector TARGETDB = new _AnonymousInnerClass39
			();

		private const long CLASSCOLLECTION_POINTER_ADDRESS = 2 + 2 * Const4.INT_LENGTH;

		public readonly LocalObjectContainer _sourceDb;

		internal readonly LocalObjectContainer _targetDb;

		private readonly IContextIDMapping _mapping;

		private IDefragmentListener _listener;

		private IQueue4 _unindexed = new NonblockingQueue();

		public DefragContextImpl(DefragmentConfig defragConfig, IDefragmentListener listener
			)
		{
			_listener = listener;
			Config4Impl originalConfig = (Config4Impl)defragConfig.Db4oConfig();
			IConfiguration sourceConfig = (IConfiguration)originalConfig.DeepClone(null);
			sourceConfig.WeakReferences(false);
			sourceConfig.FlushFileBuffers(false);
			sourceConfig.ReadOnly(true);
			_sourceDb = (LocalObjectContainer)Db4oFactory.OpenFile(sourceConfig, defragConfig
				.TempPath()).Ext();
			_targetDb = FreshYapFile(defragConfig.OrigPath(), defragConfig.BlockSize());
			_mapping = defragConfig.Mapping();
			_mapping.Open();
		}

		internal static LocalObjectContainer FreshYapFile(string fileName, int blockSize)
		{
			new Sharpen.IO.File(fileName).Delete();
			return (LocalObjectContainer)Db4oFactory.OpenFile(DefragmentConfig.VanillaDb4oConfig
				(blockSize), fileName).Ext();
		}

		public virtual int MappedID(int oldID, int defaultID)
		{
			int mapped = InternalMappedID(oldID, false);
			return (mapped != 0 ? mapped : defaultID);
		}

		public virtual int MappedID(int oldID)
		{
			int mapped = InternalMappedID(oldID, false);
			if (mapped == 0)
			{
				throw new MappingNotFoundException(oldID);
			}
			return mapped;
		}

		public virtual int MappedID(int id, bool lenient)
		{
			if (id == 0)
			{
				return 0;
			}
			int mapped = InternalMappedID(id, lenient);
			if (mapped == 0)
			{
				_listener.NotifyDefragmentInfo(new DefragmentInfo("No mapping found for ID " + id
					));
				return 0;
			}
			return mapped;
		}

		private int InternalMappedID(int oldID, bool lenient)
		{
			if (oldID == 0)
			{
				return 0;
			}
			if (_sourceDb.Handlers().IsSystemHandler(oldID))
			{
				return oldID;
			}
			return _mapping.MappedID(oldID, lenient);
		}

		public virtual void MapIDs(int oldID, int newID, bool isClassID)
		{
			_mapping.MapIDs(oldID, newID, isClassID);
		}

		public virtual void Close()
		{
			_sourceDb.Close();
			_targetDb.Close();
			_mapping.Close();
		}

		public virtual Db4objects.Db4o.Internal.Buffer ReaderByID(DefragContextImpl.DbSelector
			 selector, int id)
		{
			Slot slot = ReadPointer(selector, id);
			return ReaderByAddress(selector, slot.Address(), slot.Length());
		}

		public virtual StatefulBuffer SourceWriterByID(int id)
		{
			Slot slot = ReadPointer(SOURCEDB, id);
			return _sourceDb.ReadWriterByAddress(SOURCEDB.Transaction(this), slot.Address(), 
				slot.Length());
		}

		public virtual Db4objects.Db4o.Internal.Buffer SourceReaderByAddress(int address, 
			int length)
		{
			return ReaderByAddress(SOURCEDB, address, length);
		}

		public virtual Db4objects.Db4o.Internal.Buffer TargetReaderByAddress(int address, 
			int length)
		{
			return ReaderByAddress(TARGETDB, address, length);
		}

		public virtual Db4objects.Db4o.Internal.Buffer ReaderByAddress(DefragContextImpl.DbSelector
			 selector, int address, int length)
		{
			return selector.Db(this).BufferByAddress(address, length);
		}

		public virtual StatefulBuffer TargetWriterByAddress(int address, int length)
		{
			return _targetDb.ReadWriterByAddress(TARGETDB.Transaction(this), address, length);
		}

		public virtual Slot AllocateTargetSlot(int length)
		{
			return _targetDb.GetSlot(length);
		}

		public virtual void TargetWriteBytes(ReaderPair readers, int address)
		{
			readers.Write(_targetDb, address);
		}

		public virtual void TargetWriteBytes(Db4objects.Db4o.Internal.Buffer reader, int 
			address)
		{
			_targetDb.WriteBytes(reader, address, 0);
		}

		public virtual IStoredClass[] StoredClasses(DefragContextImpl.DbSelector selector
			)
		{
			LocalObjectContainer db = selector.Db(this);
			db.ShowInternalClasses(true);
			try
			{
				return db.StoredClasses();
			}
			finally
			{
				db.ShowInternalClasses(false);
			}
		}

		public virtual LatinStringIO StringIO()
		{
			return _sourceDb.StringIO();
		}

		public virtual void TargetCommit()
		{
			_targetDb.Commit();
		}

		public virtual ITypeHandler4 SourceHandler(int id)
		{
			return _sourceDb.HandlerByID(id);
		}

		public virtual int SourceClassCollectionID()
		{
			return _sourceDb.ClassCollection().GetID();
		}

		public static void TargetClassCollectionID(string file, int id)
		{
			RandomAccessFile raf = new RandomAccessFile(file, "rw");
			try
			{
				Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(Const4
					.INT_LENGTH);
				raf.Seek(CLASSCOLLECTION_POINTER_ADDRESS);
				reader._offset = 0;
				reader.WriteInt(id);
				raf.Write(reader._buffer);
			}
			finally
			{
				raf.Close();
			}
		}

		private Hashtable4 _classIndices = new Hashtable4(16);

		public virtual int ClassIndexID(ClassMetadata yapClass)
		{
			return ClassIndex(yapClass).Id();
		}

		public virtual void TraverseAll(ClassMetadata yapClass, IVisitor4 command)
		{
			if (!yapClass.HasIndex())
			{
				return;
			}
			yapClass.Index().TraverseAll(SOURCEDB.Transaction(this), command);
		}

		public virtual void TraverseAllIndexSlots(ClassMetadata yapClass, IVisitor4 command
			)
		{
			IEnumerator slotIDIter = yapClass.Index().AllSlotIDs(SOURCEDB.Transaction(this));
			while (slotIDIter.MoveNext())
			{
				command.Visit(slotIDIter.Current);
			}
		}

		public virtual void TraverseAllIndexSlots(BTree btree, IVisitor4 command)
		{
			IEnumerator slotIDIter = btree.AllNodeIds(SOURCEDB.Transaction(this));
			while (slotIDIter.MoveNext())
			{
				command.Visit(slotIDIter.Current);
			}
		}

		public virtual int DatabaseIdentityID(DefragContextImpl.DbSelector selector)
		{
			LocalObjectContainer db = selector.Db(this);
			Db4oDatabase identity = db.Identity();
			if (identity == null)
			{
				return 0;
			}
			return identity.GetID(selector.Transaction(this));
		}

		private IClassIndexStrategy ClassIndex(ClassMetadata yapClass)
		{
			IClassIndexStrategy classIndex = (IClassIndexStrategy)_classIndices.Get(yapClass);
			if (classIndex == null)
			{
				classIndex = new BTreeClassIndexStrategy(yapClass);
				_classIndices.Put(yapClass, classIndex);
				classIndex.Initialize(_targetDb);
			}
			return classIndex;
		}

		public virtual Db4objects.Db4o.Internal.Transaction SystemTrans()
		{
			return SOURCEDB.Transaction(this);
		}

		public virtual void CopyIdentity()
		{
			_targetDb.SetIdentity(_sourceDb.Identity());
		}

		public virtual void TargetClassCollectionID(int newClassCollectionID)
		{
			_targetDb.SystemData().ClassCollectionID(newClassCollectionID);
		}

		public virtual Db4objects.Db4o.Internal.Buffer SourceReaderByID(int sourceID)
		{
			return ReaderByID(SOURCEDB, sourceID);
		}

		public virtual BTree SourceUuidIndex()
		{
			if (SourceUuidIndexID() == 0)
			{
				return null;
			}
			return _sourceDb.GetUUIDIndex().GetIndex(SystemTrans());
		}

		public virtual void TargetUuidIndexID(int id)
		{
			_targetDb.SystemData().UuidIndexId(id);
		}

		public virtual int SourceUuidIndexID()
		{
			return _sourceDb.SystemData().UuidIndexId();
		}

		public virtual ClassMetadata YapClass(int id)
		{
			return _sourceDb.ClassMetadataForId(id);
		}

		public virtual void RegisterUnindexed(int id)
		{
			_unindexed.Add(id);
		}

		public virtual IEnumerator UnindexedIDs()
		{
			return _unindexed.Iterator();
		}

		private Slot ReadPointer(DefragContextImpl.DbSelector selector, int id)
		{
			Db4objects.Db4o.Internal.Buffer reader = ReaderByAddress(selector, id, Const4.POINTER_LENGTH
				);
			int address = reader.ReadInt();
			int length = reader.ReadInt();
			return new Slot(address, length);
		}

		public virtual int BlockSize()
		{
			return _sourceDb.Config().BlockSize();
		}
	}
}
