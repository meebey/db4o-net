/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.Slots;
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
	public class TransportObjectContainer : InMemoryObjectContainer
	{
		public TransportObjectContainer(ObjectContainerBase serviceProvider, MemoryFile memoryFile
			) : base(serviceProvider.Config(), serviceProvider, memoryFile)
		{
			_showInternalClasses = serviceProvider._showInternalClasses;
		}

		protected override void Initialize1(IConfiguration config)
		{
			_handlers = _parent._handlers;
			_classCollection = _parent.ClassCollection();
			_config = _parent.ConfigImpl();
			_references = new WeakReferenceCollector(this);
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

		public override ClassMetadata ClassMetadataForId(int id)
		{
			return _parent.ClassMetadataForId(id);
		}

		internal override void ConfigureNewFile()
		{
		}

		// do nothing
		public override int ConverterVersion()
		{
			return Converter.Version;
		}

		protected override void DropReferences()
		{
			_config = null;
		}

		protected override void HandleExceptionOnClose(Exception exc)
		{
		}

		// do nothing here
		public sealed override Transaction NewTransaction(Transaction parentTransaction, 
			TransactionalReferenceSystem referenceSystem)
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

		public override Db4oDatabase Identity()
		{
			return ((ExternalObjectContainer)_parent).Identity();
		}

		public override bool MaintainsIndices()
		{
			return false;
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
			Transaction().Commit();
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

		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
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
	}
}
