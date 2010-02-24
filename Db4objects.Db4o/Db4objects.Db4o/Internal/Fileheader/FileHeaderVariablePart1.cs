/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeaderVariablePart1 : PersistentBase
	{
		private const int Length = 2 + (Const4.IntLength * 5) + Const4.LongLength + Const4
			.AddedLength;

		private readonly LocalObjectContainer _container;

		private readonly SystemData _systemData;

		private int _identityId;

		public FileHeaderVariablePart1(LocalObjectContainer container, int id, SystemData
			 systemData)
		{
			// The variable part format is:
			// (int) converter version
			// (byte) freespace system used
			// (int)  freespace address
			// (int) identity ID
			// (long) versionGenerator
			// (int) uuid index ID
			// (byte) idSystem
			// (int) idSystem ID
			SetID(id);
			_container = container;
			_systemData = systemData;
		}

		public override byte GetIdentifier()
		{
			return Const4.Header;
		}

		public override int OwnLength()
		{
			return Length;
		}

		public override void ReadThis(Transaction trans, ByteArrayBuffer reader)
		{
			_systemData.ConverterVersion(reader.ReadInt());
			_systemData.FreespaceSystem(reader.ReadByte());
			_systemData.FreespaceAddress(reader.ReadInt());
			_identityId = reader.ReadInt();
			_systemData.LastTimeStampID(reader.ReadLong());
			_systemData.UuidIndexId(reader.ReadInt());
			if (reader.Eof())
			{
				// older versions of the file header don't have IdSystem information.
				return;
			}
			_systemData.IdSystemType(reader.ReadByte());
			_systemData.IdSystemID(reader.ReadInt());
		}

		public override void WriteThis(Transaction trans, ByteArrayBuffer writer)
		{
			writer.WriteInt(_systemData.ConverterVersion());
			writer.WriteByte(_systemData.FreespaceSystem());
			writer.WriteInt(_systemData.FreespaceAddress());
			writer.WriteInt(_systemData.Identity().GetID(trans));
			writer.WriteLong(_systemData.LastTimeStampID());
			writer.WriteInt(_systemData.UuidIndexId());
			writer.WriteByte(_systemData.IdSystemType());
			writer.WriteInt(_systemData.IdSystemID());
		}

		public virtual void ReadIdentity(LocalTransaction trans)
		{
			LocalObjectContainer file = trans.LocalContainer();
			Db4oDatabase identity = Debug4.staticIdentity ? Db4oDatabase.StaticIdentity : (Db4oDatabase
				)file.GetByID(trans, _identityId);
			if (null != identity)
			{
				// TODO: what?
				file.Activate(trans, identity, new FixedActivationDepth(2));
				_systemData.Identity(identity);
			}
		}

		protected override ByteArrayBuffer ReadBufferById(Transaction trans)
		{
			Slot slot = _container.ReadPointerSlot(_id);
			return _container.ReadBufferBySlot(slot);
		}
	}
}
