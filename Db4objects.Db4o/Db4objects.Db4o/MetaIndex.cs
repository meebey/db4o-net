/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <summary>The index record that is written to the database file.</summary>
	/// <remarks>
	/// The index record that is written to the database file.
	/// Don't obfuscate.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class MetaIndex : IInternal4
	{
		public int indexAddress;

		public int indexEntries;

		public int indexLength;

		private const int patchAddress = 0;

		private const int patchEntries = 0;

		private const int patchLength = 0;

		public virtual void Read(BufferImpl reader)
		{
			indexAddress = reader.ReadInt();
			indexEntries = reader.ReadInt();
			indexLength = reader.ReadInt();
			reader.ReadInt();
			reader.ReadInt();
			reader.ReadInt();
		}

		public virtual void Write(BufferImpl writer)
		{
			writer.WriteInt(indexAddress);
			writer.WriteInt(indexEntries);
			writer.WriteInt(indexLength);
			writer.WriteInt(patchAddress);
			writer.WriteInt(patchEntries);
			writer.WriteInt(patchLength);
		}

		public virtual void Free(LocalObjectContainer file)
		{
			file.Free(indexAddress, indexLength);
			indexAddress = 0;
			indexLength = 0;
		}
	}
}
