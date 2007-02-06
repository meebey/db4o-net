namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class UntypedMarshaller
	{
		internal Db4objects.Db4o.Internal.Marshall.MarshallerFamily _family;

		public abstract void DeleteEmbedded(Db4objects.Db4o.Internal.StatefulBuffer reader
			);

		public abstract object WriteNew(object obj, bool restoreLinkOffset, Db4objects.Db4o.Internal.StatefulBuffer
			 writer);

		public abstract object Read(Db4objects.Db4o.Internal.StatefulBuffer reader);

		public abstract Db4objects.Db4o.Internal.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Internal.Transaction
			 a_trans, Db4objects.Db4o.Internal.Buffer[] a_bytes);

		public abstract bool UseNormalClassRead();

		public abstract object ReadQuery(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader, bool toArray);

		public abstract Db4objects.Db4o.Internal.Query.Processor.QCandidate ReadSubCandidate
			(Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates, bool withIndirection);

		public abstract void Defrag(Db4objects.Db4o.Internal.ReaderPair readers);
	}
}
