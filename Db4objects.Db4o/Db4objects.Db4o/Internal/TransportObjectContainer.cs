/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.References;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Weakref;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// no reading
	/// no writing
	/// no updates
	/// no weak references
	/// navigation by ID only both sides need synchronised ClassCollections and
	/// MetaInformationCaches
	/// </summary>
	/// <exclude></exclude>
	public class TransportObjectContainer : LocalObjectContainer
	{
		private readonly ObjectContainerBase _parent;

		private readonly MemoryBin _memoryBin;

		public TransportObjectContainer(ObjectContainerBase parent, MemoryBin memoryFile)
			 : base(parent.Config())
		{
			_memoryBin = memoryFile;
			_parent = parent;
			_lock = parent.Lock();
			_showInternalClasses = parent._showInternalClasses;
			Open();
		}

		protected override void Initialize1(IConfiguration config)
		{
			_handlers = _parent._handlers;
			_classCollection = _parent.ClassCollection();
			_config = _parent.ConfigImpl;
			_references = WeakReferenceSupportFactory.DisabledWeakReferenceSupport();
			Initialize2();
		}

		internal override void Initialize2NObjectCarrier()
		{
		}

		// do nothing
		internal override void InitializeEssentialClasses()
		{
		}

		// do nothing
		protected override void InitializePostOpenExcludingTransportObjectContainer()
		{
		}

		// do nothing
		internal override void InitNewClassCollection()
		{
		}

		// do nothing
		internal override bool CanUpdate()
		{
			return false;
		}

		public override ClassMetadata ClassMetadataForID(int id)
		{
			return _parent.ClassMetadataForID(id);
		}

		internal override void ConfigureNewFile()
		{
		}

		// do nothing
		public override int ConverterVersion()
		{
			return Converter.Version;
		}

		protected virtual void DropReferences()
		{
			_config = null;
		}

		protected override void HandleExceptionOnClose(Exception exc)
		{
		}

		// do nothing here
		public sealed override Transaction NewTransaction(Transaction parentTransaction, 
			IReferenceSystem referenceSystem)
		{
			if (null != parentTransaction)
			{
				return parentTransaction;
			}
			return new TransactionObjectCarrier(this, null, referenceSystem);
		}

		public override long CurrentVersion()
		{
			return 0;
		}

		public override IDb4oType Db4oTypeStored(Transaction a_trans, object a_object)
		{
			return null;
		}

		public override bool DispatchsEvents()
		{
			return false;
		}

		~TransportObjectContainer()
		{
		}

		// do nothing
		public sealed override void Free(int a_address, int a_length)
		{
		}

		// do nothing
		public sealed override void Free(Slot slot)
		{
		}

		// do nothing
		public override Slot GetSlot(int length)
		{
			return AppendBlocks(length);
		}

		protected override bool IsValidPointer(int id)
		{
			return id != 0 && base.IsValidPointer(id);
		}

		public override Db4oDatabase Identity()
		{
			return ((ExternalObjectContainer)_parent).Identity();
		}

		public override bool MaintainsIndices()
		{
			return false;
		}

		public override long GenerateTimeStampId()
		{
			return _parent.GenerateTimeStampId();
		}

		internal override void Message(string msg)
		{
		}

		// do nothing
		public override ClassMetadata ProduceClassMetadata(IReflectClass claxx)
		{
			return _parent.ProduceClassMetadata(claxx);
		}

		public override void RaiseVersion(long a_minimumVersion)
		{
		}

		// do nothing
		internal override void ReadThis()
		{
		}

		// do nothing
		internal override bool StateMessages()
		{
			return false;
		}

		// overridden to do nothing in YapObjectCarrier
		public override void Shutdown()
		{
			ProcessPendingClassUpdates();
			WriteDirty();
			Transaction.Commit();
		}

		internal sealed override void WriteHeader(bool startFileLockingThread, bool shuttingDown
			)
		{
		}

		// do nothing
		protected override void WriteVariableHeader()
		{
		}

		public class KnownObjectIdentity
		{
			public int _id;

			public KnownObjectIdentity(int id)
			{
				_id = id;
			}
		}

		/// <exception cref="Db4objects.Db4o.Ext.DatabaseClosedException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseReadOnlyException"></exception>
		public override int StoreInternal(Transaction trans, object obj, int depth, bool 
			checkJustSet)
		{
			int id = _parent.GetID(null, obj);
			if (id > 0)
			{
				return base.StoreInternal(trans, new TransportObjectContainer.KnownObjectIdentity
					(id), depth, checkJustSet);
			}
			return base.StoreInternal(trans, obj, depth, checkJustSet);
		}

		public override object GetByID2(Transaction ta, int id)
		{
			object obj = base.GetByID2(ta, id);
			if (obj is TransportObjectContainer.KnownObjectIdentity)
			{
				TransportObjectContainer.KnownObjectIdentity oi = (TransportObjectContainer.KnownObjectIdentity
					)obj;
				Activate(oi);
				obj = _parent.GetByID(null, oi._id);
			}
			return obj;
		}

		public virtual void DeferredOpen()
		{
			Open();
		}

		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		protected sealed override void OpenImpl()
		{
			if (_memoryBin.Length() == 0)
			{
				ConfigureNewFile();
				CommitTransaction();
				WriteHeader(false, false);
			}
			else
			{
				ReadThis();
			}
		}

		/// <exception cref="System.NotSupportedException"></exception>
		public override void Backup(IStorage targetStorage, string path)
		{
			throw new NotSupportedException();
		}

		public override void BlockSize(int size)
		{
		}

		// do nothing, blocksize is always 1
		protected override void CloseTransaction()
		{
		}

		// do nothing
		protected override void CloseSystemTransaction()
		{
		}

		// do nothing
		protected override void FreeInternalResources()
		{
		}

		// nothing to do here
		protected override void ShutdownDataStorage()
		{
			DropReferences();
		}

		public override long FileLength()
		{
			return _memoryBin.Length();
		}

		public override string FileName()
		{
			return "Memory File";
		}

		protected override bool HasShutDownHook()
		{
			return false;
		}

		public sealed override bool NeedsLockFileThread()
		{
			return false;
		}

		public override void ReadBytes(byte[] bytes, int address, int length)
		{
			try
			{
				_memoryBin.Read(address, bytes, length);
			}
			catch (Exception e)
			{
				Exceptions4.ThrowRuntimeException(13, e);
			}
		}

		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			ReadBytes(bytes, address + addressOffset, length);
		}

		public override void SyncFiles()
		{
		}

		public override void WriteBytes(ByteArrayBuffer buffer, int address, int addressOffset
			)
		{
			_memoryBin.Write(address + addressOffset, buffer._buffer, buffer.Length());
		}

		public override void OverwriteDeletedBytes(int a_address, int a_length)
		{
		}

		public override void Reserve(int byteCount)
		{
			throw new NotSupportedException();
		}

		public override byte BlockSize()
		{
			return 1;
		}

		protected override void FatalStorageShutdown()
		{
			ShutdownDataStorage();
		}

		public override IReferenceSystem CreateReferenceSystem()
		{
			return new HashcodeReferenceSystem();
		}
	}
}
