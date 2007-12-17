/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

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

		public override void ReadThis(Transaction trans, BufferImpl reader)
		{
			_systemData.ConverterVersion(reader.ReadInt());
			_systemData.FreespaceSystem(reader.ReadByte());
			_systemData.FreespaceAddress(reader.ReadInt());
			ReadIdentity((LocalTransaction)trans, reader.ReadInt());
			_systemData.LastTimeStampID(reader.ReadLong());
			_systemData.UuidIndexId(reader.ReadInt());
		}

		public override void WriteThis(Transaction trans, BufferImpl writer)
		{
			writer.WriteInt(_systemData.ConverterVersion());
			writer.WriteByte(_systemData.FreespaceSystem());
			writer.WriteInt(_systemData.FreespaceAddress());
			writer.WriteInt(_systemData.Identity().GetID(trans));
			writer.WriteLong(_systemData.LastTimeStampID());
			writer.WriteInt(_systemData.UuidIndexId());
		}

		private void ReadIdentity(LocalTransaction trans, int identityID)
		{
			LocalObjectContainer file = trans.File();
			Db4oDatabase identity = Debug.staticIdentity ? Db4oDatabase.STATIC_IDENTITY : (Db4oDatabase
				)file.GetByID(trans, identityID);
			file.Activate(trans, identity, new FixedActivationDepth(2));
			_systemData.Identity(identity);
		}
	}
}
