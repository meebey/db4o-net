namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class YapFile : Db4objects.Db4o.YapStream
	{
		protected Db4objects.Db4o.Header.FileHeader _fileHeader;

		private Db4objects.Db4o.Foundation.Collection4 i_dirty;

		private Db4objects.Db4o.Inside.Freespace.FreespaceManager _freespaceManager;

		private Db4objects.Db4o.Inside.Freespace.FreespaceManager _fmChecker;

		private bool i_isServer = false;

		private Db4objects.Db4o.Foundation.Tree i_prefetchedIDs;

		private Db4objects.Db4o.Foundation.Hashtable4 i_semaphores;

		private int _blockEndAddress;

		private Db4objects.Db4o.Foundation.Tree _freeOnCommit;

		private Db4objects.Db4o.Inside.SystemData _systemData;

		internal YapFile(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.YapStream
			 a_parent) : base(config, a_parent)
		{
		}

		public virtual Db4objects.Db4o.Inside.Freespace.FreespaceManager FreespaceManager
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

		protected override bool Close2()
		{
			bool ret = base.Close2();
			i_dirty = null;
			return ret;
		}

		public override void Commit1()
		{
			CheckClosed();
			i_entryCounter++;
			try
			{
				Write(false);
			}
			catch (System.Exception t)
			{
				FatalException(t);
			}
			i_entryCounter--;
		}

		internal virtual void ConfigureNewFile()
		{
			NewSystemData(ConfigImpl().FreespaceSystem());
			SystemData().ConverterVersion(Db4objects.Db4o.Inside.Convert.Converter.VERSION);
			CreateStringIO(_systemData.StringEncoding());
			GenerateNewIdentity();
			_freespaceManager = Db4objects.Db4o.Inside.Freespace.FreespaceManager.CreateNew(this
				);
			BlockSize(ConfigImpl().BlockSize());
			_fileHeader = new Db4objects.Db4o.Header.FileHeader1();
			SetRegularEndAddress(_fileHeader.Length());
			InitNewClassCollection();
			InitializeEssentialClasses();
			_fileHeader.InitNew(this);
			_freespaceManager.Start(_systemData.FreespaceAddress());
			if (Db4objects.Db4o.Debug.freespace && Db4objects.Db4o.Debug.freespaceChecker)
			{
				_fmChecker.Start(0);
			}
		}

		private void NewSystemData(byte freespaceSystem)
		{
			_systemData = new Db4objects.Db4o.Inside.SystemData();
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

		public Db4objects.Db4o.Inside.Btree.BTree CreateBTreeClassIndex(int id)
		{
			return new Db4objects.Db4o.Inside.Btree.BTree(i_trans, id, new Db4objects.Db4o.YInt
				(this));
		}

		public Db4objects.Db4o.Inside.Query.AbstractQueryResult NewQueryResult(Db4objects.Db4o.Transaction
			 trans)
		{
			return NewQueryResult(trans, Config().LazyQueryEvaluation());
		}

		public sealed override Db4objects.Db4o.Inside.Query.AbstractQueryResult NewQueryResult
			(Db4objects.Db4o.Transaction trans, bool lazy)
		{
			if (lazy)
			{
				return new Db4objects.Db4o.Inside.Query.HybridQueryResult(trans);
			}
			return new Db4objects.Db4o.Inside.Query.IdListQueryResult(trans);
		}

		public sealed override bool Delete5(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.YapObject
			 yo, int a_cascade, bool userCall)
		{
			int id = yo.GetID();
			Db4objects.Db4o.YapWriter reader = ReadWriterByID(ta, id);
			if (reader != null)
			{
				object obj = yo.GetObject();
				if (obj != null)
				{
					if ((!ShowInternalClasses()) && Db4objects.Db4o.YapConst.CLASS_INTERNAL.IsAssignableFrom
						(obj.GetType()))
					{
						return false;
					}
				}
				reader.SetCascadeDeletes(a_cascade);
				reader.SlotDelete();
				Db4objects.Db4o.YapClass yc = yo.GetYapClass();
				yc.Delete(reader, obj);
				return true;
			}
			return false;
		}

		public abstract long FileLength();

		internal abstract string FileName();

		public virtual void Free(Db4objects.Db4o.Inside.Slots.Slot slot)
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
				i_prefetchedIDs.Traverse(new _AnonymousInnerClass208(this));
			}
			i_prefetchedIDs = null;
		}

		private sealed class _AnonymousInnerClass208 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass208(YapFile _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.Free(((Db4objects.Db4o.TreeInt)a_object)._key, Db4objects.Db4o.YapConst
					.POINTER_LENGTH);
			}

			private readonly YapFile _enclosing;
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
			SetIdentity(Db4objects.Db4o.Ext.Db4oDatabase.Generate());
		}

		public override Db4objects.Db4o.Inside.Query.AbstractQueryResult GetAll(Db4objects.Db4o.Transaction
			 trans)
		{
			return GetAll(trans, Config().LazyQueryEvaluation());
		}

		public virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult GetAll(Db4objects.Db4o.Transaction
			 trans, bool lazy)
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = NewQueryResult(trans
				, lazy);
			queryResult.LoadFromClassIndexes(ClassCollection().Iterator());
			return queryResult;
		}

		internal int GetPointerSlot()
		{
			int id = GetSlot(Db4objects.Db4o.YapConst.POINTER_LENGTH);
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
				DebugWriteXBytes(_blockEndAddress, blocksNeeded * BlockSize());
			}
			return AppendBlocks(blocksNeeded);
		}

		protected virtual int AppendBlocks(int blockCount)
		{
			int blockedAddress = _blockEndAddress;
			_blockEndAddress += blockCount;
			return blockedAddress;
		}

		internal virtual void EnsureLastSlotWritten()
		{
			if (_blockEndAddress > BlocksFor(FileLength()))
			{
				Db4objects.Db4o.YapWriter writer = GetWriter(i_systemTrans, _blockEndAddress - 1, 
					BlockSize());
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

		public Db4objects.Db4o.Inside.Slots.Pointer4 NewSlot(Db4objects.Db4o.Transaction 
			a_trans, int a_length)
		{
			int id = GetPointerSlot();
			int address = GetSlot(a_length);
			a_trans.SetPointer(id, address, a_length);
			return new Db4objects.Db4o.Inside.Slots.Pointer4(id, address);
		}

		public sealed override int NewUserObject()
		{
			return GetPointerSlot();
		}

		public virtual void PrefetchedIDConsumed(int a_id)
		{
			i_prefetchedIDs = i_prefetchedIDs.RemoveLike(new Db4objects.Db4o.TreeIntObject(a_id
				));
		}

		public virtual int PrefetchID()
		{
			int id = GetPointerSlot();
			i_prefetchedIDs = Db4objects.Db4o.Foundation.Tree.Add(i_prefetchedIDs, new Db4objects.Db4o.TreeInt
				(id));
			return id;
		}

		public virtual Db4objects.Db4o.Inside.Slots.ReferencedSlot ProduceFreeOnCommitEntry
			(int id)
		{
			Db4objects.Db4o.Foundation.Tree node = Db4objects.Db4o.TreeInt.Find(_freeOnCommit
				, id);
			if (node != null)
			{
				return (Db4objects.Db4o.Inside.Slots.ReferencedSlot)node;
			}
			Db4objects.Db4o.Inside.Slots.ReferencedSlot slot = new Db4objects.Db4o.Inside.Slots.ReferencedSlot
				(id);
			_freeOnCommit = Db4objects.Db4o.Foundation.Tree.Add(_freeOnCommit, slot);
			return slot;
		}

		public virtual void ReduceFreeOnCommitReferences(Db4objects.Db4o.Inside.Slots.ReferencedSlot
			 slot)
		{
			if (slot.RemoveReferenceIsLast())
			{
				_freeOnCommit = _freeOnCommit.RemoveNode(slot);
			}
		}

		public virtual void FreeDuringCommit(Db4objects.Db4o.Inside.Slots.ReferencedSlot 
			referencedSlot, Db4objects.Db4o.Inside.Slots.Slot slot)
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

		public override Db4objects.Db4o.YapWriter ReadWriterByID(Db4objects.Db4o.Transaction
			 a_ta, int a_id)
		{
			return (Db4objects.Db4o.YapWriter)ReadReaderOrWriterByID(a_ta, a_id, false);
		}

		public override Db4objects.Db4o.YapReader ReadReaderByID(Db4objects.Db4o.Transaction
			 a_ta, int a_id)
		{
			return ReadReaderOrWriterByID(a_ta, a_id, true);
		}

		private Db4objects.Db4o.YapReader ReadReaderOrWriterByID(Db4objects.Db4o.Transaction
			 a_ta, int a_id, bool useReader)
		{
			if (a_id == 0)
			{
				return null;
			}
			try
			{
				Db4objects.Db4o.Inside.Slots.Slot slot = a_ta.GetCurrentSlotOfID(a_id);
				if (slot == null)
				{
					return null;
				}
				if (slot._address == 0)
				{
					return null;
				}
				Db4objects.Db4o.YapReader reader = null;
				if (useReader)
				{
					reader = new Db4objects.Db4o.YapReader(slot._length);
				}
				else
				{
					reader = GetWriter(a_ta, slot._address, slot._length);
					((Db4objects.Db4o.YapWriter)reader).SetID(a_id);
				}
				reader.ReadEncrypt(this, slot._address);
				return reader;
			}
			catch (System.Exception e)
			{
			}
			return null;
		}

		internal virtual void ReadThis()
		{
			NewSystemData(Db4objects.Db4o.Inside.Freespace.FreespaceManager.FM_LEGACY_RAM);
			BlockSizeReadFromFile(1);
			_fileHeader = Db4objects.Db4o.Header.FileHeader.ReadFixedPart(this);
			CreateStringIO(_systemData.StringEncoding());
			ClassCollection().SetID(_systemData.ClassCollectionID());
			ClassCollection().Read(i_systemTrans);
			Db4objects.Db4o.Inside.Convert.Converter.Convert(new Db4objects.Db4o.Inside.Convert.ConversionStage.ClassCollectionAvailableStage
				(this));
			_freespaceManager = Db4objects.Db4o.Inside.Freespace.FreespaceManager.CreateNew(this
				, _systemData.FreespaceSystem());
			_freespaceManager.Read(_systemData.FreespaceID());
			_freespaceManager.Start(_systemData.FreespaceAddress());
			ReadHeaderVariablePart();
			if (_freespaceManager.RequiresMigration(ConfigImpl().FreespaceSystem(), _systemData
				.FreespaceSystem()))
			{
				_freespaceManager = _freespaceManager.Migrate(this, ConfigImpl().FreespaceSystem(
					));
				_fileHeader.WriteVariablePart(this, 1);
			}
			WriteHeader(false);
			Db4objects.Db4o.Transaction trans = _fileHeader.InterruptedTransaction();
			if (trans != null)
			{
				if (!ConfigImpl().CommitRecoveryDisabled())
				{
					trans.WriteOld();
				}
			}
			if (Db4objects.Db4o.Inside.Convert.Converter.Convert(new Db4objects.Db4o.Inside.Convert.ConversionStage.SystemUpStage
				(this)))
			{
				_systemData.ConverterVersion(Db4objects.Db4o.Inside.Convert.Converter.VERSION);
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
			_systemData.FreespaceAddress(Db4objects.Db4o.Inside.Freespace.FreespaceManager.InitSlot
				(this));
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

		public virtual void ReleaseSemaphore(Db4objects.Db4o.Transaction ta, string name)
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

		public override void ReleaseSemaphores(Db4objects.Db4o.Transaction ta)
		{
			if (i_semaphores != null)
			{
				Db4objects.Db4o.Foundation.Hashtable4 semaphores = i_semaphores;
				lock (semaphores)
				{
					semaphores.ForEachKeyForIdentity(new _AnonymousInnerClass558(this, semaphores), ta
						);
					Sharpen.Runtime.NotifyAll(semaphores);
				}
			}
		}

		private sealed class _AnonymousInnerClass558 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass558(YapFile _enclosing, Db4objects.Db4o.Foundation.Hashtable4
				 semaphores)
			{
				this._enclosing = _enclosing;
				this.semaphores = semaphores;
			}

			public void Visit(object a_object)
			{
				semaphores.Remove(a_object);
			}

			private readonly YapFile _enclosing;

			private readonly Db4objects.Db4o.Foundation.Hashtable4 semaphores;
		}

		public sealed override void Rollback1()
		{
			CheckClosed();
			i_entryCounter++;
			GetTransaction().Rollback();
			i_entryCounter--;
		}

		public sealed override void SetDirtyInSystemTransaction(Db4objects.Db4o.YapMeta a_object
			)
		{
			a_object.SetStateDirty();
			a_object.CacheDirty(i_dirty);
		}

		public override bool SetSemaphore(string name, int timeout)
		{
			return SetSemaphore(CheckTransaction(null), name, timeout);
		}

		public virtual bool SetSemaphore(Db4objects.Db4o.Transaction ta, string name, int
			 timeout)
		{
			if (name == null)
			{
				throw new System.ArgumentNullException();
			}
			if (i_semaphores == null)
			{
				lock (i_lock)
				{
					if (i_semaphores == null)
					{
						i_semaphores = new Db4objects.Db4o.Foundation.Hashtable4(10);
					}
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
			i_trans.Commit();
			if (shuttingDown)
			{
				WriteHeader(shuttingDown);
			}
		}

		public abstract bool WriteAccessTime(int address, int offset, long time);

		public abstract void WriteBytes(Db4objects.Db4o.YapReader a_Bytes, int address, int
			 addressOffset);

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
				Db4objects.Db4o.YapMeta dirty = (Db4objects.Db4o.YapMeta)i.Current;
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

		public sealed override void WriteEmbedded(Db4objects.Db4o.YapWriter a_parent, Db4objects.Db4o.YapWriter
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

		internal virtual void WriteHeader(bool shuttingDown)
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
			Db4objects.Db4o.YapWriter writer = GetWriter(i_systemTrans, 0, _fileHeader.Length
				());
			_fileHeader.WriteFixedPart(this, shuttingDown, writer, BlockSize(), freespaceID);
			if (shuttingDown)
			{
				EnsureLastSlotWritten();
			}
			SyncFiles();
		}

		public sealed override void WriteNew(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.YapWriter
			 aWriter)
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

		public abstract void DebugWriteXBytes(int a_address, int a_length);

		internal virtual Db4objects.Db4o.YapReader XBytes(int a_address, int a_length)
		{
			Db4objects.Db4o.YapReader bytes = GetWriter(i_systemTrans, a_address, a_length);
			for (int i = 0; i < a_length; i++)
			{
				bytes.Append(Db4objects.Db4o.YapConst.XBYTE);
			}
			return bytes;
		}

		public sealed override void WriteTransactionPointer(int address)
		{
			_fileHeader.WriteTransactionPointer(GetSystemTransaction(), address);
		}

		public void GetSlotForUpdate(Db4objects.Db4o.YapWriter forWriter)
		{
			Db4objects.Db4o.Transaction trans = forWriter.GetTransaction();
			int id = forWriter.GetID();
			int length = forWriter.GetLength();
			int address = GetSlot(length);
			forWriter.Address(address);
			trans.SlotFreeOnRollbackSetPointer(id, address, length);
		}

		public sealed override void WriteUpdate(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.YapWriter
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

		public override Db4objects.Db4o.Ext.ISystemInfo SystemInfo()
		{
			return new Db4objects.Db4o.Inside.SystemInfoFileImpl(this);
		}

		public virtual Db4objects.Db4o.Header.FileHeader GetFileHeader()
		{
			return _fileHeader;
		}

		public virtual void InstallDebugFreespaceManager(Db4objects.Db4o.Inside.Freespace.FreespaceManager
			 manager)
		{
			_freespaceManager = manager;
		}

		public virtual Db4objects.Db4o.Inside.SystemData SystemData()
		{
			return _systemData;
		}

		public override long[] GetIDsForClass(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass
			 clazz)
		{
			Db4objects.Db4o.Foundation.IntArrayList ids = new Db4objects.Db4o.Foundation.IntArrayList
				();
			clazz.Index().TraverseAll(trans, new _AnonymousInnerClass785(this, ids));
			return ids.AsLong();
		}

		private sealed class _AnonymousInnerClass785 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass785(YapFile _enclosing, Db4objects.Db4o.Foundation.IntArrayList
				 ids)
			{
				this._enclosing = _enclosing;
				this.ids = ids;
			}

			public void Visit(object obj)
			{
				ids.Add(((int)obj));
			}

			private readonly YapFile _enclosing;

			private readonly Db4objects.Db4o.Foundation.IntArrayList ids;
		}

		public override Db4objects.Db4o.Inside.Query.IQueryResult ClassOnlyQuery(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapClass clazz)
		{
			if (!clazz.HasIndex())
			{
				return null;
			}
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = NewQueryResult(trans
				);
			queryResult.LoadFromClassIndex(clazz);
			return queryResult;
		}

		public override Db4objects.Db4o.Inside.Query.IQueryResult ExecuteQuery(Db4objects.Db4o.QQuery
			 query)
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = NewQueryResult(query
				.GetTransaction());
			queryResult.LoadFromQuery(query);
			return queryResult;
		}
	}
}
