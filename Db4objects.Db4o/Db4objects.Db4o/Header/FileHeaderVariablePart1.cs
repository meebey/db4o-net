namespace Db4objects.Db4o.Header
{
	/// <exclude></exclude>
	public class FileHeaderVariablePart1 : Db4objects.Db4o.YapMeta
	{
		private const int LENGTH = 1 + (Db4objects.Db4o.YapConst.INT_LENGTH * 4) + Db4objects.Db4o.YapConst
			.LONG_LENGTH + Db4objects.Db4o.YapConst.ADDED_LENGTH;

		private readonly Db4objects.Db4o.Inside.SystemData _systemData;

		public FileHeaderVariablePart1(int id, Db4objects.Db4o.Inside.SystemData systemData
			)
		{
			SetID(id);
			_systemData = systemData;
		}

		internal virtual Db4objects.Db4o.Inside.SystemData SystemData()
		{
			return _systemData;
		}

		public override byte GetIdentifier()
		{
			return Db4objects.Db4o.YapConst.HEADER;
		}

		public override int OwnLength()
		{
			return LENGTH;
		}

		public override void ReadThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 reader)
		{
			_systemData.ConverterVersion(reader.ReadInt());
			_systemData.FreespaceSystem(reader.ReadByte());
			_systemData.FreespaceAddress(reader.ReadInt());
			ReadIdentity(trans, reader.ReadInt());
			_systemData.LastTimeStampID(reader.ReadLong());
			_systemData.UuidIndexId(reader.ReadInt());
		}

		public override void WriteThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 writer)
		{
			writer.WriteInt(_systemData.ConverterVersion());
			writer.Append(_systemData.FreespaceSystem());
			writer.WriteInt(_systemData.FreespaceAddress());
			writer.WriteInt(_systemData.Identity().GetID(trans));
			writer.WriteLong(_systemData.LastTimeStampID());
			writer.WriteInt(_systemData.UuidIndexId());
		}

		private void ReadIdentity(Db4objects.Db4o.Transaction trans, int identityID)
		{
			Db4objects.Db4o.YapFile file = trans.i_file;
			Db4objects.Db4o.Ext.Db4oDatabase identity = (Db4objects.Db4o.Ext.Db4oDatabase)file
				.GetByID1(trans, identityID);
			file.Activate1(trans, identity, 2);
			_systemData.Identity(identity);
		}
	}
}
