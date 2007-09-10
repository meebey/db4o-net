/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ITypeHandler4 : IComparable4
	{
		void CascadeActivation(Transaction trans, object obj, int depth, bool activate);

		IReflectClass ClassReflector();

		void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer buffer);

		int GetID();

		int LinkLength();

		object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect);

		object ReadQuery(Transaction trans, MarshallerFamily mf, bool withRedirection, Db4objects.Db4o.Internal.Buffer
			 buffer, bool toArray);

		QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer 
			buffer, QCandidates candidates, bool withIndirection);

		void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect);

		object Read(IReadContext context);

		void Write(IWriteContext context, object obj);
	}
}
