namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class ArrayMarshaller
	{
		public Db4objects.Db4o.Internal.Marshall.MarshallerFamily _family;

		public abstract void DeleteEmbedded(Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 arrayHandler, Db4objects.Db4o.Internal.StatefulBuffer reader);

		public Db4objects.Db4o.Internal.TreeInt CollectIDs(Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 arrayHandler, Db4objects.Db4o.Internal.TreeInt tree, Db4objects.Db4o.Internal.StatefulBuffer
			 reader)
		{
			Db4objects.Db4o.Internal.Transaction trans = reader.GetTransaction();
			return arrayHandler.CollectIDs1(trans, tree, PrepareIDReader(trans, reader));
		}

		public abstract void DefragIDs(Db4objects.Db4o.Internal.Handlers.ArrayHandler arrayHandler
			, Db4objects.Db4o.Internal.ReaderPair readers);

		public abstract void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 handler, object obj, bool topLevel);

		public abstract object Read(Db4objects.Db4o.Internal.Handlers.ArrayHandler arrayHandler
			, Db4objects.Db4o.Internal.StatefulBuffer reader);

		public abstract void ReadCandidates(Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 arrayHandler, Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates);

		public abstract object ReadQuery(Db4objects.Db4o.Internal.Handlers.ArrayHandler arrayHandler
			, Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer reader
			);

		public abstract object WriteNew(Db4objects.Db4o.Internal.Handlers.ArrayHandler arrayHandler
			, object obj, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer writer);

		protected abstract Db4objects.Db4o.Internal.Buffer PrepareIDReader(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Buffer reader);
	}
}
