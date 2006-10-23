namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public abstract class ArrayMarshaller
	{
		public Db4objects.Db4o.Inside.Marshall.MarshallerFamily _family;

		public abstract void DeleteEmbedded(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.YapWriter
			 reader);

		public Db4objects.Db4o.TreeInt CollectIDs(Db4objects.Db4o.YapArray arrayHandler, 
			Db4objects.Db4o.TreeInt tree, Db4objects.Db4o.YapWriter reader)
		{
			Db4objects.Db4o.Transaction trans = reader.GetTransaction();
			return arrayHandler.CollectIDs1(trans, tree, PrepareIDReader(trans, reader));
		}

		public abstract void DefragIDs(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.ReaderPair
			 readers);

		public abstract void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, Db4objects.Db4o.YapArray handler, object obj, bool topLevel);

		public abstract object Read(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.YapWriter
			 reader);

		public abstract void ReadCandidates(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.YapReader
			 reader, Db4objects.Db4o.QCandidates candidates);

		public abstract object ReadQuery(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapReader reader);

		public abstract object WriteNew(Db4objects.Db4o.YapArray arrayHandler, object obj
			, bool topLevel, Db4objects.Db4o.YapWriter writer);

		protected abstract Db4objects.Db4o.YapReader PrepareIDReader(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapReader reader);
	}
}
