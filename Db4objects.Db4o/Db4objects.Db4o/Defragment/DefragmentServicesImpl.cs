/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Encoding;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Defragment
{
	/// <exclude></exclude>
	public class DefragmentServicesImpl : IDefragmentServices
	{
		public abstract class DbSelector
		{
			internal DbSelector()
			{
			}

			internal abstract LocalObjectContainer Db(DefragmentServicesImpl context);

			internal virtual Db4objects.Db4o.Internal.Transaction Transaction(DefragmentServicesImpl
				 context)
			{
				return Db(context).SystemTransaction();
			}
		}

		private sealed class _DbSelector_37 : DefragmentServicesImpl.DbSelector
		{
			public _DbSelector_37()
			{
			}

			internal override LocalObjectContainer Db(DefragmentServicesImpl context)
			{
				return context._sourceDb;
			}
		}

		public static readonly DefragmentServicesImpl.DbSelector Sourcedb = new _DbSelector_37
			();

		private sealed class _DbSelector_43 : DefragmentServicesImpl.DbSelector
		{
			public _DbSelector_43()
			{
			}

			internal override LocalObjectContainer Db(DefragmentServicesImpl context)
			{
				return context._targetDb;
			}
		}

		public static readonly DefragmentServicesImpl.DbSelector Targetdb = new _DbSelector_43
			();

		public readonly LocalObjectContainer _sourceDb;

		internal readonly LocalObjectContainer _targetDb;

		private readonly IContextIDMapping _mapping;

		private IDefragmentListener _listener;

		private IQueue4 _unindexed = new NonblockingQueue();

		private readonly Hashtable4 _hasFieldIndexCache = new Hashtable4();

		private DefragmentConfig _defragConfig;

		/// <exception cref="System.IO.IOException"></exception>
		public DefragmentServicesImpl(DefragmentConfig defragConfig, IDefragmentListener 
			listener)
		{
			_listener = listener;
			Config4Impl originalConfig = (Config4Impl)defragConfig.Db4oConfig();
			Config4Impl sourceConfig = (Config4Impl)originalConfig.DeepClone(null);
			sourceConfig.WeakReferences(false);
			IStorage storage = defragConfig.BackupStorage();
			if (defragConfig.ReadOnly())
			{
				storage = new NonFlushingStorage(storage);
			}
			sourceConfig.Storage = storage;
			sourceConfig.ReadOnly(defragConfig.ReadOnly());
			_sourceDb = (LocalObjectContainer)Db4oFactory.OpenFile(sourceConfig, defragConfig
				.TempPath()).Ext();
			_sourceDb.ShowInternalClasses(true);
			_targetDb = FreshTargetFile(defragConfig);
			_mapping = defragConfig.Mapping();
			_mapping.Open();
			_defragConfig = defragConfig;
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal static LocalObjectContainer FreshTempFile(string fileName, int blockSize
			)
		{
			FileStorage storage = new FileStorage();
			storage.Delete(fileName);
			IConfiguration db4oConfig = DefragmentConfig.VanillaDb4oConfig(blockSize);
			db4oConfig.Storage = storage;
			return (LocalObjectContainer)Db4oFactory.OpenFile(db4oConfig, fileName).Ext();
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal static LocalObjectContainer FreshTargetFile(DefragmentConfig config)
		{
			config.Db4oConfig().Storage.Delete(config.OrigPath());
			return (LocalObjectContainer)Db4oFactory.OpenFile(config.ClonedDb4oConfig(), config
				.OrigPath()).Ext();
		}

		public virtual int MappedID(int oldID, int defaultID)
		{
			int mapped = InternalMappedID(oldID, false);
			return (mapped != 0 ? mapped : defaultID);
		}

		/// <exception cref="Db4objects.Db4o.Internal.Mapping.MappingNotFoundException"></exception>
		public virtual int MappedID(int oldID)
		{
			int mapped = InternalMappedID(oldID, false);
			if (mapped == 0)
			{
				throw new MappingNotFoundException(oldID);
			}
			return mapped;
		}

		/// <exception cref="Db4objects.Db4o.Internal.Mapping.MappingNotFoundException"></exception>
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

		/// <exception cref="Db4objects.Db4o.Internal.Mapping.MappingNotFoundException"></exception>
		private int InternalMappedID(int oldID, bool lenient)
		{
			if (oldID == 0)
			{
				return 0;
			}
			if (_sourceDb.Handlers.IsSystemHandler(oldID))
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

		public virtual ByteArrayBuffer BufferByID(DefragmentServicesImpl.DbSelector selector
			, int id)
		{
			Slot slot = ReadPointer(selector, id);
			return BufferByAddress(selector, slot.Address(), slot.Length());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual ByteArrayBuffer SourceBufferByAddress(int address, int length)
		{
			return BufferByAddress(Sourcedb, address, length);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual ByteArrayBuffer TargetBufferByAddress(int address, int length)
		{
			return BufferByAddress(Targetdb, address, length);
		}

		public virtual ByteArrayBuffer BufferByAddress(DefragmentServicesImpl.DbSelector 
			selector, int address, int length)
		{
			return selector.Db(this).DecryptedBufferByAddress(address, length);
		}

		/// <exception cref="System.ArgumentException"></exception>
		public virtual StatefulBuffer TargetStatefulBufferByAddress(int address, int length
			)
		{
			return _targetDb.ReadWriterByAddress(Targetdb.Transaction(this), address, length);
		}

		public virtual Slot AllocateTargetSlot(int length)
		{
			return _targetDb.GetSlot(length);
		}

		public virtual void TargetWriteBytes(DefragmentContextImpl context, int address)
		{
			context.Write(_targetDb, address);
		}

		public virtual void TargetWriteBytes(ByteArrayBuffer reader, int address)
		{
			_targetDb.WriteBytes(reader, address, 0);
		}

		public virtual IStoredClass[] StoredClasses(DefragmentServicesImpl.DbSelector selector
			)
		{
			LocalObjectContainer db = selector.Db(this);
			db.ShowInternalClasses(true);
			try
			{
				return db.ClassCollection().StoredClasses();
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
			return _sourceDb.TypeHandlerForClassMetadataID(id);
		}

		public virtual int SourceClassCollectionID()
		{
			return _sourceDb.ClassCollection().GetID();
		}

		private Hashtable4 _classIndices = new Hashtable4(16);

		public virtual int ClassIndexID(ClassMetadata classMetadata)
		{
			return ClassIndex(classMetadata).Id();
		}

		public virtual void TraverseAll(ClassMetadata classMetadata, IVisitor4 command)
		{
			if (!classMetadata.HasClassIndex())
			{
				return;
			}
			classMetadata.Index().TraverseAll(Sourcedb.Transaction(this), command);
		}

		public virtual void TraverseAllIndexSlots(ClassMetadata classMetadata, IVisitor4 
			command)
		{
			IEnumerator slotIDIter = classMetadata.Index().AllSlotIDs(Sourcedb.Transaction(this
				));
			while (slotIDIter.MoveNext())
			{
				command.Visit(slotIDIter.Current);
			}
		}

		public virtual void TraverseAllIndexSlots(BTree btree, IVisitor4 command)
		{
			IEnumerator slotIDIter = btree.AllNodeIds(Sourcedb.Transaction(this));
			while (slotIDIter.MoveNext())
			{
				command.Visit(slotIDIter.Current);
			}
		}

		public virtual void RegisterBTreeIDs(BTree btree, IDMappingCollector collector)
		{
			collector.CreateIDMapping(this, btree.GetID(), false);
			TraverseAllIndexSlots(btree, new _IVisitor4_224(this, collector));
		}

		private sealed class _IVisitor4_224 : IVisitor4
		{
			public _IVisitor4_224(DefragmentServicesImpl _enclosing, IDMappingCollector collector
				)
			{
				this._enclosing = _enclosing;
				this.collector = collector;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				collector.CreateIDMapping(this._enclosing, id, false);
			}

			private readonly DefragmentServicesImpl _enclosing;

			private readonly IDMappingCollector collector;
		}

		public virtual int DatabaseIdentityID(DefragmentServicesImpl.DbSelector selector)
		{
			LocalObjectContainer db = selector.Db(this);
			Db4oDatabase identity = db.Identity();
			if (identity == null)
			{
				return 0;
			}
			return identity.GetID(selector.Transaction(this));
		}

		private IClassIndexStrategy ClassIndex(ClassMetadata classMetadata)
		{
			IClassIndexStrategy classIndex = (IClassIndexStrategy)_classIndices.Get(classMetadata
				);
			if (classIndex == null)
			{
				classIndex = new BTreeClassIndexStrategy(classMetadata);
				_classIndices.Put(classMetadata, classIndex);
				classIndex.Initialize(_targetDb);
			}
			return classIndex;
		}

		public virtual Db4objects.Db4o.Internal.Transaction SystemTrans()
		{
			return Sourcedb.Transaction(this);
		}

		public virtual void CopyIdentity()
		{
			_targetDb.SetIdentity(_sourceDb.Identity());
		}

		public virtual void TargetClassCollectionID(int newClassCollectionID)
		{
			_targetDb.SystemData().ClassCollectionID(newClassCollectionID);
		}

		public virtual ByteArrayBuffer SourceBufferByID(int sourceID)
		{
			return BufferByID(Sourcedb, sourceID);
		}

		public virtual BTree SourceUuidIndex()
		{
			if (SourceUuidIndexID() == 0)
			{
				return null;
			}
			return _sourceDb.UUIDIndex().GetIndex(SystemTrans());
		}

		public virtual void TargetUuidIndexID(int id)
		{
			_targetDb.SystemData().UuidIndexId(id);
		}

		public virtual int SourceUuidIndexID()
		{
			return _sourceDb.SystemData().UuidIndexId();
		}

		public virtual ClassMetadata ClassMetadataForId(int id)
		{
			return _sourceDb.ClassMetadataForID(id);
		}

		public virtual void RegisterUnindexed(int id)
		{
			_unindexed.Add(id);
		}

		public virtual IdSource UnindexedIDs()
		{
			return new IdSource(_unindexed);
		}

		public virtual ObjectHeader SourceObjectHeader(ByteArrayBuffer buffer)
		{
			return new ObjectHeader(_sourceDb, buffer);
		}

		private Slot ReadPointer(DefragmentServicesImpl.DbSelector selector, int id)
		{
			ByteArrayBuffer reader = selector.Db(this).RawBufferByAddress(id, Const4.PointerLength
				);
			int address = reader.ReadInt();
			int length = reader.ReadInt();
			return new Slot(address, length);
		}

		public virtual bool HasFieldIndex(ClassMetadata clazz)
		{
			// actually only two states are used here, the third is implicit in null
			TernaryBool cachedHasFieldIndex = ((TernaryBool)_hasFieldIndexCache.Get(clazz));
			if (cachedHasFieldIndex != null)
			{
				return cachedHasFieldIndex.DefiniteYes();
			}
			BooleanByRef hasFieldIndex = new BooleanByRef(false);
			ClassMetadata curClazz = clazz;
			while (!hasFieldIndex.value && curClazz != null)
			{
				curClazz.TraverseDeclaredFields(new _IProcedure4_320(hasFieldIndex));
				curClazz = curClazz.GetAncestor();
			}
			_hasFieldIndexCache.Put(clazz, TernaryBool.ForBoolean(hasFieldIndex.value));
			return hasFieldIndex.value;
		}

		private sealed class _IProcedure4_320 : IProcedure4
		{
			public _IProcedure4_320(BooleanByRef hasFieldIndex)
			{
				this.hasFieldIndex = hasFieldIndex;
			}

			public void Apply(object arg)
			{
				FieldMetadata curField = (FieldMetadata)arg;
				if (curField.HasIndex() && Handlers4.IsIndirectedIndexed(curField.GetHandler()))
				{
					hasFieldIndex.value = true;
				}
			}

			private readonly BooleanByRef hasFieldIndex;
		}

		public virtual int BlockSize()
		{
			return _sourceDb.Config().BlockSize();
		}

		public virtual int SourceAddressByID(int sourceID)
		{
			return ReadPointer(Sourcedb, sourceID).Address();
		}

		public virtual bool Accept(IStoredClass klass)
		{
			return this._defragConfig.StoredClassFilter().Accept(klass);
		}
	}
}
