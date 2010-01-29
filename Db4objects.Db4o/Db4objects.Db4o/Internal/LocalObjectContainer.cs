/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

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
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Internal.References;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class LocalObjectContainer : ExternalObjectContainer, IInternalObjectContainer
		, IEmbeddedObjectContainer
	{
		private const int DefaultFreespaceId = 0;

		protected FileHeader _fileHeader;

		private Collection4 i_dirty;

		private IFreespaceManager _freespaceManager;

		private bool i_isServer = false;

		private Lock4 _semaphoresLock = new Lock4();

		private Hashtable4 _semaphores;

		private int _blockEndAddress;

		private Db4objects.Db4o.Internal.SystemData _systemData;

		private readonly IIdSystem _idSystem;

		private readonly byte[] _pointerBuffer = new byte[Const4.PointerLength];

		protected readonly ByteArrayBuffer _pointerIo = new ByteArrayBuffer(Const4.PointerLength
			);

		internal LocalObjectContainer(IConfiguration config) : base(config)
		{
			_idSystem = NewIdSystem();
		}

		public override Transaction NewTransaction(Transaction parentTransaction, IReferenceSystem
			 referenceSystem, bool isSystemTransaction)
		{
			LocalTransaction transaction = new LocalTransaction(this, parentTransaction, referenceSystem
				);
			if (isSystemTransaction)
			{
				IdSystem().SystemTransaction(transaction);
			}
			else
			{
				IdSystem().AddTransaction(transaction);
			}
			return transaction;
		}

		protected virtual IIdSystem NewIdSystem()
		{
			return new StandardIdSystem(this);
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
			try
			{
				if (!_config.IsReadOnly())
				{
					CommitTransaction();
					Shutdown();
				}
			}
			finally
			{
				ShutdownObjectContainer();
			}
		}

		public override void Commit1(Transaction trans)
		{
			trans.Commit();
		}

		internal virtual void ConfigureNewFile()
		{
			NewSystemData(ConfigImpl.FreespaceSystem());
			SystemData().ConverterVersion(Converter.Version);
			CreateStringIO(_systemData.StringEncoding());
			GenerateNewIdentity();
			_freespaceManager = AbstractFreespaceManager.CreateNew(this);
			BlockSize(ConfigImpl.BlockSize());
			_fileHeader = new FileHeader1();
			SetRegularEndAddress(_fileHeader.Length());
			InitNewClassCollection();
			InitializeEssentialClasses();
			_fileHeader.InitNew(this);
			_freespaceManager.Start(_systemData.FreespaceAddress());
		}

		private void NewSystemData(byte freespaceSystem)
		{
			_systemData = new Db4objects.Db4o.Internal.SystemData();
			_systemData.StringEncoding(ConfigImpl.Encoding());
			_systemData.FreespaceSystem(freespaceSystem);
		}

		public override int ConverterVersion()
		{
			return _systemData.ConverterVersion();
		}

		public override long CurrentVersion()
		{
			return _timeStampIdGenerator.LastTimeStampId();
		}

		internal virtual void InitNewClassCollection()
		{
			// overridden in YapObjectCarrier to do nothing
			ClassCollection().InitTables(1);
		}

		public BTree CreateBTreeClassIndex(int id)
		{
			return new BTree(_transaction, id, new IDHandler());
		}

		public AbstractQueryResult NewQueryResult(Transaction trans)
		{
			return NewQueryResult(trans, Config().EvaluationMode());
		}

		public sealed override AbstractQueryResult NewQueryResult(Transaction trans, QueryEvaluationMode
			 mode)
		{
			if (trans == null)
			{
				throw new ArgumentNullException();
			}
			if (mode == QueryEvaluationMode.Immediate)
			{
				return new IdListQueryResult(trans);
			}
			return new HybridQueryResult(trans, mode);
		}

		public sealed override bool Delete4(Transaction transaction, ObjectReference @ref
			, object obj, int cascade, bool userCall)
		{
			int id = @ref.GetID();
			StatefulBuffer reader = ReadWriterByID(transaction, id);
			if (reader != null)
			{
				if (obj != null)
				{
					if ((!ShowInternalClasses()) && Const4.ClassInternal.IsAssignableFrom(obj.GetType
						()))
					{
						return false;
					}
				}
				reader.SetCascadeDeletes(cascade);
				IdSystem().NotifySlotDeleted(transaction, id, SlotChangeFactory.UserObjects);
				ClassMetadata classMetadata = @ref.ClassMetadata();
				classMetadata.Delete(reader, obj);
				return true;
			}
			return false;
		}

		public abstract long FileLength();

		public abstract string FileName();

		public virtual void Free(Slot slot)
		{
			if (slot.IsNull())
			{
				return;
			}
			// TODO: This should really be an IllegalArgumentException but old database files 
			//       with index-based FreespaceManagers appear to deliver zeroed slots.
			// throw new IllegalArgumentException();
			if (_freespaceManager == null)
			{
				// Can happen on early free before freespacemanager
				// is up, during conversion.
				return;
			}
			Slot blockedSlot = ToBlockedLength(slot);
			if (DTrace.enabled)
			{
				DTrace.FileFree.LogLength(blockedSlot.Address(), blockedSlot.Length());
			}
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

		public virtual void GenerateNewIdentity()
		{
			lock (_lock)
			{
				SetIdentity(Db4oDatabase.Generate());
			}
		}

		public override AbstractQueryResult QueryAllObjects(Transaction trans)
		{
			return GetAll(trans, Config().EvaluationMode());
		}

		public virtual AbstractQueryResult GetAll(Transaction trans, QueryEvaluationMode 
			mode)
		{
			AbstractQueryResult queryResult = NewQueryResult(trans, mode);
			queryResult.LoadFromClassIndexes(ClassCollection().Iterator());
			return queryResult;
		}

		public virtual int AllocatePointerSlot()
		{
			int id = AllocateSlot(Const4.PointerLength).Address();
			if (!IsValidPointer(id))
			{
				return AllocatePointerSlot();
			}
			// write a zero pointer first
			// to prevent delete interaction trouble
			WritePointer(id, Slot.Zero);
			if (DTrace.enabled)
			{
				DTrace.GetPointerSlot.Log(id);
			}
			return id;
		}

		protected virtual bool IsValidPointer(int id)
		{
			// We have to make sure that object IDs do not collide
			// with built-in type IDs.
			return !_handlers.IsSystemHandler(id);
		}

		public virtual Slot AllocateSlot(int length)
		{
			int blocks = BytesToBlocks(length);
			Slot slot = AllocateBlockedSlot(blocks);
			if (DTrace.enabled)
			{
				DTrace.GetSlot.LogLength(slot.Address(), slot.Length());
			}
			return ToNonBlockedLength(slot);
		}

		private Slot AllocateBlockedSlot(int blocks)
		{
			if (blocks <= 0)
			{
				throw new ArgumentException();
			}
			if (_freespaceManager != null)
			{
				Slot slot = _freespaceManager.AllocateSlot(blocks);
				if (slot != null)
				{
					return slot;
				}
				while (GrowDatabaseByConfiguredSize())
				{
					slot = _freespaceManager.AllocateSlot(blocks);
					if (slot != null)
					{
						return slot;
					}
				}
			}
			return AppendBlocks(blocks);
		}

		private bool GrowDatabaseByConfiguredSize()
		{
			int reservedStorageSpace = ConfigImpl.DatabaseGrowthSize();
			if (reservedStorageSpace <= 0)
			{
				return false;
			}
			int reservedBlocks = BytesToBlocks(reservedStorageSpace);
			int reservedBytes = BlocksToBytes(reservedBlocks);
			Slot slot = new Slot(_blockEndAddress, reservedBlocks);
			if (Debug4.xbytes && Deploy.overwrite)
			{
				OverwriteDeletedBlockedSlot(slot);
			}
			else
			{
				WriteBytes(new ByteArrayBuffer(reservedBytes), _blockEndAddress, 0);
			}
			_freespaceManager.Free(slot);
			_blockEndAddress += reservedBlocks;
			return true;
		}

		protected Slot AppendBlocks(int blockCount)
		{
			int blockedStartAddress = _blockEndAddress;
			int blockedEndAddress = _blockEndAddress + blockCount;
			CheckBlockedAddress(blockedEndAddress);
			_blockEndAddress = blockedEndAddress;
			Slot slot = new Slot(blockedStartAddress, blockCount);
			if (Debug4.xbytes && Deploy.overwrite)
			{
				OverwriteDeletedBlockedSlot(slot);
			}
			return slot;
		}

		public Slot AppendBytes(long bytes)
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

		// When a file gets opened, it uses the file size to determine where 
		// new slots can be appended. If this method would not be called, the
		// freespace system could already contain a slot that points beyond
		// the end of the file and this space could be allocated and used twice,
		// for instance if a slot was allocated and freed without ever being
		// written to file.
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
			// The dirty TimeStampIdGenerator triggers writing of
			// the variable part of the systemdata. We need to
			// make it dirty here, so the new identity is persisted:
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
			return new Pointer4(AllocatePointerSlot(), AllocateSlot(length));
		}

		public sealed override int IdForNewUserObject(Transaction trans)
		{
			return IdSystem().NewId(trans, SlotChangeFactory.UserObjects);
		}

		public override void RaiseVersion(long a_minimumVersion)
		{
			lock (Lock())
			{
				_timeStampIdGenerator.SetMinimumNext(a_minimumVersion);
			}
		}

		public override StatefulBuffer ReadWriterByID(Transaction transaction, int id, bool
			 lastCommitted)
		{
			return (StatefulBuffer)ReadReaderOrWriterByID((LocalTransaction)transaction, id, 
				false, lastCommitted);
		}

		public override StatefulBuffer ReadWriterByID(Transaction a_ta, int a_id)
		{
			return ReadWriterByID(a_ta, a_id, false);
		}

		public override ByteArrayBuffer[] ReadSlotBuffers(Transaction transaction, int[] 
			ids)
		{
			ByteArrayBuffer[] buffers = new ByteArrayBuffer[ids.Length];
			for (int i = 0; i < ids.Length; ++i)
			{
				if (ids[i] == 0)
				{
					buffers[i] = null;
				}
				else
				{
					buffers[i] = ReadReaderOrWriterByID((LocalTransaction)transaction, ids[i], true);
				}
			}
			return buffers;
		}

		public override ByteArrayBuffer ReadReaderByID(Transaction transaction, int id, bool
			 lastCommitted)
		{
			return ReadReaderOrWriterByID((LocalTransaction)transaction, id, true, lastCommitted
				);
		}

		public override ByteArrayBuffer ReadReaderByID(Transaction trans, int id)
		{
			return ReadReaderByID(trans, id, false);
		}

		private ByteArrayBuffer ReadReaderOrWriterByID(LocalTransaction transaction, int 
			id, bool useReader)
		{
			return ReadReaderOrWriterByID(transaction, id, useReader, false);
		}

		private ByteArrayBuffer ReadReaderOrWriterByID(LocalTransaction trans, int id, bool
			 useReader, bool lastCommitted)
		{
			if (id <= 0)
			{
				throw new ArgumentException();
			}
			if (DTrace.enabled)
			{
				DTrace.ReadId.Log(id);
			}
			Slot slot = lastCommitted ? IdSystem().GetCommittedSlotOfID(id) : IdSystem().GetCurrentSlotOfID
				(trans, id);
			return ReadReaderOrWriterBySlot(trans, id, useReader, slot);
		}

		public virtual ByteArrayBuffer ReadSlotBuffer(Slot slot)
		{
			ByteArrayBuffer reader = new ByteArrayBuffer(slot.Length());
			reader.ReadEncrypt(this, slot.Address());
			reader.Skip(0);
			return reader;
		}

		internal virtual ByteArrayBuffer ReadReaderOrWriterBySlot(Transaction a_ta, int a_id
			, bool useReader, Slot slot)
		{
			if (slot == null)
			{
				return null;
			}
			if (slot.IsNull())
			{
				return null;
			}
			if (DTrace.enabled)
			{
				DTrace.ReadSlot.LogLength(slot.Address(), slot.Length());
			}
			ByteArrayBuffer reader = null;
			if (useReader)
			{
				reader = new ByteArrayBuffer(slot.Length());
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

		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		internal virtual void ReadThis()
		{
			NewSystemData(AbstractFreespaceManager.FmLegacyRam);
			BlockSizeReadFromFile(1);
			_fileHeader = FileHeader.ReadFixedPart(this);
			CreateStringIO(_systemData.StringEncoding());
			ClassCollection().SetID(_systemData.ClassCollectionID());
			ClassCollection().Read(SystemTransaction());
			Converter.Convert(new ConversionStage.ClassCollectionAvailableStage(this));
			ReadHeaderVariablePart();
			if (_config.IsReadOnly())
			{
				return;
			}
			_freespaceManager = AbstractFreespaceManager.CreateNew(this, _systemData.FreespaceSystem
				());
			_freespaceManager.Read(_systemData.FreespaceID());
			_freespaceManager.Start(_systemData.FreespaceAddress());
			if (FreespaceMigrationRequired())
			{
				MigrateFreespace();
			}
			WriteHeader(true, false);
			IInterruptedTransactionHandler interruptedTransactionHandler = _fileHeader.InterruptedTransactionHandler
				();
			if (interruptedTransactionHandler != null)
			{
				if (!ConfigImpl.CommitRecoveryDisabled())
				{
					interruptedTransactionHandler.CompleteInterruptedTransaction();
				}
			}
			if (Converter.Convert(new ConversionStage.SystemUpStage(this)))
			{
				_systemData.ConverterVersion(Converter.Version);
				_fileHeader.WriteVariablePart(this, 1);
				Transaction.Commit();
			}
		}

		private bool FreespaceMigrationRequired()
		{
			if (_freespaceManager == null)
			{
				return false;
			}
			byte readSystem = _systemData.FreespaceSystem();
			byte configuredSystem = ConfigImpl.FreespaceSystem();
			if (_freespaceManager.SystemType() == configuredSystem)
			{
				return false;
			}
			if (configuredSystem != 0)
			{
				return true;
			}
			return AbstractFreespaceManager.MigrationRequired(readSystem);
		}

		private void MigrateFreespace()
		{
			IFreespaceManager oldFreespaceManager = _freespaceManager;
			IFreespaceManager newFreespaceManager = AbstractFreespaceManager.CreateNew(this, 
				ConfigImpl.FreespaceSystem());
			newFreespaceManager.Start(0);
			SystemData().FreespaceSystem(ConfigImpl.FreespaceSystem());
			_freespaceManager = newFreespaceManager;
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
				if (_semaphores == null)
				{
					return;
				}
			}
			_semaphoresLock.Run(new _IClosure4_576(this, trans, name));
		}

		private sealed class _IClosure4_576 : IClosure4
		{
			public _IClosure4_576(LocalObjectContainer _enclosing, Transaction trans, string 
				name)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.name = name;
			}

			public object Run()
			{
				Transaction transaction = this._enclosing.CheckTransaction(trans);
				if (this._enclosing._semaphores != null && transaction == this._enclosing._semaphores
					.Get(name))
				{
					this._enclosing._semaphores.Remove(name);
				}
				this._enclosing._semaphoresLock.Awake();
				return null;
			}

			private readonly LocalObjectContainer _enclosing;

			private readonly Transaction trans;

			private readonly string name;
		}

		public override void ReleaseSemaphores(Transaction trans)
		{
			if (_semaphores != null)
			{
				Hashtable4 semaphores = _semaphores;
				_semaphoresLock.Run(new _IClosure4_590(this, semaphores, trans));
			}
		}

		private sealed class _IClosure4_590 : IClosure4
		{
			public _IClosure4_590(LocalObjectContainer _enclosing, Hashtable4 semaphores, Transaction
				 trans)
			{
				this._enclosing = _enclosing;
				this.semaphores = semaphores;
				this.trans = trans;
			}

			public object Run()
			{
				semaphores.ForEachKeyForIdentity(new _IVisitor4_591(semaphores), trans);
				this._enclosing._semaphoresLock.Awake();
				return null;
			}

			private sealed class _IVisitor4_591 : IVisitor4
			{
				public _IVisitor4_591(Hashtable4 semaphores)
				{
					this.semaphores = semaphores;
				}

				public void Visit(object a_object)
				{
					semaphores.Remove(a_object);
				}

				private readonly Hashtable4 semaphores;
			}

			private readonly LocalObjectContainer _enclosing;

			private readonly Hashtable4 semaphores;

			private readonly Transaction trans;
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
				if (_semaphores == null)
				{
					_semaphores = new Hashtable4(10);
				}
			}
			BooleanByRef acquired = new BooleanByRef();
			_semaphoresLock.Run(new _IClosure4_627(this, trans, name, acquired, timeout));
			return acquired.value;
		}

		private sealed class _IClosure4_627 : IClosure4
		{
			public _IClosure4_627(LocalObjectContainer _enclosing, Transaction trans, string 
				name, BooleanByRef acquired, int timeout)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.name = name;
				this.acquired = acquired;
				this.timeout = timeout;
			}

			public object Run()
			{
				try
				{
					Transaction transaction = this._enclosing.CheckTransaction(trans);
					object candidateTransaction = this._enclosing._semaphores.Get(name);
					if (trans == candidateTransaction)
					{
						acquired.value = true;
						return null;
					}
					if (candidateTransaction == null)
					{
						this._enclosing._semaphores.Put(name, transaction);
						acquired.value = true;
						return null;
					}
					long endtime = Runtime.CurrentTimeMillis() + timeout;
					long waitTime = timeout;
					while (waitTime > 0)
					{
						this._enclosing._semaphoresLock.Awake();
						this._enclosing._semaphoresLock.Snooze(waitTime);
						if (this._enclosing.ClassCollection() == null)
						{
							acquired.value = false;
							return null;
						}
						candidateTransaction = this._enclosing._semaphores.Get(name);
						if (candidateTransaction == null)
						{
							this._enclosing._semaphores.Put(name, transaction);
							acquired.value = true;
							return null;
						}
						waitTime = endtime - Runtime.CurrentTimeMillis();
					}
					acquired.value = false;
					return null;
				}
				finally
				{
					this._enclosing._semaphoresLock.Awake();
				}
			}

			private readonly LocalObjectContainer _enclosing;

			private readonly Transaction trans;

			private readonly string name;

			private readonly BooleanByRef acquired;

			private readonly int timeout;
		}

		public virtual void SetServer(bool flag)
		{
			i_isServer = flag;
		}

		public abstract void SyncFiles();

		protected override string DefaultToString()
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

		public abstract void WriteBytes(ByteArrayBuffer buffer, int blockedAddress, int addressOffset
			);

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

		public void WriteEncrypt(ByteArrayBuffer buffer, int address, int addressOffset)
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
			int freespaceID = DefaultFreespaceId;
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
			 classMetadata, ByteArrayBuffer buffer)
		{
			WriteEncrypt(buffer, pointer.Address(), 0);
			if (classMetadata == null)
			{
				return;
			}
			classMetadata.AddToIndex(trans, pointer.Id());
		}

		// This is a reroute of writeBytes to write the free blocks
		// unchecked.
		public abstract void OverwriteDeletedBytes(int address, int length);

		public virtual void OverwriteDeletedBlockedSlot(Slot slot)
		{
			OverwriteDeletedBytes(slot.Address(), BlocksToBytes(slot.Length()));
		}

		public sealed override void WriteTransactionPointer(int address)
		{
			_fileHeader.WriteTransactionPointer(SystemTransaction(), address);
		}

		public Slot AllocateSlotForUserObjectUpdate(Transaction trans, int id, int length
			)
		{
			Slot slot = AllocateSlot(length);
			IdSystem().NotifySlotChanged(trans, id, slot, SlotChangeFactory.UserObjects);
			return slot;
		}

		public Slot AllocateSlotForNewUserObject(Transaction trans, int id, int length)
		{
			Slot slot = AllocateSlot(length);
			IdSystem().NotifySlotCreated(trans, id, slot, SlotChangeFactory.UserObjects);
			return slot;
		}

		public sealed override void WriteUpdate(Transaction trans, Pointer4 pointer, ClassMetadata
			 classMetadata, ArrayType arrayType, ByteArrayBuffer buffer)
		{
			int address = pointer.Address();
			if (address == 0)
			{
				address = AllocateSlotForUserObjectUpdate(trans, pointer.Id(), pointer.Length()).
					Address();
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
			clazz.Index().TraverseAll(trans, new _IVisitor4_805(ids));
			return ids.AsLong();
		}

		private sealed class _IVisitor4_805 : IVisitor4
		{
			public _IVisitor4_805(IntArrayList ids)
			{
				this.ids = ids;
			}

			public void Visit(object obj)
			{
				ids.Add(((int)obj));
			}

			private readonly IntArrayList ids;
		}

		public override IQueryResult ClassOnlyQuery(QQueryBase query, ClassMetadata clazz
			)
		{
			if (!clazz.HasClassIndex())
			{
				return new IdListQueryResult(query.Transaction());
			}
			AbstractQueryResult queryResult = NewQueryResult(query.Transaction());
			queryResult.LoadFromClassIndex(clazz);
			return queryResult;
		}

		public override IQueryResult ExecuteQuery(QQuery query)
		{
			AbstractQueryResult queryResult = NewQueryResult(query.Transaction());
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

		// do nothing
		public override int InstanceCount(ClassMetadata clazz, Transaction trans)
		{
			lock (Lock())
			{
				return clazz.IndexEntryCount(trans);
			}
		}

		public virtual IObjectContainer OpenSession()
		{
			lock (Lock())
			{
				return new ObjectContainerSession(this);
			}
		}

		public virtual IIdSystem IdSystem()
		{
			return _idSystem;
		}

		public override bool IsDeleted(Transaction trans, int id)
		{
			return IdSystem().IsDeleted(trans, id);
		}

		public virtual void WritePointer(int id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.WritePointer.Log(id);
				DTrace.WritePointer.LogLength(slot);
			}
			_pointerIo.Seek(0);
			_pointerIo.WriteInt(slot.Address());
			_pointerIo.WriteInt(slot.Length());
			WriteBytes(_pointerIo, id, 0);
		}

		public virtual Pointer4 DebugReadPointer(int id)
		{
			return null;
		}

		public virtual Pointer4 ReadPointer(int id)
		{
			if (!IsValidId(id))
			{
				throw new InvalidIDException(id);
			}
			ReadBytes(_pointerBuffer, id, Const4.PointerLength);
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			if (!IsValidSlot(address, length))
			{
				throw new InvalidSlotException(address, length, id);
			}
			return new Pointer4(id, new Slot(address, length));
		}

		private bool IsValidId(int id)
		{
			return FileLength() >= id;
		}

		private bool IsValidSlot(int address, int length)
		{
			// just in case overflow 
			long fileLength = FileLength();
			bool validAddress = fileLength >= address;
			bool validLength = fileLength >= length;
			bool validSlot = fileLength >= (address + length);
			return validAddress && validLength && validSlot;
		}
	}
}
