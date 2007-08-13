/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Replication;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// TODO: refactor for symmetric inheritance - don't inherit from YapField and override,
	/// instead extract an abstract superclass from YapField and let both YapField and this class implement
	/// </summary>
	/// <exclude></exclude>
	public abstract class VirtualFieldMetadata : FieldMetadata
	{
		internal VirtualFieldMetadata() : base(null)
		{
		}

		public abstract override void AddFieldIndex(MarshallerFamily mf, ClassMetadata yapClass
			, StatefulBuffer a_writer, Slot oldSlot);

		public override bool Alive()
		{
			return true;
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, object obj)
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

		internal override void CollectConstraints(Transaction a_trans, QConObject a_parent
			, object a_template, IVisitor4 a_visitor)
		{
		}

		internal override void Deactivate(Transaction a_trans, object a_onObject, int a_depth
			)
		{
		}

		public abstract override void Delete(MarshallerFamily mf, StatefulBuffer a_bytes, 
			bool isUpdate);

		public override object GetOrCreate(Transaction a_trans, object a_OnObject)
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

		public override void Instantiate(MarshallerFamily mf, ObjectReference a_yapObject
			, object a_onObject, StatefulBuffer a_bytes)
		{
			a_yapObject.ProduceVirtualAttributes();
			Instantiate1(a_bytes.GetTransaction(), a_yapObject, a_bytes);
		}

		internal abstract void Instantiate1(Transaction a_trans, ObjectReference a_yapObject
			, Db4objects.Db4o.Internal.Buffer a_bytes);

		public override void LoadHandler(ObjectContainerBase a_stream)
		{
		}

		public sealed override void Marshall(ObjectReference a_yapObject, object a_object
			, MarshallerFamily mf, StatefulBuffer a_bytes, Config4Class a_config, bool a_new
			)
		{
			Transaction trans = a_bytes.GetTransaction();
			if (!trans.SupportsVirtualFields())
			{
				MarshallIgnore(a_bytes);
				return;
			}
			ObjectContainerBase stream = trans.Container();
			HandlerRegistry handlers = stream._handlers;
			bool migrating = false;
			if (stream._replicationCallState != Const4.NONE)
			{
				if (stream._replicationCallState == Const4.OLD)
				{
					migrating = true;
					if (a_yapObject.VirtualAttributes() == null)
					{
						object obj = a_yapObject.GetObject();
						ObjectReference migrateYapObject = null;
						MigrationConnection mgc = handlers.i_migration;
						if (mgc != null)
						{
							migrateYapObject = mgc.ReferenceFor(obj);
							if (migrateYapObject == null)
							{
								ObjectContainerBase peer = mgc.Peer(stream);
								migrateYapObject = peer.Transaction().ReferenceForObject(obj);
							}
						}
						if (migrateYapObject != null)
						{
							VirtualAttributes migrateAttributes = migrateYapObject.VirtualAttributes();
							if (migrateAttributes != null && migrateAttributes.i_database != null)
							{
								migrating = true;
								a_yapObject.SetVirtualAttributes((VirtualAttributes)migrateAttributes.ShallowClone
									());
								migrateAttributes.i_database.Bind(trans);
							}
						}
					}
				}
				else
				{
					IDb4oReplicationReferenceProvider provider = handlers._replicationReferenceProvider;
					object parentObject = a_yapObject.GetObject();
					IDb4oReplicationReference @ref = provider.ReferenceFor(parentObject);
					if (@ref != null)
					{
						migrating = true;
						VirtualAttributes va = a_yapObject.ProduceVirtualAttributes();
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

		internal abstract void Marshall1(ObjectReference a_yapObject, StatefulBuffer a_bytes
			, bool a_migrating, bool a_new);

		internal abstract void MarshallIgnore(Db4objects.Db4o.Internal.Buffer writer);

		public override void ReadVirtualAttribute(Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_reader, ObjectReference a_yapObject)
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

		protected override IIndexable4 IndexHandler(ObjectContainerBase stream)
		{
			return (IIndexable4)i_handler;
		}
	}
}
