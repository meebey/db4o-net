namespace Db4objects.Db4o.Internal.Classindex
{
	/// <summary>client class index.</summary>
	/// <remarks>
	/// client class index. Largly intended to do nothing or
	/// redirect functionality to the server.
	/// </remarks>
	internal sealed class ClassIndexClient : Db4objects.Db4o.Internal.Classindex.ClassIndex
	{
		internal ClassIndexClient(Db4objects.Db4o.Internal.ClassMetadata aYapClass) : base
			(aYapClass)
		{
		}

		public override void Add(int a_id)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.VirtualException();
		}

		internal void EnsureActive()
		{
		}

		public override void Read(Db4objects.Db4o.Internal.Transaction a_trans)
		{
		}

		internal override void SetDirty(Db4objects.Db4o.Internal.ObjectContainerBase a_stream
			)
		{
		}

		public sealed override void WriteOwnID(Db4objects.Db4o.Internal.Transaction trans
			, Db4objects.Db4o.Internal.Buffer a_writer)
		{
			a_writer.WriteInt(0);
		}
	}
}
