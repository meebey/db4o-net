using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeaderVariablePart1 : PersistentBase
	{
		private const int LENGTH = 1 + (Const4.INT_LENGTH * 4) + Const4.LONG_LENGTH + Const4
			.ADDED_LENGTH;

		private readonly Db4objects.Db4o.Internal.SystemData _systemData;

		public FileHeaderVariablePart1(int id, Db4objects.Db4o.Internal.SystemData systemData
			)
		{
			SetID(id);
			_systemData = systemData;
		}

		internal virtual Db4objects.Db4o.Internal.SystemData SystemData()
		{
			return _systemData;
		}

		public override byte GetIdentifier()
		{
			return Const4.HEADER;
		}

		public override int OwnLength()
		{
			return LENGTH;
		}

		public override void ReadThis(Transaction trans, Db4objects.Db4o.Internal.Buffer 
			reader)
		{
			_systemData.ConverterVersion(reader.ReadInt());
			_systemData.FreespaceSystem(reader.ReadByte());
			_systemData.FreespaceAddress(reader.ReadInt());
			ReadIdentity((LocalTransaction)trans, reader.ReadInt());
			_systemData.LastTimeStampID(reader.ReadLong());
			_systemData.UuidIndexId(reader.ReadInt());
		}

		public override void WriteThis(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 writer)
		{
			writer.WriteInt(_systemData.ConverterVersion());
			writer.Append(_systemData.FreespaceSystem());
			writer.WriteInt(_systemData.FreespaceAddress());
			writer.WriteInt(_systemData.Identity().GetID(trans));
			writer.WriteLong(_systemData.LastTimeStampID());
			writer.WriteInt(_systemData.UuidIndexId());
		}

		private void ReadIdentity(LocalTransaction trans, int identityID)
		{
			LocalObjectContainer file = trans.File();
			Db4oDatabase identity = (Db4oDatabase)file.GetByID1(trans, identityID);
			file.Activate1(trans, identity, 2);
			_systemData.Identity(identity);
		}
	}
}
