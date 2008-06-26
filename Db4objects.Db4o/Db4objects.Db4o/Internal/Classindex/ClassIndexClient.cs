/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Classindex
{
	/// <summary>client class index.</summary>
	/// <remarks>
	/// client class index. Largly intended to do nothing or
	/// redirect functionality to the server.
	/// </remarks>
	internal sealed class ClassIndexClient : Db4objects.Db4o.Internal.Classindex.ClassIndex
	{
		internal ClassIndexClient(ClassMetadata aYapClass) : base(aYapClass)
		{
		}

		public override void Add(int a_id)
		{
			throw Exceptions4.VirtualException();
		}

		internal void EnsureActive()
		{
		}

		// do nothing
		public override void Read(Transaction a_trans)
		{
		}

		// do nothing
		internal override void SetDirty(ObjectContainerBase a_stream)
		{
		}

		// do nothing
		public sealed override void WriteOwnID(Transaction trans, ByteArrayBuffer a_writer
			)
		{
			a_writer.WriteInt(0);
		}
	}
}
