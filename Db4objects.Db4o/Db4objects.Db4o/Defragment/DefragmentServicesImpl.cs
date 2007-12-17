/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
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

		private sealed class _DbSelector_36 : DefragmentServicesImpl.DbSelector
		{
			public _DbSelector_36()
			{
			}

			internal override LocalObjectContainer Db(DefragmentServicesImpl context)
			{
				return context._sourceDb;
			}
		}

		public static readonly DefragmentServicesImpl.DbSelector SOURCEDB = new _DbSelector_36
			();

		private sealed class _DbSelector_42 : DefragmentServicesImpl.DbSelector
		{
			public _DbSelector_42()
			{
			}

			internal override LocalObjectContainer Db(DefragmentServicesImpl context)
			{
				return context._targetDb;
			}
		}

		public static readonly DefragmentServicesImpl.DbSelector TARGETDB = new _DbSelector_42
			();

		private const long CLASSCOLLECTION_POINTER_ADDRESS = 2 + 2 * Const4.INT_LENGTH;

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
			sourceConfig.FlushFileBuffers(false);
			sourceConfig.ReadOnly(true);
			_sourceDb = (LocalObjectContainer)Db4oFactory.OpenFile(sourceConfig, defragConfig
				.TempPath()).Ext();
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

		public virtual BufferImpl BufferByID(DefragmentServicesImpl.DbSelector selector, 
			int id)
		{
			Slot slot = ReadPointer(selector, id);
			return BufferByAddress(selector, slot.Address(), slot.Length());
		}

		/// <exception cref="IOException"></exception>
		public virtual BufferImpl SourceBufferByAddress(int address, int length)
		{
			return BufferByAddress(SOURCEDB, address, length);
		}

		/// <exception cref="IOException"></exception>
		public virtual BufferImpl TargetBufferByAddress(int address, int length)
		{
			return BufferByAddress(TARGETDB, address, length);
		}

		public virtual BufferImpl BufferByAddress(DefragmentServicesImpl.DbSelector selector
			, int address, int length)
		{
			return selector.Db(this).BufferByAddress(address, length);
		}

		/// <exception cref="ArgumentException"></exception>
		public virtual StatefulBuffer TargetStatefulBufferByAddress(int address, int length
			)
		{
			return _targetDb.ReadWriterByAddress(TARGETDB.Transaction(this), address, length);
		}

		public virtual Slot AllocateTargetSlot(int length)
		{
			return _targetDb.GetSlot(length);
		}

		public virtual void TargetWriteBytes(DefragmentContextImpl context, int address)
		{
			context.Write(_targetDb, address);
		}

		public virtual void TargetWriteBytes(BufferImpl reader, int address)
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
			return _sourceDb.HandlerByID(id);
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
				BufferImpl reader = new BufferImpl(Const4.INT_LENGTH);
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
			if (!yapClass.HasClassIndex())
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

		/// <exception cref="IOException"></exception>
		public virtual BufferImpl SourceBufferByID(int sourceID)
		{
			return BufferByID(SOURCEDB, sourceID);
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

		public virtual IEnumerator UnindexedIDs()
		{
			return _unindexed.Iterator();
		}

		public virtual ObjectHeader SourceObjectHeader(BufferImpl buffer)
		{
			return new ObjectHeader(_sourceDb, buffer);
		}

		private Slot ReadPointer(DefragmentServicesImpl.DbSelector selector, int id)
		{
			BufferImpl reader = BufferByAddress(selector, id, Const4.POINTER_LENGTH);
			int address = reader.ReadInt();
			int length = reader.ReadInt();
			return new Slot(address, length);
		}

		public virtual bool HasFieldIndex(ClassMetadata clazz)
		{
			TernaryBool cachedHasFieldIndex = ((TernaryBool)_hasFieldIndexCache.Get(clazz));
			if (cachedHasFieldIndex != null)
			{
				return cachedHasFieldIndex.DefiniteYes();
			}
			bool hasFieldIndex = false;
			IEnumerator fieldIter = clazz.Fields();
			while (fieldIter.MoveNext())
			{
				FieldMetadata curField = (FieldMetadata)fieldIter.Current;
				if (curField.HasIndex() && (curField.GetHandler() is StringHandler))
				{
					hasFieldIndex = true;
					break;
				}
			}
			_hasFieldIndexCache.Put(clazz, TernaryBool.ForBoolean(hasFieldIndex));
			return hasFieldIndex;
		}

		public virtual int BlockSize()
		{
			return _sourceDb.Config().BlockSize();
		}

		public virtual int SourceAddressByID(int sourceID)
		{
			return ReadPointer(SOURCEDB, sourceID).Address();
		}

		public virtual bool Accept(IStoredClass klass)
		{
			return this._defragConfig.StoredClassFilter().Accept(klass);
		}
	}
}
