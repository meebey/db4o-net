/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class UUIDFieldMetadata : VirtualFieldMetadata
	{
		private const int LINK_LENGTH = Const4.LONG_LENGTH + Const4.ID_LENGTH;

		internal UUIDFieldMetadata(ObjectContainerBase stream) : base()
		{
			SetName(Const4.VIRTUAL_FIELD_PREFIX + "uuid");
			i_handler = new LongHandler(stream);
		}

		public override void AddFieldIndex(MarshallerFamily mf, ClassMetadata yapClass, StatefulBuffer
			 writer, Slot oldSlot)
		{
			bool isnew = (oldSlot == null);
			int offset = writer._offset;
			int db4oDatabaseIdentityID = writer.ReadInt();
			long uuid = writer.ReadLong();
			writer._offset = offset;
			LocalObjectContainer yf = (LocalObjectContainer)writer.GetStream();
			if ((uuid == 0 || db4oDatabaseIdentityID == 0) && writer.GetID() > 0 && !isnew)
			{
				UUIDFieldMetadata.DatabaseIdentityIDAndUUID identityAndUUID = ReadDatabaseIdentityIDAndUUID
					(yf, yapClass, oldSlot, false);
				db4oDatabaseIdentityID = identityAndUUID.databaseIdentityID;
				uuid = identityAndUUID.uuid;
			}
			if (db4oDatabaseIdentityID == 0)
			{
				db4oDatabaseIdentityID = yf.Identity().GetID(writer.GetTransaction());
			}
			if (uuid == 0)
			{
				uuid = yf.GenerateTimeStampId();
			}
			writer.WriteInt(db4oDatabaseIdentityID);
			writer.WriteLong(uuid);
			if (isnew)
			{
				AddIndexEntry(writer, uuid);
			}
		}

		internal class DatabaseIdentityIDAndUUID
		{
			public int databaseIdentityID;

			public long uuid;

			public DatabaseIdentityIDAndUUID(int databaseIdentityID_, long uuid_)
			{
				databaseIdentityID = databaseIdentityID_;
				uuid = uuid_;
			}
		}

		private UUIDFieldMetadata.DatabaseIdentityIDAndUUID ReadDatabaseIdentityIDAndUUID
			(ObjectContainerBase stream, ClassMetadata yapClass, Slot oldSlot, bool checkClass
			)
		{
			Db4objects.Db4o.Internal.Buffer reader = stream.BufferByAddress(oldSlot.Address()
				, oldSlot.Length());
			if (checkClass)
			{
				ClassMetadata realClass = ClassMetadata.ReadClass(stream, reader);
				if (realClass != yapClass)
				{
					return null;
				}
			}
			if (null == yapClass.FindOffset(reader, this))
			{
				return null;
			}
			return new UUIDFieldMetadata.DatabaseIdentityIDAndUUID(reader.ReadInt(), reader.ReadLong
				());
		}

		public override void Delete(MarshallerFamily mf, StatefulBuffer a_bytes, bool isUpdate
			)
		{
			if (isUpdate)
			{
				a_bytes.IncrementOffset(LinkLength());
				return;
			}
			a_bytes.IncrementOffset(Const4.INT_LENGTH);
			long longPart = a_bytes.ReadLong();
			if (longPart > 0)
			{
				ObjectContainerBase stream = a_bytes.GetStream();
				if (stream.MaintainsIndices())
				{
					RemoveIndexEntry(a_bytes.GetTransaction(), a_bytes.GetID(), longPart);
				}
			}
		}

		public override bool HasIndex()
		{
			return true;
		}

		public override BTree GetIndex(Transaction transaction)
		{
			EnsureIndex(transaction);
			return base.GetIndex(transaction);
		}

		protected override void RebuildIndexForObject(LocalObjectContainer stream, ClassMetadata
			 yapClass, int objectId)
		{
			UUIDFieldMetadata.DatabaseIdentityIDAndUUID data = ReadDatabaseIdentityIDAndUUID(
				stream, yapClass, ((LocalTransaction)stream.SystemTransaction()).GetCurrentSlotOfID
				(objectId), true);
			if (null == data)
			{
				return;
			}
			AddIndexEntry(stream.GetLocalSystemTransaction(), objectId, data.uuid);
		}

		private void EnsureIndex(Transaction transaction)
		{
			if (null == transaction)
			{
				throw new ArgumentNullException();
			}
			if (null != base.GetIndex(transaction))
			{
				return;
			}
			LocalObjectContainer file = ((LocalObjectContainer)transaction.Container());
			SystemData sd = file.SystemData();
			if (sd == null)
			{
				return;
			}
			InitIndex(transaction, sd.UuidIndexId());
			if (sd.UuidIndexId() == 0)
			{
				sd.UuidIndexId(base.GetIndex(transaction).GetID());
				file.GetFileHeader().WriteVariablePart(file, 1);
			}
		}

		internal override void Instantiate1(Transaction a_trans, ObjectReference a_yapObject
			, Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			int dbID = a_bytes.ReadInt();
			ObjectContainerBase stream = a_trans.Container();
			stream.ShowInternalClasses(true);
			try
			{
				Db4oDatabase db = (Db4oDatabase)stream.GetByID2(a_trans, dbID);
				if (db != null && db.i_signature == null)
				{
					stream.Activate(a_trans, db, 2);
				}
				VirtualAttributes va = a_yapObject.VirtualAttributes();
				va.i_database = db;
				va.i_uuid = a_bytes.ReadLong();
			}
			finally
			{
				stream.ShowInternalClasses(false);
			}
		}

		public override int LinkLength()
		{
			return LINK_LENGTH;
		}

		internal override void Marshall1(ObjectReference a_yapObject, StatefulBuffer a_bytes
			, bool a_migrating, bool a_new)
		{
			ObjectContainerBase stream = a_bytes.GetStream();
			Transaction trans = a_bytes.GetTransaction();
			bool indexEntry = a_new && stream.MaintainsIndices();
			int dbID = 0;
			VirtualAttributes attr = a_yapObject.VirtualAttributes();
			bool linkToDatabase = !a_migrating;
			if (attr != null && attr.i_database == null)
			{
				linkToDatabase = true;
			}
			if (linkToDatabase)
			{
				Db4oDatabase db = ((IInternalObjectContainer)stream).Identity();
				if (db == null)
				{
					attr = null;
				}
				else
				{
					if (attr.i_database == null)
					{
						attr.i_database = db;
						if (stream is LocalObjectContainer)
						{
							attr.i_uuid = stream.GenerateTimeStampId();
							indexEntry = true;
						}
					}
					db = attr.i_database;
					if (db != null)
					{
						dbID = db.GetID(trans);
					}
				}
			}
			else
			{
				if (attr != null)
				{
					dbID = attr.i_database.GetID(trans);
				}
			}
			a_bytes.WriteInt(dbID);
			if (attr != null)
			{
				a_bytes.WriteLong(attr.i_uuid);
				if (indexEntry)
				{
					AddIndexEntry(a_bytes, attr.i_uuid);
				}
			}
			else
			{
				a_bytes.WriteLong(0);
			}
		}

		internal override void MarshallIgnore(Db4objects.Db4o.Internal.Buffer writer)
		{
			writer.WriteInt(0);
			writer.WriteLong(0);
		}

		public HardObjectReference GetHardObjectReferenceBySignature(Transaction transaction
			, long longPart, byte[] signature)
		{
			IBTreeRange range = Search(transaction, longPart);
			IEnumerator keys = range.Keys();
			while (keys.MoveNext())
			{
				FieldIndexKey current = (FieldIndexKey)keys.Current;
				HardObjectReference hardRef = GetHardObjectReferenceById(transaction, current.ParentID
					(), signature);
				if (null != hardRef)
				{
					return hardRef;
				}
			}
			return HardObjectReference.INVALID;
		}

		protected HardObjectReference GetHardObjectReferenceById(Transaction transaction, 
			int parentId, byte[] signature)
		{
			HardObjectReference hardRef = transaction.Container().GetHardObjectReferenceById(
				transaction, parentId);
			if (hardRef._reference == null)
			{
				return null;
			}
			VirtualAttributes vad = hardRef._reference.VirtualAttributes(transaction);
			if (!Arrays4.AreEqual(signature, vad.i_database.i_signature))
			{
				return null;
			}
			return hardRef;
		}

		public override void DefragField(MarshallerFamily mf, BufferPair readers)
		{
			readers.CopyID();
			readers.IncrementOffset(Const4.LONG_LENGTH);
		}
	}
}
