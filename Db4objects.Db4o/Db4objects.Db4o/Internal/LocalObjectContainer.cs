/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.Fileheader;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Internal.Slots;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class LocalObjectContainer : ExternalObjectContainer, IInternalObjectContainer
	{
		private const int DEFAULT_FREESPACE_ID = 0;

		protected FileHeader _fileHeader;

		private Collection4 i_dirty;

		private IFreespaceManager _freespaceManager;

		private bool i_isServer = false;

		private Tree i_prefetchedIDs;

		private Hashtable4 i_semaphores;

		private int _blockEndAddress;

		private Tree _freeOnCommit;

		private Db4objects.Db4o.Internal.SystemData _systemData;

		internal LocalObjectContainer(IConfiguration config, ObjectContainerBase parentContainer
			) : base(config, parentContainer)
		{
		}

		public override Transaction NewTransaction(Transaction parentTransaction, TransactionalReferenceSystem
			 referenceSystem)
		{
			return new LocalTransaction(this, parentTransaction, referenceSystem);
		}

		public virtual IFreespaceManager FreespaceManager()
		{
			return _freespaceManager;
		}

		public abstract void BlockSize(int size);

		public virtual void BlockSizeReadFromFile(int size)
		{
			BlockSize(size);
			SetRegularEndAddress(FileLength());
		}

		public virtual void SetRegularEndAddress(long address)
		{
			_blockEndAddress = BytesToBlocks(address);
		}

		protected sealed override void Close2()
		{
			if (!_config.IsReadOnly())
			{
				FreeInternalResources();
				CommitTransaction();
				Shutdown();
			}
			ShutdownObjectContainer();
		}

		protected abstract void FreeInternalResources();

		public override void Commit1(Transaction trans)
		{
			trans.Commit();
		}

		internal virtual void ConfigureNewFile()
		{
			NewSystemData(ConfigImpl().FreespaceSystem());
			SystemData().ConverterVersion(Converter.VERSION);
			CreateStringIO(_systemData.StringEncoding());
			GenerateNewIdentity();
			_freespaceManager = AbstractFreespaceManager.CreateNew(this);
			BlockSize(ConfigImpl().BlockSize());
			_fileHeader = new FileHeader1();
			SetRegularEndAddress(_fileHeader.Length());
			InitNewClassCollection();
			InitializeEssentialClasses();
			_fileHeader.InitNew(this);
			_freespaceManager.OnNew(this);
			_freespaceManager.Start(_systemData.FreespaceAddress());
		}

		private void NewSystemData(byte freespaceSystem)
		{
			_systemData = new Db4objects.Db4o.Internal.SystemData();
			_systemData.StringEncoding(ConfigImpl().Encoding());
			_systemData.FreespaceSystem(freespaceSystem);
		}

		public override int ConverterVersion()
		{
			return _systemData.ConverterVersion();
		}

		public abstract void Copy(int oldAddress, int oldAddressOffset, int newAddress, int
			 newAddressOffset, int length);

		public override long CurrentVersion()
		{
			return _timeStampIdGenerator.LastTimeStampId();
		}

		internal virtual void InitNewClassCollection()
		{
			ClassCollection().InitTables(1);
		}

		public BTree CreateBTreeClassIndex(int id)
		{
			return new BTree(_transaction, id, new IDHandler(this));
		}

		public AbstractQueryResult NewQueryResult(Transaction trans)
		{
			return NewQueryResult(trans, Config().QueryEvaluationMode());
		}

		public sealed override AbstractQueryResult NewQueryResult(Transaction trans, QueryEvaluationMode
			 mode)
		{
			if (mode == QueryEvaluationMode.IMMEDIATE)
			{
				return new IdListQueryResult(trans);
			}
			return new HybridQueryResult(trans, mode);
		}

		public sealed override bool Delete4(Transaction ta, ObjectReference yo, int a_cascade
			, bool userCall)
		{
			int id = yo.GetID();
			StatefulBuffer reader = ReadWriterByID(ta, id);
			if (reader != null)
			{
				object obj = yo.GetObject();
				if (obj != null)
				{
					if ((!ShowInternalClasses()) && Const4.CLASS_INTERNAL.IsAssignableFrom(obj.GetType
						()))
					{
						return false;
					}
				}
				reader.SetCascadeDeletes(a_cascade);
				reader.SlotDelete();
				ClassMetadata yc = yo.ClassMetadata();
				yc.Delete(reader, obj);
				return true;
			}
			return false;
		}

		public abstract long FileLength();

		public abstract string FileName();

		public virtual void Free(Slot slot)
		{
			if (slot.Address() == 0)
			{
				return;
			}
			if (_freespaceManager == null)
			{
				return;
			}
			Slot blockedSlot = ToBlockedLength(slot);
			_freespaceManager.Free(blockedSlot);
		}

		public virtual Slot ToBlockedLength(Slot slot)
		{
			return new Slot(slot.Address(), BytesToBlocks(slot.Length()));
		}

		public virtual Slot ToNonBlockedLength(Slot slot)
		{
			return new Slot(slot.Address(), BlocksToBytes(slot.Length()));
		}

		public virtual void Free(int address, int a_length)
		{
			Free(new Slot(address, a_length));
		}

		internal void FreePrefetchedPointers()
		{
			if (i_prefetchedIDs != null)
			{
				i_prefetchedIDs.Traverse(new _IVisitor4_212(this));
			}
			i_prefetchedIDs = null;
		}

		private sealed class _IVisitor4_212 : IVisitor4
		{
			public _IVisitor4_212(LocalObjectContainer _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.Free(((TreeInt)a_object)._key, Const4.POINTER_LENGTH);
			}

			private readonly LocalObjectContainer _enclosing;
		}

		public virtual void GenerateNewIdentity()
		{
			lock (_lock)
			{
				SetIdentity(Db4oDatabase.Generate());
			}
		}

		public override AbstractQueryResult GetAll(Transaction trans)
		{
			return GetAll(trans, Config().QueryEvaluationMode());
		}

		public virtual AbstractQueryResult GetAll(Transaction trans, QueryEvaluationMode 
			mode)
		{
			AbstractQueryResult queryResult = NewQueryResult(trans, mode);
			queryResult.LoadFromClassIndexes(ClassCollection().Iterator());
			return queryResult;
		}

		public int GetPointerSlot()
		{
			int id = GetSlot(Const4.POINTER_LENGTH).Address();
			((LocalTransaction)SystemTransaction()).WriteZeroPointer(id);
			if (_handlers.IsSystemHandler(id))
			{
				return GetPointerSlot();
			}
			return id;
		}

		public virtual Slot GetSlot(int length)
		{
			int blocks = BytesToBlocks(length);
			Slot slot = GetBlockedSlot(blocks);
			return ToNonBlockedLength(slot);
		}

		private Slot GetBlockedSlot(int blocks)
		{
			if (blocks <= 0)
			{
				throw new ArgumentException();
			}
			if (_freespaceManager != null)
			{
				Slot slot = _freespaceManager.GetSlot(blocks);
				if (slot != null)
				{
					return slot;
				}
			}
			return AppendBlocks(blocks);
		}

		protected Slot AppendBlocks(int blockCount)
		{
			int blockedStartAddress = _blockEndAddress;
			int blockedEndAddress = _blockEndAddress + blockCount;
			CheckBlockedAddress(blockedEndAddress);
			_blockEndAddress = blockedEndAddress;
			Slot slot = new Slot(blockedStartAddress, blockCount);
			if (Debug.xbytes && Deploy.overwrite)
			{
				OverwriteDeletedBlockedSlot(slot);
			}
			return slot;
		}

		internal Slot AppendBytes(long bytes)
		{
			Slot slot = AppendBlocks(BytesToBlocks(bytes));
			return ToNonBlockedLength(slot);
		}

		private void CheckBlockedAddress(int blockedAddress)
		{
			if (blockedAddress < 0)
			{
				SwitchToReadOnlyMode();
				throw new DatabaseMaximumSizeReachedException();
			}
		}

		private void SwitchToReadOnlyMode()
		{
			_config.ReadOnly(true);
		}

		internal virtual void EnsureLastSlotWritten()
		{
			if (_blockEndAddress > BytesToBlocks(FileLength()))
			{
				StatefulBuffer writer = GetWriter(SystemTransaction(), _blockEndAddress - 1, BlockSize
					());
				writer.Write();
			}
		}

		public override Db4oDatabase Identity()
		{
			return _systemData.Identity();
		}

		public virtual void SetIdentity(Db4oDatabase identity)
		{
			_systemData.Identity(identity);
			_timeStampIdGenerator.Next();
		}

		internal override void Initialize2()
		{
			i_dirty = new Collection4();
			base.Initialize2();
		}

		internal override bool IsServer()
		{
			return i_isServer;
		}

		public Pointer4 NewSlot(int length)
		{
			return new Pointer4(GetPointerSlot(), GetSlot(length));
		}

		public sealed override int NewUserObject()
		{
			return GetPointerSlot();
		}

		public virtual void PrefetchedIDConsumed(int a_id)
		{
			i_prefetchedIDs = i_prefetchedIDs.RemoveLike(new TreeIntObject(a_id));
		}

		public virtual int PrefetchID()
		{
			int id = GetPointerSlot();
			i_prefetchedIDs = Tree.Add(i_prefetchedIDs, new TreeInt(id));
			return id;
		}

		public virtual ReferencedSlot ProduceFreeOnCommitEntry(int id)
		{
			Tree node = TreeInt.Find(_freeOnCommit, id);
			if (node != null)
			{
				return (ReferencedSlot)node;
			}
			ReferencedSlot slot = new ReferencedSlot(id);
			_freeOnCommit = Tree.Add(_freeOnCommit, slot);
			return slot;
		}

		public virtual void ReduceFreeOnCommitReferences(ReferencedSlot slot)
		{
			if (slot.RemoveReferenceIsLast())
			{
				_freeOnCommit = _freeOnCommit.RemoveNode(slot);
			}
		}

		public virtual void FreeDuringCommit(ReferencedSlot referencedSlot, Slot slot)
		{
			_freeOnCommit = referencedSlot.Free(this, _freeOnCommit, slot);
		}

		public override void RaiseVersion(long a_minimumVersion)
		{
			lock (Lock())
			{
				_timeStampIdGenerator.SetMinimumNext(a_minimumVersion);
			}
		}

		public override StatefulBuffer ReadWriterByID(Transaction a_ta, int a_id)
		{
			return (StatefulBuffer)ReadReaderOrWriterByID(a_ta, a_id, false);
		}

		public override StatefulBuffer[] ReadWritersByIDs(Transaction a_ta, int[] ids)
		{
			StatefulBuffer[] yapWriters = new StatefulBuffer[ids.Length];
			for (int i = 0; i < ids.Length; ++i)
			{
				if (ids[i] == 0)
				{
					yapWriters[i] = null;
				}
				else
				{
					yapWriters[i] = (StatefulBuffer)ReadReaderOrWriterByID(a_ta, ids[i], false);
				}
			}
			return yapWriters;
		}

		public override Db4objects.Db4o.Internal.Buffer ReadReaderByID(Transaction a_ta, 
			int a_id)
		{
			return ReadReaderOrWriterByID(a_ta, a_id, true);
		}

		private Db4objects.Db4o.Internal.Buffer ReadReaderOrWriterByID(Transaction a_ta, 
			int a_id, bool useReader)
		{
			if (a_id <= 0)
			{
				throw new ArgumentException();
			}
			Slot slot = ((LocalTransaction)a_ta).GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return null;
			}
			if (slot.Address() == 0)
			{
				return null;
			}
			Db4objects.Db4o.Internal.Buffer reader = null;
			if (useReader)
			{
				reader = new Db4objects.Db4o.Internal.Buffer(slot.Length());
			}
			else
			{
				reader = GetWriter(a_ta, slot.Address(), slot.Length());
				((StatefulBuffer)reader).SetID(a_id);
			}
			reader.ReadEncrypt(this, slot.Address());
			return reader;
		}

		protected override bool DoFinalize()
		{
			return _fileHeader != null;
		}

		internal virtual void ReadThis()
		{
			NewSystemData(AbstractFreespaceManager.FM_LEGACY_RAM);
			BlockSizeReadFromFile(1);
			_fileHeader = FileHeader.ReadFixedPart(this);
			CreateStringIO(_systemData.StringEncoding());
			ClassCollection().SetID(_systemData.ClassCollectionID());
			ClassCollection().Read(SystemTransaction());
			Converter.Convert(new ConversionStage.ClassCollectionAvailableStage(this));
			ReadHeaderVariablePart();
			if (!_config.IsReadOnly())
			{
				_freespaceManager = AbstractFreespaceManager.CreateNew(this, _systemData.FreespaceSystem
					());
				_freespaceManager.Read(_systemData.FreespaceID());
				_freespaceManager.Start(_systemData.FreespaceAddress());
			}
			if (NeedFreespaceMigration())
			{
				MigrateFreespace();
			}
			if (_config.IsReadOnly())
			{
				return;
			}
			WriteHeader(true, false);
			LocalTransaction trans = (LocalTransaction)_fileHeader.InterruptedTransaction();
			if (trans != null)
			{
				if (!ConfigImpl().CommitRecoveryDisabled())
				{
					trans.WriteOld();
				}
			}
			if (Converter.Convert(new ConversionStage.SystemUpStage(this)))
			{
				_systemData.ConverterVersion(Converter.VERSION);
				_fileHeader.WriteVariablePart(this, 1);
				Transaction().Commit();
			}
		}

		private bool NeedFreespaceMigration()
		{
			byte readSystem = _systemData.FreespaceSystem();
			byte configuredSystem = ConfigImpl().FreespaceSystem();
			return (configuredSystem != 0 || readSystem == AbstractFreespaceManager.FM_LEGACY_RAM
				) && (_freespaceManager.SystemType() != configuredSystem);
		}

		private void MigrateFreespace()
		{
			IFreespaceManager oldFreespaceManager = _freespaceManager;
			_freespaceManager = AbstractFreespaceManager.CreateNew(this, ConfigImpl().FreespaceSystem
				());
			SystemData().FreespaceAddress(0);
			SystemData().FreespaceSystem(ConfigImpl().FreespaceSystem());
			_freespaceManager.Start(_freespaceManager.OnNew(this));
			AbstractFreespaceManager.Migrate(oldFreespaceManager, _freespaceManager);
			_fileHeader.WriteVariablePart(this, 1);
		}

		private void ReadHeaderVariablePart()
		{
			_fileHeader.ReadVariablePart(this);
			SetNextTimeStampId(SystemData().LastTimeStampID());
		}

		public int CreateFreespaceSlot(byte freespaceSystem)
		{
			_systemData.FreespaceAddress(AbstractFreespaceManager.InitSlot(this));
			_systemData.FreespaceSystem(freespaceSystem);
			return _systemData.FreespaceAddress();
		}

		public virtual int EnsureFreespaceSlot()
		{
			int address = SystemData().FreespaceAddress();
			if (address == 0)
			{
				return CreateFreespaceSlot(SystemData().FreespaceSystem());
			}
			return address;
		}

		public sealed override void ReleaseSemaphore(string name)
		{
			ReleaseSemaphore(null, name);
		}

		public void ReleaseSemaphore(Transaction trans, string name)
		{
			lock (_lock)
			{
				if (i_semaphores == null)
				{
					return;
				}
			}
			lock (i_semaphores)
			{
				trans = CheckTransaction(trans);
				if (i_semaphores != null && trans == i_semaphores.Get(name))
				{
					i_semaphores.Remove(name);
					Sharpen.Runtime.NotifyAll(i_semaphores);
				}
			}
		}

		public override void ReleaseSemaphores(Transaction ta)
		{
			if (i_semaphores != null)
			{
				Hashtable4 semaphores = i_semaphores;
				lock (semaphores)
				{
					semaphores.ForEachKeyForIdentity(new _IVisitor4_559(this, semaphores), ta);
					Sharpen.Runtime.NotifyAll(semaphores);
				}
			}
		}

		private sealed class _IVisitor4_559 : IVisitor4
		{
			public _IVisitor4_559(LocalObjectContainer _enclosing, Hashtable4 semaphores)
			{
				this._enclosing = _enclosing;
				this.semaphores = semaphores;
			}

			public void Visit(object a_object)
			{
				semaphores.Remove(a_object);
			}

			private readonly LocalObjectContainer _enclosing;

			private readonly Hashtable4 semaphores;
		}

		public sealed override void Rollback1(Transaction trans)
		{
			trans.Rollback();
		}

		public sealed override void SetDirtyInSystemTransaction(PersistentBase a_object)
		{
			a_object.SetStateDirty();
			a_object.CacheDirty(i_dirty);
		}

		public sealed override bool SetSemaphore(string name, int timeout)
		{
			return SetSemaphore(null, name, timeout);
		}

		public bool SetSemaphore(Transaction trans, string name, int timeout)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			lock (_lock)
			{
				if (i_semaphores == null)
				{
					i_semaphores = new Hashtable4(10);
				}
			}
			lock (i_semaphores)
			{
				trans = CheckTransaction(trans);
				object obj = i_semaphores.Get(name);
				if (obj == null)
				{
					i_semaphores.Put(name, trans);
					return true;
				}
				if (trans == obj)
				{
					return true;
				}
				long endtime = Runtime.CurrentTimeMillis() + timeout;
				long waitTime = timeout;
				while (waitTime > 0)
				{
					try
					{
						Sharpen.Runtime.Wait(i_semaphores, waitTime);
					}
					catch (Exception)
					{
					}
					if (ClassCollection() == null)
					{
						return false;
					}
					obj = i_semaphores.Get(name);
					if (obj == null)
					{
						i_semaphores.Put(name, trans);
						return true;
					}
					waitTime = endtime - Runtime.CurrentTimeMillis();
				}
				return false;
			}
		}

		public virtual void SetServer(bool flag)
		{
			i_isServer = flag;
		}

		public abstract void SyncFiles();

		public override string ToString()
		{
			return FileName();
		}

		public override void Shutdown()
		{
			WriteHeader(false, true);
		}

		public void CommitTransaction()
		{
			_transaction.Commit();
		}

		public abstract void WriteBytes(Db4objects.Db4o.Internal.Buffer a_Bytes, int address
			, int addressOffset);

		public sealed override void WriteDirty()
		{
			WriteCachedDirty();
			WriteVariableHeader();
		}

		private void WriteCachedDirty()
		{
			IEnumerator i = i_dirty.GetEnumerator();
			while (i.MoveNext())
			{
				PersistentBase dirty = (PersistentBase)i.Current;
				dirty.Write(SystemTransaction());
				dirty.NotCachedDirty();
			}
			i_dirty.Clear();
		}

		public void WriteEncrypt(Db4objects.Db4o.Internal.Buffer buffer, int address, int
			 addressOffset)
		{
			_handlers.Encrypt(buffer);
			WriteBytes(buffer, address, addressOffset);
			_handlers.Decrypt(buffer);
		}

		protected virtual void WriteVariableHeader()
		{
			if (!_timeStampIdGenerator.IsDirty())
			{
				return;
			}
			_systemData.LastTimeStampID(_timeStampIdGenerator.LastTimeStampId());
			_fileHeader.WriteVariablePart(this, 2);
			_timeStampIdGenerator.SetClean();
		}

		internal virtual void WriteHeader(bool startFileLockingThread, bool shuttingDown)
		{
			int freespaceID = DEFAULT_FREESPACE_ID;
			if (shuttingDown)
			{
				freespaceID = _freespaceManager.Write();
				_freespaceManager = null;
			}
			StatefulBuffer writer = GetWriter(SystemTransaction(), 0, _fileHeader.Length());
			_fileHeader.WriteFixedPart(this, startFileLockingThread, shuttingDown, writer, BlockSize
				(), freespaceID);
			if (shuttingDown)
			{
				EnsureLastSlotWritten();
			}
			SyncFiles();
		}

		public sealed override void WriteNew(Transaction trans, Pointer4 pointer, ClassMetadata
			 classMetadata, Db4objects.Db4o.Internal.Buffer buffer)
		{
			WriteEncrypt(buffer, pointer.Address(), 0);
			if (classMetadata == null)
			{
				return;
			}
			if (MaintainsIndices())
			{
				classMetadata.AddToIndex(this, trans, pointer.Id());
			}
		}

		public abstract void OverwriteDeletedBytes(int address, int length);

		public virtual void OverwriteDeletedBlockedSlot(Slot slot)
		{
			OverwriteDeletedBytes(slot.Address(), BlocksToBytes(slot.Length()));
		}

		public sealed override void WriteTransactionPointer(int address)
		{
			_fileHeader.WriteTransactionPointer(SystemTransaction(), address);
		}

		public void GetSlotForUpdate(StatefulBuffer buffer)
		{
			Slot slot = GetSlotForUpdate(buffer.GetTransaction(), buffer.GetID(), buffer.Length
				());
			buffer.Address(slot.Address());
		}

		public Slot GetSlotForUpdate(Transaction trans, int id, int length)
		{
			Slot slot = GetSlot(length);
			trans.ProduceUpdateSlotChange(id, slot);
			return slot;
		}

		public sealed override void WriteUpdate(Transaction trans, Pointer4 pointer, ClassMetadata
			 classMetadata, Db4objects.Db4o.Internal.Buffer buffer)
		{
			int address = pointer.Address();
			if (address == 0)
			{
				address = GetSlotForUpdate(trans, pointer.Id(), pointer.Length()).Address();
			}
			WriteEncrypt(buffer, address, 0);
		}

		public virtual void SetNextTimeStampId(long val)
		{
			_timeStampIdGenerator.SetMinimumNext(val);
			_timeStampIdGenerator.SetClean();
		}

		public override ISystemInfo SystemInfo()
		{
			return new SystemInfoFileImpl(this);
		}

		public virtual FileHeader GetFileHeader()
		{
			return _fileHeader;
		}

		public virtual void InstallDebugFreespaceManager(AbstractFreespaceManager manager
			)
		{
			_freespaceManager = manager;
		}

		public virtual Db4objects.Db4o.Internal.SystemData SystemData()
		{
			return _systemData;
		}

		public override long[] GetIDsForClass(Transaction trans, ClassMetadata clazz)
		{
			IntArrayList ids = new IntArrayList();
			clazz.Index().TraverseAll(trans, new _IVisitor4_762(this, ids));
			return ids.AsLong();
		}

		private sealed class _IVisitor4_762 : IVisitor4
		{
			public _IVisitor4_762(LocalObjectContainer _enclosing, IntArrayList ids)
			{
				this._enclosing = _enclosing;
				this.ids = ids;
			}

			public void Visit(object obj)
			{
				ids.Add(((int)obj));
			}

			private readonly LocalObjectContainer _enclosing;

			private readonly IntArrayList ids;
		}

		public override IQueryResult ClassOnlyQuery(Transaction trans, ClassMetadata clazz
			)
		{
			if (!clazz.HasClassIndex())
			{
				return null;
			}
			AbstractQueryResult queryResult = NewQueryResult(trans);
			queryResult.LoadFromClassIndex(clazz);
			return queryResult;
		}

		public override IQueryResult ExecuteQuery(QQuery query)
		{
			AbstractQueryResult queryResult = NewQueryResult(query.GetTransaction());
			queryResult.LoadFromQuery(query);
			return queryResult;
		}

		public virtual LocalTransaction GetLocalSystemTransaction()
		{
			return (LocalTransaction)SystemTransaction();
		}

		public override void OnCommittedListener()
		{
		}
	}
}
