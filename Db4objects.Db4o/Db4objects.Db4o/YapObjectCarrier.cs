namespace Db4objects.Db4o
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
	public class YapObjectCarrier : Db4objects.Db4o.YapMemoryFile
	{
		internal YapObjectCarrier(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.YapStream
			 a_callingStream, Db4objects.Db4o.Ext.MemoryFile memoryFile) : base(config, a_callingStream
			, memoryFile)
		{
		}

		protected override void Initialize1(Db4objects.Db4o.Config.IConfiguration config)
		{
			i_handlers = i_parent.i_handlers;
			_classCollection = i_parent.ClassCollection();
			i_config = i_parent.ConfigImpl();
			i_references = new Db4objects.Db4o.YapReferences(this);
			Initialize2();
		}

		internal override void Initialize2NObjectCarrier()
		{
		}

		internal override void InitializeEssentialClasses()
		{
		}

		internal override void Initialize4NObjectCarrier()
		{
		}

		internal override void InitNewClassCollection()
		{
		}

		internal override bool CanUpdate()
		{
			return false;
		}

		internal override void ConfigureNewFile()
		{
		}

		public override int ConverterVersion()
		{
			return Db4objects.Db4o.Inside.Convert.Converter.VERSION;
		}

		public override bool Close()
		{
			lock (i_lock)
			{
				bool ret = Close1();
				if (ret)
				{
					i_config = null;
				}
				return ret;
			}
		}

		public sealed override Db4objects.Db4o.Transaction NewTransaction(Db4objects.Db4o.Transaction
			 parentTransaction)
		{
			if (null != parentTransaction)
			{
				return parentTransaction;
			}
			return new Db4objects.Db4o.TransactionObjectCarrier(this, null);
		}

		public override long CurrentVersion()
		{
			return 0;
		}

		public override Db4objects.Db4o.Types.IDb4oType Db4oTypeStored(Db4objects.Db4o.Transaction
			 a_trans, object a_object)
		{
			return null;
		}

		public override bool DispatchsEvents()
		{
			return false;
		}

		~YapObjectCarrier()
		{
		}

		public sealed override void Free(int a_address, int a_length)
		{
		}

		public override int GetSlot(int length)
		{
			return AppendBlocks(length);
		}

		public override Db4objects.Db4o.Ext.Db4oDatabase Identity()
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

		public override void Write(bool shuttingDown)
		{
			CheckNeededUpdates();
			WriteDirty();
			GetTransaction().Commit();
		}

		internal sealed override void WriteHeader(bool shuttingDown)
		{
		}

		protected override void WriteVariableHeader()
		{
		}
	}
}
