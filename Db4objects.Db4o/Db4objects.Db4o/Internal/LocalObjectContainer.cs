namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class LocalObjectContainer : Db4objects.Db4o.Internal.ObjectContainerBase
	{
		protected Db4objects.Db4o.Internal.Fileheader.FileHeader _fileHeader;

		private Db4objects.Db4o.Foundation.Collection4 i_dirty;

		private Db4objects.Db4o.Internal.Freespace.FreespaceManager _freespaceManager;

		private Db4objects.Db4o.Internal.Freespace.FreespaceManager _fmChecker;

		private bool i_isServer = false;

		private Db4objects.Db4o.Foundation.Tree i_prefetchedIDs;

		private Db4objects.Db4o.Foundation.Hashtable4 i_semaphores;

		private int _blockEndAddress;

		private Db4objects.Db4o.Foundation.Tree _freeOnCommit;

		private Db4objects.Db4o.Internal.SystemData _systemData;

		internal LocalObjectContainer(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Internal.ObjectContainerBase
			 a_parent) : base(config, a_parent)
		{
		}

		public override Db4objects.Db4o.Internal.Transaction NewTransaction(Db4objects.Db4o.Internal.Transaction
			 parentTransaction)
		{
			return new Db4objects.Db4o.Internal.LocalTransaction(this, parentTransaction);
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

		protected override void Close2()
		{
			base.Close2();
			i_dirty = null;
		}

		public override void Commit1()
		{
			try
			{
				Write(false);
			}
			catch (System.Exception e)
			{
				FatalException(e);
			}
		}

		internal virtual void ConfigureNewFile()
		{
			NewSystemData(ConfigImpl().FreespaceSystem());
			SystemData().ConverterVersion(Db4objects.Db4o.Internal.Convert.Converter.VERSION);
			CreateStringIO(_systemData.StringEncoding());
			GenerateNewIdentity();
			_freespaceManager = Db4objects.Db4o.Internal.Freespace.FreespaceManager.CreateNew
				(this);
			BlockSize(ConfigImpl().BlockSize());
			_fileHeader = new Db4objects.Db4o.Internal.Fileheader.FileHeader1();
			SetRegularEndAddress(_fileHeader.Length());
			InitNewClassCollection();
			InitializeEssentialClasses();
			_fileHeader.InitNew(this);
			_freespaceManager.OnNew(this);
			_freespaceManager.Start(_systemData.FreespaceAddress());
			if (Db4objects.Db4o.Debug.freespace && Db4objects.Db4o.Debug.freespaceChecker)
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

		public Db4objects.Db4o.Internal.Btree.BTree CreateBTreeClassIndex(int id)
		{
			return new Db4objects.Db4o.Internal.Btree.BTree(i_trans, id, new Db4objects.Db4o.Internal.IDHandler
				(this));
		}

		public Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult NewQueryResult(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			return NewQueryResult(trans, Config().QueryEvaluationMode());
		}

		public sealed override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult 
			NewQueryResult(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Config.QueryEvaluationMode
			 mode)
		{
			if (mode == Db4objects.Db4o.Config.QueryEvaluationMode.IMMEDIATE)
			{
				return new Db4objects.Db4o.Internal.Query.Result.IdListQueryResult(trans);
			}
			return new Db4objects.Db4o.Internal.Query.Result.HybridQueryResult(trans, mode);
		}

		public sealed override bool Delete4(Db4objects.Db4o.Internal.Transaction ta, Db4objects.Db4o.Internal.ObjectReference
			 yo, int a_cascade, bool userCall)
		{
			int id = yo.GetID();
			Db4objects.Db4o.Internal.StatefulBuffer reader = ReadWriterByID(ta, id);
			if (reader != null)
			{
				object obj = yo.GetObject();
				if (obj != null)
				{
					if ((!ShowInternalClasses()) && Db4objects.Db4o.Internal.Const4.CLASS_INTERNAL.IsAssignableFrom
						(obj.GetType()))
					{
						return false;
					}
				}
				reader.SetCascadeDeletes(a_cascade);
				reader.SlotDelete();
				Db4objects.Db4o.Internal.ClassMetadata yc = yo.GetYapClass();
				yc.Delete(reader, obj);
				return true;
			}
			return false;
		}

		public abstract long FileLength();

		internal abstract string FileName();

		public virtual void Free(Db4objects.Db4o.Internal.Slots.Slot slot)
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
			if (Db4objects.Db4o.Debug.freespace && Db4objects.Db4o.Debug.freespaceChecker)
			{
				_fmChecker.Free(a_address, a_length);
			}
		}

		internal void FreePrefetchedPointers()
		{
			if (i_prefetchedIDs != null)
			{
				i_prefetchedIDs.Traverse(new _AnonymousInnerClass211(this));
			}
			i_prefetchedIDs = null;
		}

		private sealed class _AnonymousInnerClass211 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass211(LocalObjectContainer _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.Free(((Db4objects.Db4o.Internal.TreeInt)a_object)._key, Db4objects.Db4o.Internal.Const4
					.POINTER_LENGTH);
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
				SetIdentity(Db4objects.Db4o.Ext.Db4oDatabase.Generate());
			}
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult GetAll(
			Db4objects.Db4o.Internal.Transaction trans)
		{
			return GetAll(trans, Config().QueryEvaluationMode());
		}

		public virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult GetAll(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Config.QueryEvaluationMode mode)
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = NewQueryResult
				(trans, mode);
			queryResult.LoadFromClassIndexes(ClassCollection().Iterator());
			return queryResult;
		}

		internal int GetPointerSlot()
		{
			int id = GetSlot(Db4objects.Db4o.Internal.Const4.POINTER_LENGTH);
			i_systemTrans.WritePointer(id, 0, 0);
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
			Db4objects.Db4o.DTrace.GET_SLOT.LogLength(address, a_length);
			return address;
		}

		private int GetSlot1(int bytes)
		{
			if (_freespaceManager != null)
			{
				int freeAddress = _freespaceManager.GetSlot(bytes);
				if (Db4objects.Db4o.Debug.freespace && Db4objects.Db4o.Debug.freespaceChecker)
				{
					if (freeAddress > 0)
					{
						Db4objects.Db4o.Foundation.Collection4 wrongOnes = new Db4objects.Db4o.Foundation.Collection4
							();
						int freeCheck = _fmChecker.GetSlot(bytes);
						while (freeCheck != freeAddress && freeCheck > 0)
						{
							wrongOnes.Add(new int[] { freeCheck, bytes });
							freeCheck = _fmChecker.GetSlot(bytes);
						}
						System.Collections.IEnumerator i = wrongOnes.GetEnumerator();
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
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
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
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(69);
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
				Db4objects.Db4o.Internal.StatefulBuffer writer = GetWriter(i_systemTrans, _blockEndAddress
					 - 1, BlockSize());
				writer.Write();
			}
		}

		public override Db4objects.Db4o.Ext.Db4oDatabase Identity()
		{
			return _systemData.Identity();
		}

		public virtual void SetIdentity(Db4objects.Db4o.Ext.Db4oDatabase identity)
		{
			_systemData.Identity(identity);
			_timeStampIdGenerator.Next();
		}

		internal override void Initialize2()
		{
			i_dirty = new Db4objects.Db4o.Foundation.Collection4();
			base.Initialize2();
		}

		internal override bool IsServer()
		{
			return i_isServer;
		}

		public Db4objects.Db4o.Internal.Slots.Pointer4 NewSlot(Db4objects.Db4o.Internal.Transaction
			 a_trans, int a_length)
		{
			int id = GetPointerSlot();
			int address = GetSlot(a_length);
			a_trans.SetPointer(id, address, a_length);
			return new Db4objects.Db4o.Internal.Slots.Pointer4(id, address);
		}

		public sealed override int NewUserObject()
		{
			return GetPointerSlot();
		}

		public virtual void PrefetchedIDConsumed(int a_id)
		{
			i_prefetchedIDs = i_prefetchedIDs.RemoveLike(new Db4objects.Db4o.Internal.TreeIntObject
				(a_id));
		}

		public virtual int PrefetchID()
		{
			int id = GetPointerSlot();
			i_prefetchedIDs = Db4objects.Db4o.Foundation.Tree.Add(i_prefetchedIDs, new Db4objects.Db4o.Internal.TreeInt
				(id));
			return id;
		}

		public virtual Db4objects.Db4o.Internal.Slots.ReferencedSlot ProduceFreeOnCommitEntry
			(int id)
		{
			Db4objects.Db4o.Foundation.Tree node = Db4objects.Db4o.Internal.TreeInt.Find(_freeOnCommit
				, id);
			if (node != null)
			{
				return (Db4objects.Db4o.Internal.Slots.ReferencedSlot)node;
			}
			Db4objects.Db4o.Internal.Slots.ReferencedSlot slot = new Db4objects.Db4o.Internal.Slots.ReferencedSlot
				(id);
			_freeOnCommit = Db4objects.Db4o.Foundation.Tree.Add(_freeOnCommit, slot);
			return slot;
		}

		public virtual void ReduceFreeOnCommitReferences(Db4objects.Db4o.Internal.Slots.ReferencedSlot
			 slot)
		{
			if (slot.RemoveReferenceIsLast())
			{
				_freeOnCommit = _freeOnCommit.RemoveNode(slot);
			}
		}

		public virtual void FreeDuringCommit(Db4objects.Db4o.Internal.Slots.ReferencedSlot
			 referencedSlot, Db4objects.Db4o.Internal.Slots.Slot slot)
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

		public override Db4objects.Db4o.Internal.StatefulBuffer ReadWriterByID(Db4objects.Db4o.Internal.Transaction
			 a_ta, int a_id)
		{
			return (Db4objects.Db4o.Internal.StatefulBuffer)ReadReaderOrWriterByID(a_ta, a_id
				, false);
		}

		public override Db4objects.Db4o.Internal.StatefulBuffer[] ReadWritersByIDs(Db4objects.Db4o.Internal.Transaction
			 a_ta, int[] ids)
		{
			Db4objects.Db4o.Internal.StatefulBuffer[] yapWriters = new Db4objects.Db4o.Internal.StatefulBuffer
				[ids.Length];
			for (int i = 0; i < ids.Length; ++i)
			{
				yapWriters[i] = (Db4objects.Db4o.Internal.StatefulBuffer)ReadReaderOrWriterByID(a_ta
					, ids[i], false);
			}
			return yapWriters;
		}

		public override Db4objects.Db4o.Internal.Buffer ReadReaderByID(Db4objects.Db4o.Internal.Transaction
			 a_ta, int a_id)
		{
			return ReadReaderOrWriterByID(a_ta, a_id, true);
		}

		private Db4objects.Db4o.Internal.Buffer ReadReaderOrWriterByID(Db4objects.Db4o.Internal.Transaction
			 a_ta, int a_id, bool useReader)
		{
			if (a_id == 0)
			{
				return null;
			}
			try
			{
				Db4objects.Db4o.Internal.Slots.Slot slot = ((Db4objects.Db4o.Internal.LocalTransaction
					)a_ta).GetCurrentSlotOfID(a_id);
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
					((Db4objects.Db4o.Internal.StatefulBuffer)reader).SetID(a_id);
				}
				reader.ReadEncrypt(this, slot._address);
				return reader;
			}
			catch (System.Exception e)
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
			_fileHeader = Db4objects.Db4o.Internal.Fileheader.FileHeader.ReadFixedPart(this);
			CreateStringIO(_systemData.StringEncoding());
			ClassCollection().SetID(_systemData.ClassCollectionID());
			ClassCollection().Read(i_systemTrans);
			Db4objects.Db4o.Internal.Convert.Converter.Convert(new Db4objects.Db4o.Internal.Convert.ConversionStage.ClassCollectionAvailableStage
				(this));
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
			Db4objects.Db4o.Internal.LocalTransaction trans = (Db4objects.Db4o.Internal.LocalTransaction
				)_fileHeader.InterruptedTransaction();
			if (trans != null)
			{
				if (!ConfigImpl().CommitRecoveryDisabled())
				{
					trans.WriteOld();
				}
			}
			if (Db4objects.Db4o.Internal.Convert.Converter.Convert(new Db4objects.Db4o.Internal.Convert.ConversionStage.SystemUpStage
				(this)))
			{
				_systemData.ConverterVersion(Db4objects.Db4o.Internal.Convert.Converter.VERSION);
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

		public virtual void ReleaseSemaphore(Db4objects.Db4o.Internal.Transaction ta, string
			 name)
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

		public override void ReleaseSemaphores(Db4objects.Db4o.Internal.Transaction ta)
		{
			if (i_semaphores != null)
			{
				Db4objects.Db4o.Foundation.Hashtable4 semaphores = i_semaphores;
				lock (semaphores)
				{
					semaphores.ForEachKeyForIdentity(new _AnonymousInnerClass589(this, semaphores), ta
						);
					Sharpen.Runtime.NotifyAll(semaphores);
				}
			}
		}

		private sealed class _AnonymousInnerClass589 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass589(LocalObjectContainer _enclosing, Db4objects.Db4o.Foundation.Hashtable4
				 semaphores)
			{
				this._enclosing = _enclosing;
				this.semaphores = semaphores;
			}

			public void Visit(object a_object)
			{
				semaphores.Remove(a_object);
			}

			private readonly LocalObjectContainer _enclosing;

			private readonly Db4objects.Db4o.Foundation.Hashtable4 semaphores;
		}

		public sealed override void Rollback1()
		{
			GetTransaction().Rollback();
		}

		public sealed override void SetDirtyInSystemTransaction(Db4objects.Db4o.Internal.PersistentBase
			 a_object)
		{
			a_object.SetStateDirty();
			a_object.CacheDirty(i_dirty);
		}

		public override bool SetSemaphore(string name, int timeout)
		{
			return SetSemaphore(CheckTransaction(null), name, timeout);
		}

		public virtual bool SetSemaphore(Db4objects.Db4o.Internal.Transaction ta, string 
			name, int timeout)
		{
			if (name == null)
			{
				throw new System.ArgumentNullException();
			}
			lock (i_lock)
			{
				if (i_semaphores == null)
				{
					i_semaphores = new Db4objects.Db4o.Foundation.Hashtable4(10);
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
				long endtime = Sharpen.Runtime.CurrentTimeMillis() + timeout;
				long waitTime = timeout;
				while (waitTime > 0)
				{
					try
					{
						Sharpen.Runtime.Wait(i_semaphores, waitTime);
					}
					catch (System.Exception e)
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
					waitTime = endtime - Sharpen.Runtime.CurrentTimeMillis();
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

		public override void Write(bool shuttingDown)
		{
			if (i_config.IsReadOnly())
			{
				return;
			}
			i_trans.Commit();
			if (shuttingDown)
			{
				WriteHeader(false, true);
			}
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
			System.Collections.IEnumerator i = i_dirty.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.PersistentBase dirty = (Db4objects.Db4o.Internal.PersistentBase
					)i.Current;
				dirty.Write(i_systemTrans);
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

		public sealed override void WriteEmbedded(Db4objects.Db4o.Internal.StatefulBuffer
			 a_parent, Db4objects.Db4o.Internal.StatefulBuffer a_child)
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
			int freespaceID = _freespaceManager.Write(shuttingDown);
			if (shuttingDown)
			{
				_freespaceManager = null;
			}
			if (Db4objects.Db4o.Debug.freespace && Db4objects.Db4o.Debug.freespaceChecker)
			{
				freespaceID = _fmChecker.Write(shuttingDown);
			}
			Db4objects.Db4o.Internal.StatefulBuffer writer = GetWriter(i_systemTrans, 0, _fileHeader
				.Length());
			_fileHeader.WriteFixedPart(this, startFileLockingThread, shuttingDown, writer, BlockSize
				(), freespaceID);
			if (shuttingDown)
			{
				EnsureLastSlotWritten();
			}
			SyncFiles();
		}

		public sealed override void WriteNew(Db4objects.Db4o.Internal.ClassMetadata a_yapClass
			, Db4objects.Db4o.Internal.StatefulBuffer aWriter)
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
			_fileHeader.WriteTransactionPointer(GetSystemTransaction(), address);
		}

		public void GetSlotForUpdate(Db4objects.Db4o.Internal.StatefulBuffer forWriter)
		{
			Db4objects.Db4o.Internal.Transaction trans = forWriter.GetTransaction();
			int id = forWriter.GetID();
			int length = forWriter.GetLength();
			int address = GetSlot(length);
			forWriter.Address(address);
			trans.SlotFreeOnRollbackSetPointer(id, address, length);
		}

		public sealed override void WriteUpdate(Db4objects.Db4o.Internal.ClassMetadata a_yapClass
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes)
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

		public override Db4objects.Db4o.Ext.ISystemInfo SystemInfo()
		{
			return new Db4objects.Db4o.Internal.SystemInfoFileImpl(this);
		}

		public virtual Db4objects.Db4o.Internal.Fileheader.FileHeader GetFileHeader()
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

		public override long[] GetIDsForClass(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.ClassMetadata clazz)
		{
			Db4objects.Db4o.Foundation.IntArrayList ids = new Db4objects.Db4o.Foundation.IntArrayList
				();
			clazz.Index().TraverseAll(trans, new _AnonymousInnerClass804(this, ids));
			return ids.AsLong();
		}

		private sealed class _AnonymousInnerClass804 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass804(LocalObjectContainer _enclosing, Db4objects.Db4o.Foundation.IntArrayList
				 ids)
			{
				this._enclosing = _enclosing;
				this.ids = ids;
			}

			public void Visit(object obj)
			{
				ids.Add(((int)obj));
			}

			private readonly LocalObjectContainer _enclosing;

			private readonly Db4objects.Db4o.Foundation.IntArrayList ids;
		}

		public override Db4objects.Db4o.Internal.Query.Result.IQueryResult ClassOnlyQuery
			(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.ClassMetadata
			 clazz)
		{
			if (!clazz.HasIndex())
			{
				return null;
			}
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = NewQueryResult
				(trans);
			queryResult.LoadFromClassIndex(clazz);
			return queryResult;
		}

		public override Db4objects.Db4o.Internal.Query.Result.IQueryResult ExecuteQuery(Db4objects.Db4o.Internal.Query.Processor.QQuery
			 query)
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = NewQueryResult
				(query.GetTransaction());
			queryResult.LoadFromQuery(query);
			return queryResult;
		}
	}
}
