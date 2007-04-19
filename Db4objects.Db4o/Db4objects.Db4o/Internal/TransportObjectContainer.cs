using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;
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
			i_showInternalClasses = serviceProvider.i_showInternalClasses;
		}

		protected override void Initialize1(IConfiguration config)
		{
			i_handlers = i_parent.i_handlers;
			_classCollection = i_parent.ClassCollection();
			i_config = i_parent.ConfigImpl();
			i_references = new WeakReferenceCollector(this);
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
			return false;
		}

		public override ClassMetadata ClassMetadataForId(int id)
		{
			return i_parent.ClassMetadataForId(id);
		}

		internal override void ConfigureNewFile()
		{
		}

		public override int ConverterVersion()
		{
			return Converter.VERSION;
		}

		protected override void DropReferences()
		{
			i_config = null;
		}

		protected override void HandleExceptionOnClose(Exception exc)
		{
		}

		public sealed override Transaction NewTransaction(Transaction parentTransaction)
		{
			if (null != parentTransaction)
			{
				return parentTransaction;
			}
			return new TransactionObjectCarrier(this, null);
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

		public override int GetSlot(int length)
		{
			return AppendBlocks(length);
		}

		public override Db4oDatabase Identity()
		{
			return i_parent.Identity();
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
			return i_parent.ProduceClassMetadata(claxx);
		}

		public override void RaiseVersion(long a_minimumVersion)
		{
		}

		internal override void ReadThis()
		{
		}

		internal override bool StateMessages()
		{
			return false;
		}

		public override void Shutdown()
		{
			ProcessPendingClassUpdates();
			WriteDirty();
			GetTransaction().Commit();
		}

		internal sealed override void WriteHeader(bool startFileLockingThread, bool shuttingDown
			)
		{
		}

		protected override void WriteVariableHeader()
		{
		}
	}
}
