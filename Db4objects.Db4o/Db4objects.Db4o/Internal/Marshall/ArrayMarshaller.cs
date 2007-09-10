/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class ArrayMarshaller
	{
		public MarshallerFamily _family;

		public abstract void DeleteEmbedded(ArrayHandler arrayHandler, StatefulBuffer reader
			);

		public TreeInt CollectIDs(ArrayHandler arrayHandler, TreeInt tree, StatefulBuffer
			 reader)
		{
			Transaction trans = reader.GetTransaction();
			return arrayHandler.CollectIDs1(trans, tree, PrepareIDReader(trans, reader));
		}

		public abstract void DefragIDs(ArrayHandler arrayHandler, BufferPair readers);

		public abstract object Read(ArrayHandler arrayHandler, StatefulBuffer reader);

		public abstract void ReadCandidates(ArrayHandler arrayHandler, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates);

		public abstract object ReadQuery(ArrayHandler arrayHandler, Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader);

		protected abstract Db4objects.Db4o.Internal.Buffer PrepareIDReader(Transaction trans
			, Db4objects.Db4o.Internal.Buffer reader);
	}
}
