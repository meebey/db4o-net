/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Replication;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// TODO: refactor for symmetric inheritance - don't inherit from YapField and override,
	/// instead extract an abstract superclass from YapField and let both YapField and this class implement
	/// </summary>
	/// <exclude></exclude>
	public abstract class VirtualFieldMetadata : FieldMetadata
	{
		private static readonly object ANY_OBJECT = new object();

		internal VirtualFieldMetadata() : base(null)
		{
		}

		public abstract override void AddFieldIndex(MarshallerFamily mf, ClassMetadata yapClass
			, StatefulBuffer a_writer, Slot oldSlot);

		public override bool Alive()
		{
			return true;
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
			return ANY_OBJECT;
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

		public override void Instantiate(UnmarshallingContext context)
		{
			context.Reference().ProduceVirtualAttributes();
			Instantiate1(context.Transaction(), context.Reference(), context.Buffer());
		}

		internal abstract void Instantiate1(Transaction a_trans, ObjectReference a_yapObject
			, Db4objects.Db4o.Internal.Buffer a_bytes);

		public override void LoadHandler(ObjectContainerBase a_stream)
		{
		}

		public override void Marshall(MarshallingContext context, object obj)
		{
			context.DoNotIndirectWrites();
			Marshall(context.Transaction(), context.Reference(), context, context.IsNew());
		}

		private void Marshall(Transaction trans, ObjectReference @ref, IWriteBuffer buffer
			, bool isNew)
		{
			if (!trans.SupportsVirtualFields())
			{
				MarshallIgnore(buffer);
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
					if (@ref.VirtualAttributes() == null)
					{
						object obj = @ref.GetObject();
						ObjectReference migratingRef = null;
						MigrationConnection mgc = handlers.i_migration;
						if (mgc != null)
						{
							migratingRef = mgc.ReferenceFor(obj);
							if (migratingRef == null)
							{
								ObjectContainerBase peer = mgc.Peer(stream);
								migratingRef = peer.Transaction().ReferenceForObject(obj);
							}
						}
						if (migratingRef != null)
						{
							VirtualAttributes migrateAttributes = migratingRef.VirtualAttributes();
							if (migrateAttributes != null && migrateAttributes.i_database != null)
							{
								migrating = true;
								@ref.SetVirtualAttributes((VirtualAttributes)migrateAttributes.ShallowClone());
								migrateAttributes.i_database.Bind(trans);
							}
						}
					}
				}
				else
				{
					IDb4oReplicationReferenceProvider provider = handlers._replicationReferenceProvider;
					object parentObject = @ref.GetObject();
					IDb4oReplicationReference replicationReference = provider.ReferenceFor(parentObject
						);
					if (replicationReference != null)
					{
						migrating = true;
						VirtualAttributes va = @ref.ProduceVirtualAttributes();
						va.i_version = replicationReference.Version();
						va.i_uuid = replicationReference.LongPart();
						va.i_database = replicationReference.SignaturePart();
					}
				}
			}
			if (@ref.VirtualAttributes() == null)
			{
				@ref.ProduceVirtualAttributes();
				migrating = false;
			}
			Marshall(trans, @ref, buffer, migrating, isNew);
		}

		internal abstract void Marshall(Transaction trans, ObjectReference @ref, IWriteBuffer
			 buffer, bool migrating, bool isNew);

		internal abstract void MarshallIgnore(IWriteBuffer writer);

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
