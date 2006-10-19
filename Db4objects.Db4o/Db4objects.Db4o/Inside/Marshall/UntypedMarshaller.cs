namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public abstract class UntypedMarshaller
	{
		internal Db4objects.Db4o.Inside.Marshall.MarshallerFamily _family;

		public abstract void DeleteEmbedded(Db4objects.Db4o.YapWriter reader);

		public abstract object WriteNew(object obj, bool restoreLinkOffset, Db4objects.Db4o.YapWriter
			 writer);

		public abstract object Read(Db4objects.Db4o.YapWriter reader);

		public abstract Db4objects.Db4o.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Transaction
			 a_trans, Db4objects.Db4o.YapReader[] a_bytes);

		public abstract bool UseNormalClassRead();

		public abstract object ReadQuery(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 reader, bool toArray);

		public abstract Db4objects.Db4o.QCandidate ReadSubCandidate(Db4objects.Db4o.YapReader
			 reader, Db4objects.Db4o.QCandidates candidates, bool withIndirection);
	}
}
