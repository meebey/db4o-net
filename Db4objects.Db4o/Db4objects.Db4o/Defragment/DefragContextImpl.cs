namespace Db4objects.Db4o.Defragment
{
	/// <exclude></exclude>
	public class DefragContextImpl : Db4objects.Db4o.Inside.Mapping.IDefragContext
	{
		public abstract class DbSelector
		{
			internal DbSelector()
			{
			}

			internal abstract Db4objects.Db4o.YapFile Db(Db4objects.Db4o.Defragment.DefragContextImpl
				 context);

			internal virtual Db4objects.Db4o.Transaction Transaction(Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
			{
				return Db(context).GetSystemTransaction();
			}
		}

		private sealed class _AnonymousInnerClass32 : Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
		{
			public _AnonymousInnerClass32()
			{
			}

			internal override Db4objects.Db4o.YapFile Db(Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
			{
				return context._sourceDb;
			}
		}

		public static readonly Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector SOURCEDB
			 = new _AnonymousInnerClass32();

		private sealed class _AnonymousInnerClass38 : Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
		{
			public _AnonymousInnerClass38()
			{
			}

			internal override Db4objects.Db4o.YapFile Db(Db4objects.Db4o.Defragment.DefragContextImpl
				 context)
			{
				return context._targetDb;
			}
		}

		public static readonly Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector TARGETDB
			 = new _AnonymousInnerClass38();

		private const long CLASSCOLLECTION_POINTER_ADDRESS = 2 + 2 * Db4objects.Db4o.YapConst
			.INT_LENGTH;

		public readonly Db4objects.Db4o.YapFile _sourceDb;

		internal readonly Db4objects.Db4o.YapFile _targetDb;

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
			_sourceDb = (Db4objects.Db4o.YapFile)Db4objects.Db4o.Db4oFactory.OpenFile(sourceConfig
				, defragConfig.BackupPath()).Ext();
			_targetDb = FreshYapFile(defragConfig.OrigPath());
			_mapping = defragConfig.Mapping();
			_mapping.Open();
		}

		internal static Db4objects.Db4o.YapFile FreshYapFile(string fileName)
		{
			new Sharpen.IO.File(fileName).Delete();
			return (Db4objects.Db4o.YapFile)Db4objects.Db4o.Db4oFactory.OpenFile(Db4objects.Db4o.Defragment.DefragmentConfig
				.VanillaDb4oConfig(), fileName).Ext();
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
				throw new Db4objects.Db4o.Inside.Mapping.MappingNotFoundException(oldID);
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

		public virtual Db4objects.Db4o.YapReader ReaderByID(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector, int id)
		{
			Db4objects.Db4o.Inside.Slots.Slot slot = ReadPointer(selector, id);
			return ReaderByAddress(selector, slot._address, slot._length);
		}

		public virtual Db4objects.Db4o.YapWriter SourceWriterByID(int id)
		{
			Db4objects.Db4o.Inside.Slots.Slot slot = ReadPointer(SOURCEDB, id);
			return _sourceDb.ReadWriterByAddress(SOURCEDB.Transaction(this), slot._address, slot
				._length);
		}

		public virtual Db4objects.Db4o.YapReader SourceReaderByAddress(int address, int length
			)
		{
			return ReaderByAddress(SOURCEDB, address, length);
		}

		public virtual Db4objects.Db4o.YapReader TargetReaderByAddress(int address, int length
			)
		{
			return ReaderByAddress(TARGETDB, address, length);
		}

		public virtual Db4objects.Db4o.YapReader ReaderByAddress(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector, int address, int length)
		{
			return selector.Db(this).ReadReaderByAddress(address, length);
		}

		public virtual Db4objects.Db4o.YapWriter TargetWriterByAddress(int address, int length
			)
		{
			return _targetDb.ReadWriterByAddress(TARGETDB.Transaction(this), address, length);
		}

		public virtual int AllocateTargetSlot(int length)
		{
			return _targetDb.GetSlot(length);
		}

		public virtual void TargetWriteBytes(Db4objects.Db4o.ReaderPair readers, int address
			)
		{
			readers.Write(_targetDb, address);
		}

		public virtual void TargetWriteBytes(Db4objects.Db4o.YapReader reader, int address
			)
		{
			_targetDb.WriteBytes(reader, address, 0);
		}

		public virtual Db4objects.Db4o.Ext.IStoredClass[] StoredClasses(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector)
		{
			Db4objects.Db4o.YapFile db = selector.Db(this);
			db.ShowInternalClasses(true);
			Db4objects.Db4o.Ext.IStoredClass[] classes = db.StoredClasses();
			return classes;
		}

		public virtual Db4objects.Db4o.YapStringIO StringIO()
		{
			return _sourceDb.StringIO();
		}

		public virtual void TargetCommit()
		{
			_targetDb.Commit();
		}

		public virtual Db4objects.Db4o.ITypeHandler4 SourceHandler(int id)
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
				Db4objects.Db4o.YapReader reader = new Db4objects.Db4o.YapReader(Db4objects.Db4o.YapConst
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

		public virtual int ClassIndexID(Db4objects.Db4o.YapClass yapClass)
		{
			return ClassIndex(yapClass).Id();
		}

		public virtual void TraverseAll(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			if (!yapClass.HasIndex())
			{
				return;
			}
			yapClass.Index().TraverseAll(SOURCEDB.Transaction(this), command);
		}

		public virtual void TraverseAllIndexSlots(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			System.Collections.IEnumerator slotIDIter = yapClass.Index().AllSlotIDs(SOURCEDB.
				Transaction(this));
			while (slotIDIter.MoveNext())
			{
				command.Visit(slotIDIter.Current);
			}
		}

		public virtual void TraverseAllIndexSlots(Db4objects.Db4o.Inside.Btree.BTree btree
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
			Db4objects.Db4o.YapFile db = selector.Db(this);
			return db.Identity().GetID(selector.Transaction(this));
		}

		private Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy ClassIndex(Db4objects.Db4o.YapClass
			 yapClass)
		{
			Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy classIndex = (Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy
				)_classIndices.Get(yapClass);
			if (classIndex == null)
			{
				classIndex = new Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy(yapClass
					);
				_classIndices.Put(yapClass, classIndex);
				classIndex.Initialize(_targetDb);
			}
			return classIndex;
		}

		public virtual Db4objects.Db4o.Transaction SystemTrans()
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

		public virtual Db4objects.Db4o.YapReader SourceReaderByID(int sourceID)
		{
			return ReaderByID(SOURCEDB, sourceID);
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTree SourceUuidIndex()
		{
			if (SourceUuidIndexID() == 0)
			{
				return null;
			}
			return _sourceDb.GetFieldUUID().GetIndex(SystemTrans());
		}

		public virtual void TargetUuidIndexID(int id)
		{
			_targetDb.SystemData().UuidIndexId(id);
		}

		public virtual int SourceUuidIndexID()
		{
			return _sourceDb.SystemData().UuidIndexId();
		}

		public virtual Db4objects.Db4o.YapClass YapClass(int id)
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

		private Db4objects.Db4o.Inside.Slots.Slot ReadPointer(Db4objects.Db4o.Defragment.DefragContextImpl.DbSelector
			 selector, int id)
		{
			Db4objects.Db4o.YapReader reader = ReaderByAddress(selector, id, Db4objects.Db4o.YapConst
				.POINTER_LENGTH);
			int address = reader.ReadInt();
			int length = reader.ReadInt();
			return new Db4objects.Db4o.Inside.Slots.Slot(address, length);
		}
	}
}
