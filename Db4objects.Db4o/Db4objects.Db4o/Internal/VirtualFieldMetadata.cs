namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// TODO: refactor for symmetric inheritance - don't inherit from YapField and override,
	/// instead extract an abstract superclass from YapField and let both YapField and this class implement
	/// </summary>
	/// <exclude></exclude>
	public abstract class VirtualFieldMetadata : Db4objects.Db4o.Internal.FieldMetadata
	{
		internal VirtualFieldMetadata() : base(null)
		{
		}

		public abstract override void AddFieldIndex(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.StatefulBuffer
			 a_writer, Db4objects.Db4o.Internal.Slots.Slot oldSlot);

		public override bool Alive()
		{
			return true;
		}

		public override void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, object obj)
		{
			header.AddBaseLength(LinkLength());
		}

		internal override bool CanAddToQuery(string fieldName)
		{
			return fieldName.Equals(GetName());
		}

		public override bool CanUseNullBitmap()
		{
			return false;
		}

		internal override void CollectConstraints(Db4objects.Db4o.Internal.Transaction a_trans
			, Db4objects.Db4o.Internal.Query.Processor.QConObject a_parent, object a_template
			, Db4objects.Db4o.Foundation.IVisitor4 a_visitor)
		{
		}

		internal override void Deactivate(Db4objects.Db4o.Internal.Transaction a_trans, object
			 a_onObject, int a_depth)
		{
		}

		public abstract override void Delete(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool isUpdate);

		public override object GetOrCreate(Db4objects.Db4o.Internal.Transaction a_trans, 
			object a_OnObject)
		{
			return null;
		}

		public override bool NeedsArrayAndPrimitiveInfo()
		{
			return false;
		}

		public override bool NeedsHandlerId()
		{
			return false;
		}

		public override void Instantiate(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ObjectReference a_yapObject, object a_onObject, Db4objects.Db4o.Internal.StatefulBuffer
			 a_bytes)
		{
			a_yapObject.ProduceVirtualAttributes();
			Instantiate1(a_bytes.GetTransaction(), a_yapObject, a_bytes);
		}

		internal abstract void Instantiate1(Db4objects.Db4o.Internal.Transaction a_trans, 
			Db4objects.Db4o.Internal.ObjectReference a_yapObject, Db4objects.Db4o.Internal.Buffer
			 a_bytes);

		public override void LoadHandler(Db4objects.Db4o.Internal.ObjectContainerBase a_stream
			)
		{
		}

		public sealed override void Marshall(Db4objects.Db4o.Internal.ObjectReference a_yapObject
			, object a_object, Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.StatefulBuffer
			 a_bytes, Db4objects.Db4o.Internal.Config4Class a_config, bool a_new)
		{
			Db4objects.Db4o.Internal.Transaction trans = a_bytes.GetTransaction();
			if (!trans.SupportsVirtualFields())
			{
				MarshallIgnore(a_bytes);
				return;
			}
			Db4objects.Db4o.Internal.ObjectContainerBase stream = trans.Stream();
			Db4objects.Db4o.Internal.HandlerRegistry handlers = stream.i_handlers;
			bool migrating = false;
			if (stream._replicationCallState != Db4objects.Db4o.Internal.Const4.NONE)
			{
				if (stream._replicationCallState == Db4objects.Db4o.Internal.Const4.OLD)
				{
					migrating = true;
					if (a_yapObject.VirtualAttributes() == null)
					{
						object obj = a_yapObject.GetObject();
						Db4objects.Db4o.Internal.ObjectReference migrateYapObject = null;
						Db4objects.Db4o.Internal.Replication.MigrationConnection mgc = handlers.i_migration;
						if (mgc != null)
						{
							migrateYapObject = mgc.ReferenceFor(obj);
							if (migrateYapObject == null)
							{
								migrateYapObject = mgc.Peer(stream).GetYapObject(obj);
							}
						}
						if (migrateYapObject != null)
						{
							Db4objects.Db4o.Internal.VirtualAttributes migrateAttributes = migrateYapObject.VirtualAttributes
								();
							if (migrateAttributes != null && migrateAttributes.i_database != null)
							{
								migrating = true;
								a_yapObject.SetVirtualAttributes((Db4objects.Db4o.Internal.VirtualAttributes)migrateAttributes
									.ShallowClone());
								migrateAttributes.i_database.Bind(trans);
							}
						}
					}
				}
				else
				{
					Db4objects.Db4o.Internal.Replication.IDb4oReplicationReferenceProvider provider = 
						handlers._replicationReferenceProvider;
					object parentObject = a_yapObject.GetObject();
					Db4objects.Db4o.Internal.Replication.IDb4oReplicationReference @ref = provider.ReferenceFor
						(parentObject);
					if (@ref != null)
					{
						migrating = true;
						Db4objects.Db4o.Internal.VirtualAttributes va = a_yapObject.ProduceVirtualAttributes
							();
						va.i_version = @ref.Version();
						va.i_uuid = @ref.LongPart();
						va.i_database = @ref.SignaturePart();
					}
				}
			}
			if (a_yapObject.VirtualAttributes() == null)
			{
				a_yapObject.ProduceVirtualAttributes();
				migrating = false;
			}
			Marshall1(a_yapObject, a_bytes, migrating, a_new);
		}

		internal abstract void Marshall1(Db4objects.Db4o.Internal.ObjectReference a_yapObject
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool a_migrating, bool a_new);

		internal abstract void MarshallIgnore(Db4objects.Db4o.Internal.Buffer writer);

		public override void ReadVirtualAttribute(Db4objects.Db4o.Internal.Transaction a_trans
			, Db4objects.Db4o.Internal.Buffer a_reader, Db4objects.Db4o.Internal.ObjectReference
			 a_yapObject)
		{
			if (!a_trans.SupportsVirtualFields())
			{
				a_reader.IncrementOffset(LinkLength());
				return;
			}
			Instantiate1(a_trans, a_yapObject, a_reader);
		}

		public override bool IsVirtual()
		{
			return true;
		}

		protected override object IndexEntryFor(object indexEntry)
		{
			return indexEntry;
		}

		protected override Db4objects.Db4o.Internal.IX.IIndexable4 IndexHandler(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream)
		{
			return i_handler;
		}
	}
}
