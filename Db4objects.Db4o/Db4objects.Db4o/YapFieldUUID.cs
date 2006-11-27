namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapFieldUUID : Db4objects.Db4o.YapFieldVirtual
	{
		private const int LINK_LENGTH = Db4objects.Db4o.YapConst.LONG_LENGTH + Db4objects.Db4o.YapConst
			.ID_LENGTH;

		internal YapFieldUUID(Db4objects.Db4o.YapStream stream) : base()
		{
			i_name = Db4objects.Db4o.YapConst.VIRTUAL_FIELD_PREFIX + "uuid";
			i_handler = new Db4objects.Db4o.YLong(stream);
		}

		public override void AddFieldIndex(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapWriter writer, Db4objects.Db4o.Inside.Slots.Slot
			 oldSlot)
		{
			bool isnew = (oldSlot == null);
			int offset = writer._offset;
			int db4oDatabaseIdentityID = writer.ReadInt();
			long uuid = writer.ReadLong();
			writer._offset = offset;
			Db4objects.Db4o.YapFile yf = (Db4objects.Db4o.YapFile)writer.GetStream();
			if ((uuid == 0 || db4oDatabaseIdentityID == 0) && writer.GetID() > 0 && !isnew)
			{
				Db4objects.Db4o.YapFieldUUID.DatabaseIdentityIDAndUUID identityAndUUID = ReadDatabaseIdentityIDAndUUID
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

		private Db4objects.Db4o.YapFieldUUID.DatabaseIdentityIDAndUUID ReadDatabaseIdentityIDAndUUID
			(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.Inside.Slots.Slot
			 oldSlot, bool checkClass)
		{
			Db4objects.Db4o.YapReader reader = stream.ReadReaderByAddress(oldSlot.GetAddress(
				), oldSlot.GetLength());
			if (checkClass)
			{
				Db4objects.Db4o.YapClass realClass = Db4objects.Db4o.YapClass.ReadClass(stream, reader
					);
				if (realClass != yapClass)
				{
					return null;
				}
			}
			if (null == yapClass.FindOffset(reader, this))
			{
				return null;
			}
			return new Db4objects.Db4o.YapFieldUUID.DatabaseIdentityIDAndUUID(reader.ReadInt(
				), reader.ReadLong());
		}

		public override void Delete(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, 
			Db4objects.Db4o.YapWriter a_bytes, bool isUpdate)
		{
			if (isUpdate)
			{
				a_bytes.IncrementOffset(LinkLength());
				return;
			}
			a_bytes.IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
			long longPart = a_bytes.ReadLong();
			if (longPart > 0)
			{
				Db4objects.Db4o.YapStream stream = a_bytes.GetStream();
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

		public override Db4objects.Db4o.Inside.Btree.BTree GetIndex(Db4objects.Db4o.Transaction
			 transaction)
		{
			EnsureIndex(transaction);
			return base.GetIndex(transaction);
		}

		protected override void RebuildIndexForObject(Db4objects.Db4o.YapFile stream, Db4objects.Db4o.YapClass
			 yapClass, int objectId)
		{
			Db4objects.Db4o.YapFieldUUID.DatabaseIdentityIDAndUUID data = ReadDatabaseIdentityIDAndUUID
				(stream, yapClass, stream.GetSystemTransaction().GetCurrentSlotOfID(objectId), true
				);
			if (null == data)
			{
				return;
			}
			AddIndexEntry(stream.GetSystemTransaction(), objectId, data.uuid);
		}

		private void EnsureIndex(Db4objects.Db4o.Transaction transaction)
		{
			if (null == transaction)
			{
				throw new System.ArgumentNullException();
			}
			if (null != base.GetIndex(transaction))
			{
				return;
			}
			Db4objects.Db4o.YapFile file = ((Db4objects.Db4o.YapFile)transaction.Stream());
			Db4objects.Db4o.Inside.SystemData sd = file.SystemData();
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

		internal override void Instantiate1(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapObject
			 a_yapObject, Db4objects.Db4o.YapReader a_bytes)
		{
			int dbID = a_bytes.ReadInt();
			Db4objects.Db4o.YapStream stream = a_trans.Stream();
			stream.ShowInternalClasses(true);
			Db4objects.Db4o.Ext.Db4oDatabase db = (Db4objects.Db4o.Ext.Db4oDatabase)stream.GetByID2
				(a_trans, dbID);
			if (db != null && db.i_signature == null)
			{
				stream.Activate2(a_trans, db, 2);
			}
			Db4objects.Db4o.VirtualAttributes va = a_yapObject.VirtualAttributes();
			va.i_database = db;
			va.i_uuid = a_bytes.ReadLong();
			stream.ShowInternalClasses(false);
		}

		public override int LinkLength()
		{
			return LINK_LENGTH;
		}

		internal override void Marshall1(Db4objects.Db4o.YapObject a_yapObject, Db4objects.Db4o.YapWriter
			 a_bytes, bool a_migrating, bool a_new)
		{
			Db4objects.Db4o.YapStream stream = a_bytes.GetStream();
			Db4objects.Db4o.Transaction trans = a_bytes.GetTransaction();
			bool indexEntry = a_new && stream.MaintainsIndices();
			int dbID = 0;
			Db4objects.Db4o.VirtualAttributes attr = a_yapObject.VirtualAttributes();
			bool linkToDatabase = !a_migrating;
			if (attr != null && attr.i_database == null)
			{
				linkToDatabase = true;
			}
			if (linkToDatabase)
			{
				Db4objects.Db4o.Ext.Db4oDatabase db = stream.Identity();
				if (db == null)
				{
					attr = null;
				}
				else
				{
					if (attr.i_database == null)
					{
						attr.i_database = db;
						if (stream is Db4objects.Db4o.YapFile)
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

		internal override void MarshallIgnore(Db4objects.Db4o.YapReader writer)
		{
			writer.WriteInt(0);
			writer.WriteLong(0);
		}

		public virtual object[] ObjectAndYapObjectBySignature(Db4objects.Db4o.Transaction
			 transaction, long longPart, byte[] signature)
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange range = Search(transaction, longPart);
			System.Collections.IEnumerator keys = range.Keys();
			while (keys.MoveNext())
			{
				Db4objects.Db4o.Inside.Btree.FieldIndexKey current = (Db4objects.Db4o.Inside.Btree.FieldIndexKey
					)keys.Current;
				object[] objectAndYapObject = GetObjectAndYapObjectByID(transaction, current.ParentID
					(), signature);
				if (null != objectAndYapObject)
				{
					return objectAndYapObject;
				}
			}
			return new object[2];
		}

		protected virtual object[] GetObjectAndYapObjectByID(Db4objects.Db4o.Transaction 
			transaction, int parentId, byte[] signature)
		{
			object[] arr = transaction.Stream().GetObjectAndYapObjectByID(transaction, parentId
				);
			if (arr[1] == null)
			{
				return null;
			}
			Db4objects.Db4o.YapObject yod = (Db4objects.Db4o.YapObject)arr[1];
			Db4objects.Db4o.VirtualAttributes vad = yod.VirtualAttributes(transaction);
			if (!Db4objects.Db4o.Foundation.Arrays4.AreEqual(signature, vad.i_database.i_signature
				))
			{
				return null;
			}
			return arr;
		}

		public override void DefragField(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.ReaderPair readers)
		{
			readers.CopyID();
			readers.IncrementOffset(Db4objects.Db4o.YapConst.LONG_LENGTH);
		}
	}
}
