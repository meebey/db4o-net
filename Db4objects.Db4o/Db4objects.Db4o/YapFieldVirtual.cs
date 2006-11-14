namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class YapFieldVirtual : Db4objects.Db4o.YapField
	{
		internal YapFieldVirtual() : base(null)
		{
		}

		public abstract override void AddFieldIndex(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapWriter a_writer, Db4objects.Db4o.Inside.Slots.Slot
			 oldSlot);

		public override bool Alive()
		{
			return true;
		}

		public override void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, object obj)
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

		internal override void CollectConstraints(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.QConObject
			 a_parent, object a_template, Db4objects.Db4o.Foundation.IVisitor4 a_visitor)
		{
		}

		internal override void Deactivate(Db4objects.Db4o.Transaction a_trans, object a_onObject
			, int a_depth)
		{
		}

		public abstract override void Delete(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_bytes, bool isUpdate);

		public override object GetOrCreate(Db4objects.Db4o.Transaction a_trans, object a_OnObject
			)
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

		public override void Instantiate(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapObject a_yapObject, object a_onObject, Db4objects.Db4o.YapWriter
			 a_bytes)
		{
			if (a_yapObject.i_virtualAttributes == null)
			{
				a_yapObject.i_virtualAttributes = new Db4objects.Db4o.VirtualAttributes();
			}
			Instantiate1(a_bytes.GetTransaction(), a_yapObject, a_bytes);
		}

		internal abstract void Instantiate1(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapObject
			 a_yapObject, Db4objects.Db4o.YapReader a_bytes);

		public override void LoadHandler(Db4objects.Db4o.YapStream a_stream)
		{
		}

		public sealed override void Marshall(Db4objects.Db4o.YapObject a_yapObject, object
			 a_object, Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapWriter
			 a_bytes, Db4objects.Db4o.Config4Class a_config, bool a_new)
		{
			Db4objects.Db4o.Transaction trans = a_bytes.i_trans;
			if (!trans.SupportsVirtualFields())
			{
				MarshallIgnore(a_bytes);
				return;
			}
			Db4objects.Db4o.YapStream stream = trans.Stream();
			Db4objects.Db4o.YapHandlers handlers = stream.i_handlers;
			bool migrating = false;
			if (stream._replicationCallState != Db4objects.Db4o.YapConst.NONE)
			{
				if (stream._replicationCallState == Db4objects.Db4o.YapConst.OLD)
				{
					migrating = true;
					if (a_yapObject.i_virtualAttributes == null)
					{
						object obj = a_yapObject.GetObject();
						Db4objects.Db4o.YapObject migrateYapObject = null;
						Db4objects.Db4o.Inside.Replication.MigrationConnection mgc = handlers.i_migration;
						if (mgc != null)
						{
							migrateYapObject = mgc.ReferenceFor(obj);
							if (migrateYapObject == null)
							{
								migrateYapObject = mgc.Peer(stream).GetYapObject(obj);
							}
						}
						if (migrateYapObject != null && migrateYapObject.i_virtualAttributes != null && migrateYapObject
							.i_virtualAttributes.i_database != null)
						{
							migrating = true;
							a_yapObject.i_virtualAttributes = (Db4objects.Db4o.VirtualAttributes)migrateYapObject
								.i_virtualAttributes.ShallowClone();
							if (migrateYapObject.i_virtualAttributes.i_database != null)
							{
								migrateYapObject.i_virtualAttributes.i_database.Bind(trans);
							}
						}
					}
				}
				else
				{
					Db4objects.Db4o.Inside.Replication.IDb4oReplicationReferenceProvider provider = handlers
						._replicationReferenceProvider;
					object parentObject = a_yapObject.GetObject();
					Db4objects.Db4o.Inside.Replication.IDb4oReplicationReference @ref = provider.ReferenceFor
						(parentObject);
					if (@ref != null)
					{
						migrating = true;
						if (a_yapObject.i_virtualAttributes == null)
						{
							a_yapObject.i_virtualAttributes = new Db4objects.Db4o.VirtualAttributes();
						}
						Db4objects.Db4o.VirtualAttributes va = a_yapObject.i_virtualAttributes;
						va.i_version = @ref.Version();
						va.i_uuid = @ref.LongPart();
						va.i_database = @ref.SignaturePart();
					}
				}
			}
			if (a_yapObject.i_virtualAttributes == null)
			{
				a_yapObject.i_virtualAttributes = new Db4objects.Db4o.VirtualAttributes();
				migrating = false;
			}
			Marshall1(a_yapObject, a_bytes, migrating, a_new);
		}

		internal abstract void Marshall1(Db4objects.Db4o.YapObject a_yapObject, Db4objects.Db4o.YapWriter
			 a_bytes, bool a_migrating, bool a_new);

		internal abstract void MarshallIgnore(Db4objects.Db4o.YapReader writer);

		public override void ReadVirtualAttribute(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapReader
			 a_reader, Db4objects.Db4o.YapObject a_yapObject)
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
	}
}
