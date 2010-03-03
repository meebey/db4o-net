/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeaderVariablePart2 : FileHeaderVariablePart1
	{
		private const int Length = 2 + (Const4.IntLength * 5) + Const4.LongLength + Const4
			.AddedLength;

		public FileHeaderVariablePart2(LocalObjectContainer container, int id, SystemData
			 systemData) : base(container, id, systemData)
		{
		}

		// The variable part format is:
		// (int) converter version
		// (byte) freespace system used
		// (int)  freespace address
		// (int) identity ID
		// (long) versionGenerator
		// (int) uuid index ID
		// (byte) idSystem
		// (int) idSystem ID
		public override int OwnLength()
		{
			return Length;
		}

		public override void ReadThis(ByteArrayBuffer reader)
		{
			base.ReadThis(reader);
			_systemData.IdSystemType(reader.ReadByte());
			_systemData.IdSystemID(reader.ReadInt());
		}

		public override void WriteThis(ByteArrayBuffer writer)
		{
			base.WriteThis(writer);
			writer.WriteByte(_systemData.IdSystemType());
			writer.WriteInt(_systemData.IdSystemID());
		}
	}
}
