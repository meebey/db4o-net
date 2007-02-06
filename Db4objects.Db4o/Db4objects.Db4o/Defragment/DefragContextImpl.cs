namespace Db4objects.Db4o.Defragment
{
	/// <exclude></exclude>
	public class DefragContextImpl : Db4objects.Db4o.Internal.Mapping.IDefragContext
	{
		public abstract class DbSelector
		{
			internal DbSelector()
			{
			}

			internal abstract Db4objects.Db4o.Internal.LocalObjectContainer Db(Db4objects.Db4o.Defragment.DefragContextImpl
				 context);

			internal virtual Db4objects.Db4o.Internal.Transaction Transaction(Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
			{
				return Db(context).GetSystemTransaction();
			}
		}

		private sealed class _AnonymousInnerClass33 : Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
		{
			public _AnonymousInnerClass33()
			{
			}

			internal override Db4objects.Db4o.Internal.LocalObjectContainer Db(Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
			{
				return context._sourceDb;
			}
		}

		public static readonly Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector SOURCEDB
			 = new _AnonymousInnerClass33();

		private sealed class _AnonymousInnerClass39 : Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
		{
			public _AnonymousInnerClass39()
			{
			}

			internal override Db4objects.Db4o.Internal.LocalObjectContainer Db(Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
			{
				return context._targetDb;
			}
		}

		public static readonly Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector TARGETDB
			 = new _AnonymousInnerClass39();

		private const long CLASSCOLLECTION_POINTER_ADDRESS = 2 + 2 * Db4objects.Db4o.Internal.Const4
			.INT_LENGTH;

		public readonly Db4objects.Db4o.Internal.LocalObjectContainer _sourceDb;

		internal readonly Db4objects.Db4o.Internal.LocalObjectContainer _targetDb;

		private readonly Db4objects.Db4o.Defragment.IContextIDMapping _mapping;

		private Db4objects.Db4o.Defragment.IDefragmentListener _listener;

		private Db4objects.Db4o.Foundation.Queue4 _unindexed = new Db4objects.Db4o.Foundation.Queue4
			();

		public DefragContextImpl(Db4objects.Db4o.Defragment.DefragmentConfig defragConfig
			, Db4objects.Db4o.Defragment.IDefragmentListener listener)
		{
			_listener = listener;
			Db4objects.Db4o.Config.IConfiguration sourceConfig = defragConfig.Db4oConfig();
			sourceConfig.WeakReferences(false);
			sourceConfig.FlushFileBuffers(false);
			sourceConfig.ReadOnly(true);
			_sourceDb = (Db4objects.Db4o.Internal.LocalObjectContainer)Db4objects.Db4o.Db4oFactory
				.OpenFile(sourceConfig, defragConfig.BackupPath()).Ext();
			_targetDb = FreshYapFile(defragConfig.OrigPath());
			_mapping = defragConfig.Mapping();
			_mapping.Open();
		}

		internal static Db4objects.Db4o.Internal.LocalObjectContainer FreshYapFile(string
			 fileName)
		{
			new Sharpen.IO.File(fileName).Delete();
			return (Db4objects.Db4o.Internal.LocalObjectContainer)Db4objects.Db4o.Db4oFactory
				.OpenFile(Db4objects.Db4o.Defragment.DefragmentConfig.VanillaDb4oConfig(), fileName
				).Ext();
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
				throw new Db4objects.Db4o.Internal.Mapping.MappingNotFoundException(oldID);
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
				_listener.NotifyDefragmentInfo(new Db4objects.Db4o.Defragment.DefragmentInfo("No mapping found for ID "
					 + id));
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

		public virtual Db4objects.Db4o.Internal.Buffer ReaderByID(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector, int id)
		{
			Db4objects.Db4o.Internal.Slots.Slot slot = ReadPointer(selector, id);
			return ReaderByAddress(selector, slot._address, slot._length);
		}

		public virtual Db4objects.Db4o.Internal.StatefulBuffer SourceWriterByID(int id)
		{
			Db4objects.Db4o.Internal.Slots.Slot slot = ReadPointer(SOURCEDB, id);
			return _sourceDb.ReadWriterByAddress(SOURCEDB.Transaction(this), slot._address, slot
				._length);
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

		public virtual Db4objects.Db4o.Internal.Buffer ReaderByAddress(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector, int address, int length)
		{
			return selector.Db(this).ReadReaderByAddress(address, length);
		}

		public virtual Db4objects.Db4o.Internal.StatefulBuffer TargetWriterByAddress(int 
			address, int length)
		{
			return _targetDb.ReadWriterByAddress(TARGETDB.Transaction(this), address, length);
		}

		public virtual int AllocateTargetSlot(int length)
		{
			return _targetDb.GetSlot(length);
		}

		public virtual void TargetWriteBytes(Db4objects.Db4o.Internal.ReaderPair readers, 
			int address)
		{
			readers.Write(_targetDb, address);
		}

		public virtual void TargetWriteBytes(Db4objects.Db4o.Internal.Buffer reader, int 
			address)
		{
			_targetDb.WriteBytes(reader, address, 0);
		}

		public virtual Db4objects.Db4o.Ext.IStoredClass[] StoredClasses(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector)
		{
			Db4objects.Db4o.Internal.LocalObjectContainer db = selector.Db(this);
			db.ShowInternalClasses(true);
			Db4objects.Db4o.Ext.IStoredClass[] classes = db.StoredClasses();
			return classes;
		}

		public virtual Db4objects.Db4o.Internal.LatinStringIO StringIO()
		{
			return _sourceDb.StringIO();
		}

		public virtual void TargetCommit()
		{
			_targetDb.Commit();
		}

		public virtual Db4objects.Db4o.Internal.ITypeHandler4 SourceHandler(int id)
		{
			return _sourceDb.HandlerByID(id);
		}

		public virtual int SourceClassCollectionID()
		{
			return _sourceDb.ClassCollection().GetID();
		}

		public static void TargetClassCollectionID(string file, int id)
		{
			Sharpen.IO.RandomAccessFile raf = new Sharpen.IO.RandomAccessFile(file, "rw");
			try
			{
				Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(Db4objects.Db4o.Internal.Const4
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

		private Db4objects.Db4o.Foundation.Hashtable4 _classIndices = new Db4objects.Db4o.Foundation.Hashtable4
			(16);

		public virtual int ClassIndexID(Db4objects.Db4o.Internal.ClassMetadata yapClass)
		{
			return ClassIndex(yapClass).Id();
		}

		public virtual void TraverseAll(Db4objects.Db4o.Internal.ClassMetadata yapClass, 
			Db4objects.Db4o.Foundation.IVisitor4 command)
		{
			if (!yapClass.HasIndex())
			{
				return;
			}
			yapClass.Index().TraverseAll(SOURCEDB.Transaction(this), command);
		}

		public virtual void TraverseAllIndexSlots(Db4objects.Db4o.Internal.ClassMetadata 
			yapClass, Db4objects.Db4o.Foundation.IVisitor4 command)
		{
			System.Collections.IEnumerator slotIDIter = yapClass.Index().AllSlotIDs(SOURCEDB.
				Transaction(this));
			while (slotIDIter.MoveNext())
			{
				command.Visit(slotIDIter.Current);
			}
		}

		public virtual void TraverseAllIndexSlots(Db4objects.Db4o.Internal.Btree.BTree btree
			, Db4objects.Db4o.Foundation.IVisitor4 command)
		{
			System.Collections.IEnumerator slotIDIter = btree.AllNodeIds(SOURCEDB.Transaction
				(this));
			while (slotIDIter.MoveNext())
			{
				command.Visit(slotIDIter.Current);
			}
		}

		public virtual int DatabaseIdentityID(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector)
		{
			Db4objects.Db4o.Internal.LocalObjectContainer db = selector.Db(this);
			Db4objects.Db4o.Ext.Db4oDatabase identity = db.Identity();
			if (identity == null)
			{
				return 0;
			}
			return identity.GetID(selector.Transaction(this));
		}

		private Db4objects.Db4o.Internal.Classindex.IClassIndexStrategy ClassIndex(Db4objects.Db4o.Internal.ClassMetadata
			 yapClass)
		{
			Db4objects.Db4o.Internal.Classindex.IClassIndexStrategy classIndex = (Db4objects.Db4o.Internal.Classindex.IClassIndexStrategy
				)_classIndices.Get(yapClass);
			if (classIndex == null)
			{
				classIndex = new Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy(yapClass
					);
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

		public virtual Db4objects.Db4o.Internal.Btree.BTree SourceUuidIndex()
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

		public virtual Db4objects.Db4o.Internal.ClassMetadata YapClass(int id)
		{
			return _sourceDb.GetYapClass(id);
		}

		public virtual void RegisterUnindexed(int id)
		{
			_unindexed.Add(id);
		}

		public virtual System.Collections.IEnumerator UnindexedIDs()
		{
			return _unindexed.Iterator();
		}

		private Db4objects.Db4o.Internal.Slots.Slot ReadPointer(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector, int id)
		{
			Db4objects.Db4o.Internal.Buffer reader = ReaderByAddress(selector, id, Db4objects.Db4o.Internal.Const4
				.POINTER_LENGTH);
			int address = reader.ReadInt();
			int length = reader.ReadInt();
			return new Db4objects.Db4o.Internal.Slots.Slot(address, length);
		}
	}
}
