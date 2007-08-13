/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class UntypedMarshaller
	{
		internal MarshallerFamily _family;

		public abstract void DeleteEmbedded(StatefulBuffer reader);

		public abstract object WriteNew(object obj, bool restoreLinkOffset, StatefulBuffer
			 writer);

		public abstract object Read(StatefulBuffer reader);

		public abstract ITypeHandler4 ReadArrayHandler(Transaction a_trans, Db4objects.Db4o.Internal.Buffer[]
			 a_bytes);

		public abstract bool UseNormalClassRead();

		public abstract object ReadQuery(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader, bool toArray);

		public abstract QCandidate ReadSubCandidate(Db4objects.Db4o.Internal.Buffer reader
			, QCandidates candidates, bool withIndirection);

		public abstract void Defrag(BufferPair readers);
	}
}
