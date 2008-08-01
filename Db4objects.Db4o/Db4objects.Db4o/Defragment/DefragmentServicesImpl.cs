/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.IO;

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

		private const long ClasscollectionPointerAddress = 2 + 2 * Const4.IntLength;

		public readonly LocalObjectContainer _sourceDb;

		internal readonly LocalObjectContainer _targetDb;

		private readonly IContextIDMapping _mapping;

		private IDefragmentListener _listener;

		private IQueue4 _unindexed = new NonblockingQueue();

		private readonly Hashtable4 _hasFieldIndexCache = new Hashtable4();

		private DefragmentConfig _defragConfig;

		public DefragmentServicesImpl(DefragmentConfig defragConfig, IDefragmentListener 
			listener)
		{
			_listener = listener;
			Config4Impl originalConfig = (Config4Impl)defragConfig.Db4oConfig();
			IConfiguration sourceConfig = (IConfiguration)originalConfig.DeepClone(null);
			sourceConfig.WeakReferences(false);
			sourceConfig.Io(new NonFlushingIoAdapter(sourceConfig.Io()));
			sourceConfig.ReadOnly(defragConfig.ReadOnly());
			_sourceDb = (LocalObjectContainer)Db4oFactory.OpenFile(sourceConfig, defragConfig
				.TempPath()).Ext();
			_sourceDb.ShowInternalClasses(true);
			_targetDb = FreshYapFile(defragConfig);
			_mapping = defragConfig.Mapping();
			_mapping.Open();
			_defragConfig = defragConfig;
		}

		internal static LocalObjectContainer FreshYapFile(string fileName, int blockSize)
		{
			File4.Delete(fileName);
			return (LocalObjectContainer)Db4oFactory.OpenFile(DefragmentConfig.VanillaDb4oConfig
				(blockSize), fileName).Ext();
		}

		internal static LocalObjectContainer FreshYapFile(DefragmentConfig config)
		{
			File4.Delete(config.OrigPath());
			return (LocalObjectContainer)Db4oFactory.OpenFile(config.ClonedDb4oConfig(), config
				.OrigPath()).Ext();
		}

		public virtual int MappedID(int oldID, int defaultID)
		{
			int mapped = InternalMappedID(oldID, false);
			return (mapped != 0 ? mapped : defaultID);
		}

		/// <exception cref="MappingNotFoundException"></exception>
		public virtual int MappedID(int oldID)
		{
			int mapped = InternalMappedID(oldID, false);
			if (mapped == 0)
			{
				throw new MappingNotFoundException(oldID);
			}
			return mapped;
		}

		/// <exception cref="MappingNotFoundException"></exception>
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

		/// <exception cref="MappingNotFoundException"></exception>
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

		public virtual ByteArrayBuffer BufferByID(DefragmentServicesImpl.DbSelector selector
			, int id)
		{
			Slot slot = ReadPointer(selector, id);
			return BufferByAddress(selector, slot.Address(), slot.Length());
		}

		/// <exception cref="IOException"></exception>
		public virtual ByteArrayBuffer SourceBufferByAddress(int address, int length)
		{
			return BufferByAddress(Sourcedb, address, length);
		}

		/// <exception cref="IOException"></exception>
		public virtual ByteArrayBuffer TargetBufferByAddress(int address, int length)
		{
			return BufferByAddress(Targetdb, address, length);
		}

		public virtual ByteArrayBuffer BufferByAddress(DefragmentServicesImpl.DbSelector 
			selector, int address, int length)
		{
			return selector.Db(this).BufferByAddress(address, length);
		}

		/// <exception cref="ArgumentException"></exception>
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
			return _sourceDb.TypeHandlerForId(id);
		}

		public virtual int SourceClassCollectionID()
		{
			return _sourceDb.ClassCollection().GetID();
		}

		/// <exception cref="IOException"></exception>
		public static void TargetClassCollectionID(string file, int id)
		{
			RandomAccessFile raf = new RandomAccessFile(file, "rw");
			try
			{
				ByteArrayBuffer reader = new ByteArrayBuffer(Const4.IntLength);
				raf.Seek(ClasscollectionPointerAddress);
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
			if (!yapClass.HasClassIndex())
			{
				return;
			}
			yapClass.Index().TraverseAll(Sourcedb.Transaction(this), command);
		}

		public virtual void TraverseAllIndexSlots(ClassMetadata yapClass, IVisitor4 command
			)
		{
			IEnumerator slotIDIter = yapClass.Index().AllSlotIDs(Sourcedb.Transaction(this));
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
			return _sourceDb.ClassMetadataForId(id);
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
			ByteArrayBuffer reader = BufferByAddress(selector, id, Const4.PointerLength);
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
			clazz.ForEachDeclaredField(new _IProcedure4_318(hasFieldIndex));
			_hasFieldIndexCache.Put(clazz, TernaryBool.ForBoolean(hasFieldIndex.value));
			return hasFieldIndex.value;
		}

		private sealed class _IProcedure4_318 : IProcedure4
		{
			public _IProcedure4_318(BooleanByRef hasFieldIndex)
			{
				this.hasFieldIndex = hasFieldIndex;
			}

			public void Apply(object arg)
			{
				FieldMetadata curField = (FieldMetadata)arg;
				if (curField.HasIndex() && (curField.GetHandler() is StringHandler))
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
