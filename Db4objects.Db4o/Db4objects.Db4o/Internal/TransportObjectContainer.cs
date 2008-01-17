/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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

		internal override void InitializeEssentialClasses()
		{
		}

		protected override void InitializePostOpenExcludingTransportObjectContainer()
		{
		}

		internal override void InitNewClassCollection()
		{
		}

		internal override bool CanUpdate()
		{
			// do nothing
			// do nothing
			// do nothing
			// do nothing
			return false;
		}

		public override ClassMetadata ClassMetadataForId(int id)
		{
			return _parent.ClassMetadataForId(id);
		}

		internal override void ConfigureNewFile()
		{
		}

		public override int ConverterVersion()
		{
			// do nothing
			return Converter.Version;
		}

		protected override void DropReferences()
		{
			_config = null;
		}

		protected override void HandleExceptionOnClose(Exception exc)
		{
		}

		public sealed override Transaction NewTransaction(Transaction parentTransaction, 
			TransactionalReferenceSystem referenceSystem)
		{
			// do nothing here
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

		public sealed override void Free(int a_address, int a_length)
		{
		}

		public sealed override void Free(Slot slot)
		{
		}

		public override Slot GetSlot(int length)
		{
			// do nothing
			// do nothing
			// do nothing
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

		public override ClassMetadata ProduceClassMetadata(IReflectClass claxx)
		{
			// do nothing
			return _parent.ProduceClassMetadata(claxx);
		}

		public override void RaiseVersion(long a_minimumVersion)
		{
		}

		internal override void ReadThis()
		{
		}

		internal override bool StateMessages()
		{
			// do nothing
			// do nothing
			return false;
		}

		public override void Shutdown()
		{
			// overridden to do nothing in YapObjectCarrier
			ProcessPendingClassUpdates();
			WriteDirty();
			Transaction().Commit();
		}

		internal sealed override void WriteHeader(bool startFileLockingThread, bool shuttingDown
			)
		{
		}

		protected override void WriteVariableHeader()
		{
		}

		public class KnownObjectIdentity
		{
			public int _id;

			public KnownObjectIdentity(int id)
			{
				// do nothing
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
			else
			{
				return base.StoreInternal(trans, obj, depth, checkJustSet);
			}
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
