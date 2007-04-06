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
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Internal.Slots;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class LocalObjectContainer : ObjectContainerBase
	{
		private const int DEFAULT_FREESPACE_ID = 0;

		protected FileHeader _fileHeader;

		private Collection4 i_dirty;

		private Db4objects.Db4o.Internal.Freespace.FreespaceManager _freespaceManager;

		private Db4objects.Db4o.Internal.Freespace.FreespaceManager _fmChecker;

		private bool i_isServer = false;

		private Tree i_prefetchedIDs;

		private Hashtable4 i_semaphores;

		private int _blockEndAddress;

		private Tree _freeOnCommit;

		private Db4objects.Db4o.Internal.SystemData _systemData;

		internal LocalObjectContainer(IConfiguration config, ObjectContainerBase a_parent
			) : base(config, a_parent)
		{
		}

		public override Transaction NewTransaction(Transaction parentTransaction)
		{
			return new LocalTransaction(this, parentTransaction);
		}

		public virtual Db4objects.Db4o.Internal.Freespace.FreespaceManager FreespaceManager
			()
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
			_blockEndAddress = BlocksFor(address);
		}

		protected sealed override void Close2()
		{
			FreeInternalResources();
			CommitTransaction();
			Shutdown();
			ShutdownObjectContainer();
		}

		protected abstract void FreeInternalResources();

		public override void Commit1()
		{
			CommitTransaction();
		}

		internal virtual void ConfigureNewFile()
		{
			NewSystemData(ConfigImpl().FreespaceSystem());
			SystemData().ConverterVersion(Converter.VERSION);
			CreateStringIO(_systemData.StringEncoding());
			GenerateNewIdentity();
			_freespaceManager = Db4objects.Db4o.Internal.Freespace.FreespaceManager.CreateNew
				(this);
			BlockSize(ConfigImpl().BlockSize());
			_fileHeader = new FileHeader1();
			SetRegularEndAddress(_fileHeader.Length());
			InitNewClassCollection();
			InitializeEssentialClasses();
			_fileHeader.InitNew(this);
			_freespaceManager.OnNew(this);
			_freespaceManager.Start(_systemData.FreespaceAddress());
			if (Debug.freespace && Debug.freespaceChecker)
			{
				_fmChecker.Start(0);
			}
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
			return new BTree(i_trans, id, new IDHandler(this));
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
				ClassMetadata yc = yo.GetYapClass();
				yc.Delete(reader, obj);
				return true;
			}
			return false;
		}

		public abstract long FileLength();

		public abstract string FileName();

		public virtual void Free(Slot slot)
		{
			if (slot == null)
			{
				return;
			}
			if (slot._address == 0)
			{
				return;
			}
			Free(slot._address, slot._length);
		}

		public virtual void Free(int a_address, int a_length)
		{
			if (_freespaceManager == null)
			{
				return;
			}
			_freespaceManager.Free(a_address, a_length);
			if (Debug.freespace && Debug.freespaceChecker)
			{
				_fmChecker.Free(a_address, a_length);
			}
		}

		internal void FreePrefetchedPointers()
		{
			if (i_prefetchedIDs != null)
			{
				i_prefetchedIDs.Traverse(new _AnonymousInnerClass213(this));
			}
			i_prefetchedIDs = null;
		}

		private sealed class _AnonymousInnerClass213 : IVisitor4
		{
			public _AnonymousInnerClass213(LocalObjectContainer _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.Free(((TreeInt)a_object)._key, Const4.POINTER_LENGTH);
			}

			private readonly LocalObjectContainer _enclosing;
		}

		internal void FreeSpaceBeginCommit()
		{
			if (_freespaceManager == null)
			{
				return;
			}
			_freespaceManager.BeginCommit();
		}

		internal void FreeSpaceEndCommit()
		{
			if (_freespaceManager == null)
			{
				return;
			}
			_freespaceManager.EndCommit();
		}

		public virtual void GenerateNewIdentity()
		{
			lock (i_lock)
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

		internal int GetPointerSlot()
		{
			int id = GetSlot(Const4.POINTER_LENGTH);
			((LocalTransaction)SystemTransaction()).WritePointer(id, 0, 0);
			if (i_handlers.IsSystemHandler(id))
			{
				return GetPointerSlot();
			}
			return id;
		}

		public virtual int GetSlot(int a_length)
		{
			return GetSlot1(a_length);
			int address = GetSlot1(a_length);
			DTrace.GET_SLOT.LogLength(address, a_length);
			return address;
		}

		private int GetSlot1(int bytes)
		{
			if (_freespaceManager != null)
			{
				int freeAddress = _freespaceManager.GetSlot(bytes);
				if (Debug.freespace && Debug.freespaceChecker)
				{
					if (freeAddress > 0)
					{
						Collection4 wrongOnes = new Collection4();
						int freeCheck = _fmChecker.GetSlot(bytes);
						while (freeCheck != freeAddress && freeCheck > 0)
						{
							wrongOnes.Add(new int[] { freeCheck, bytes });
							freeCheck = _fmChecker.GetSlot(bytes);
						}
						IEnumerator i = wrongOnes.GetEnumerator();
						while (i.MoveNext())
						{
							int[] adrLength = (int[])i.Current;
							_fmChecker.Free(adrLength[0], adrLength[1]);
						}
						if (freeCheck == 0)
						{
							_freespaceManager.Debug();
							_fmChecker.Debug();
						}
					}
				}
				if (freeAddress > 0)
				{
					return freeAddress;
				}
			}
			int blocksNeeded = BlocksFor(bytes);
			if (Debug.xbytes && Deploy.overwrite)
			{
				OverwriteDeletedBytes(_blockEndAddress, blocksNeeded * BlockSize());
			}
			return AppendBlocks(blocksNeeded);
		}

		protected virtual int AppendBlocks(int blockCount)
		{
			int blockedStartAddress = _blockEndAddress;
			int blockedEndAddress = _blockEndAddress + blockCount;
			CheckBlockedAddress(blockedEndAddress);
			_blockEndAddress = blockedEndAddress;
			return blockedStartAddress;
		}

		private void CheckBlockedAddress(int blockedAddress)
		{
			if (blockedAddress < 0)
			{
				Rollback1();
				SwitchToReadOnlyMode();
				Exceptions4.ThrowRuntimeException(69);
			}
		}

		private void SwitchToReadOnlyMode()
		{
			i_config.ReadOnly(true);
		}

		internal virtual void EnsureLastSlotWritten()
		{
			if (_blockEndAddress > BlocksFor(FileLength()))
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

		public Pointer4 NewSlot(Transaction a_trans, int a_length)
		{
			int id = GetPointerSlot();
			int address = GetSlot(a_length);
			a_trans.SetPointer(id, address, a_length);
			return new Pointer4(id, address);
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
				throw new ArgumentException("id must be greater than 0");
			}
			try
			{
				Slot slot = ((LocalTransaction)a_ta).GetCurrentSlotOfID(a_id);
				if (slot == null)
				{
					return null;
				}
				if (slot._address == 0)
				{
					return null;
				}
				Db4objects.Db4o.Internal.Buffer reader = null;
				if (useReader)
				{
					reader = new Db4objects.Db4o.Internal.Buffer(slot._length);
				}
				else
				{
					reader = GetWriter(a_ta, slot._address, slot._length);
					((StatefulBuffer)reader).SetID(a_id);
				}
				reader.ReadEncrypt(this, slot._address);
				return reader;
			}
			catch (Exception e)
			{
			}
			return null;
		}

		protected override bool DoFinalize()
		{
			return _fileHeader != null;
		}

		internal virtual void ReadThis()
		{
			NewSystemData(Db4objects.Db4o.Internal.Freespace.FreespaceManager.FM_LEGACY_RAM);
			BlockSizeReadFromFile(1);
			_fileHeader = FileHeader.ReadFixedPart(this);
			CreateStringIO(_systemData.StringEncoding());
			ClassCollection().SetID(_systemData.ClassCollectionID());
			ClassCollection().Read(SystemTransaction());
			Converter.Convert(new ConversionStage.ClassCollectionAvailableStage(this));
			ReadHeaderVariablePart();
			_freespaceManager = Db4objects.Db4o.Internal.Freespace.FreespaceManager.CreateNew
				(this, _systemData.FreespaceSystem());
			_freespaceManager.Read(_systemData.FreespaceID());
			_freespaceManager.Start(_systemData.FreespaceAddress());
			if (_freespaceManager.RequiresMigration(ConfigImpl().FreespaceSystem(), _systemData
				.FreespaceSystem()))
			{
				Db4objects.Db4o.Internal.Freespace.FreespaceManager oldFreespaceManager = _freespaceManager;
				_freespaceManager = Db4objects.Db4o.Internal.Freespace.FreespaceManager.CreateNew
					(this, _systemData.FreespaceSystem());
				_freespaceManager.Start(NewFreespaceSlot(_systemData.FreespaceSystem()));
				Db4objects.Db4o.Internal.Freespace.FreespaceManager.Migrate(oldFreespaceManager, 
					_freespaceManager);
				_fileHeader.WriteVariablePart(this, 1);
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
				GetTransaction().Commit();
			}
		}

		private void ReadHeaderVariablePart()
		{
			_fileHeader.ReadVariablePart(this);
			SetNextTimeStampId(SystemData().LastTimeStampID());
		}

		public virtual int NewFreespaceSlot(byte freespaceSystem)
		{
			_systemData.FreespaceAddress(Db4objects.Db4o.Internal.Freespace.FreespaceManager.
				InitSlot(this));
			_systemData.FreespaceSystem(freespaceSystem);
			return _systemData.FreespaceAddress();
		}

		public virtual void EnsureFreespaceSlot()
		{
			if (SystemData().FreespaceAddress() == 0)
			{
				NewFreespaceSlot(SystemData().FreespaceSystem());
			}
		}

		public override void ReleaseSemaphore(string name)
		{
			ReleaseSemaphore(CheckTransaction(null), name);
		}

		public virtual void ReleaseSemaphore(Transaction ta, string name)
		{
			if (i_semaphores != null)
			{
				lock (i_semaphores)
				{
					if (i_semaphores != null && ta == i_semaphores.Get(name))
					{
						i_semaphores.Remove(name);
						Sharpen.Runtime.NotifyAll(i_semaphores);
					}
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
					semaphores.ForEachKeyForIdentity(new _AnonymousInnerClass596(this, semaphores), ta
						);
					Sharpen.Runtime.NotifyAll(semaphores);
				}
			}
		}

		private sealed class _AnonymousInnerClass596 : IVisitor4
		{
			public _AnonymousInnerClass596(LocalObjectContainer _enclosing, Hashtable4 semaphores
				)
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

		public sealed override void Rollback1()
		{
			GetTransaction().Rollback();
		}

		public sealed override void SetDirtyInSystemTransaction(PersistentBase a_object)
		{
			a_object.SetStateDirty();
			a_object.CacheDirty(i_dirty);
		}

		public override bool SetSemaphore(string name, int timeout)
		{
			return SetSemaphore(CheckTransaction(null), name, timeout);
		}

		public virtual bool SetSemaphore(Transaction ta, string name, int timeout)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			lock (i_lock)
			{
				if (i_semaphores == null)
				{
					i_semaphores = new Hashtable4(10);
				}
			}
			lock (i_semaphores)
			{
				object obj = i_semaphores.Get(name);
				if (obj == null)
				{
					i_semaphores.Put(name, ta);
					return true;
				}
				if (ta == obj)
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
					catch (Exception e)
					{
					}
					if (ClassCollection() == null)
					{
						return false;
					}
					obj = i_semaphores.Get(name);
					if (obj == null)
					{
						i_semaphores.Put(name, ta);
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
			if (i_config.IsReadOnly())
			{
				return;
			}
			WriteHeader(false, true);
		}

		public virtual void CommitTransaction()
		{
			if (i_config.IsReadOnly())
			{
				return;
			}
			i_trans.Commit();
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

		public sealed override void WriteEmbedded(StatefulBuffer a_parent, StatefulBuffer
			 a_child)
		{
			int length = a_child.GetLength();
			int address = GetSlot(length);
			a_child.GetTransaction().SlotFreeOnRollback(address, address, length);
			a_child.Address(address);
			a_child.WriteEncrypt();
			int offsetBackup = a_parent._offset;
			a_parent._offset = a_child.GetID();
			a_parent.WriteInt(address);
			a_parent._offset = offsetBackup;
		}

		internal virtual void WriteHeader(bool startFileLockingThread, bool shuttingDown)
		{
			int freespaceID = DEFAULT_FREESPACE_ID;
			if (shuttingDown)
			{
				freespaceID = _freespaceManager.Shutdown();
				_freespaceManager = null;
			}
			if (Debug.freespace && Debug.freespaceChecker)
			{
				freespaceID = _fmChecker.Shutdown();
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

		public sealed override void WriteNew(ClassMetadata a_yapClass, StatefulBuffer aWriter
			)
		{
			aWriter.WriteEncrypt(this, aWriter.GetAddress(), 0);
			if (a_yapClass == null)
			{
				return;
			}
			if (MaintainsIndices())
			{
				a_yapClass.AddToIndex(this, aWriter.GetTransaction(), aWriter.GetID());
			}
		}

		public abstract void OverwriteDeletedBytes(int a_address, int a_length);

		public sealed override void WriteTransactionPointer(int address)
		{
			_fileHeader.WriteTransactionPointer(SystemTransaction(), address);
		}

		public void GetSlotForUpdate(StatefulBuffer forWriter)
		{
			Transaction trans = forWriter.GetTransaction();
			int id = forWriter.GetID();
			int length = forWriter.GetLength();
			int address = GetSlot(length);
			forWriter.Address(address);
			trans.ProduceUpdateSlotChange(id, address, length);
		}

		public sealed override void WriteUpdate(ClassMetadata a_yapClass, StatefulBuffer 
			a_bytes)
		{
			if (a_bytes.GetAddress() == 0)
			{
				GetSlotForUpdate(a_bytes);
			}
			a_bytes.WriteEncrypt();
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

		public virtual void InstallDebugFreespaceManager(Db4objects.Db4o.Internal.Freespace.FreespaceManager
			 manager)
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
			clazz.Index().TraverseAll(trans, new _AnonymousInnerClass815(this, ids));
			return ids.AsLong();
		}

		private sealed class _AnonymousInnerClass815 : IVisitor4
		{
			public _AnonymousInnerClass815(LocalObjectContainer _enclosing, IntArrayList ids)
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
			if (!clazz.HasIndex())
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
	}
}
