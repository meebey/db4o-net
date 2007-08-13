/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
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

		bool HasFixedLength();

		int LinkLength();

		/// <summary>
		/// The length calculation is different, depending from where we
		/// calculate.
		/// </summary>
		/// <remarks>
		/// The length calculation is different, depending from where we
		/// calculate. If we are still in the link area at the beginning of
		/// the slot, no data needs to be written to the payload area for
		/// primitive types, since they fully fit into the link area. If
		/// we are already writing something like an array (or deeper) to
		/// the payload area when we come here, a primitive does require
		/// space in the payload area.
		/// Differentiation is expressed with the 'topLevel' parameter.
		/// If 'topLevel==true' we are asking for a size calculation for
		/// the link area. If 'topLevel==false' we are asking for a size
		/// calculation for the payload area at the end of the slot.
		/// </remarks>
		void CalculateLengths(Transaction trans, ObjectHeaderAttributes header, bool topLevel
			, object obj, bool withIndirection);

		object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect);

		object ReadQuery(Transaction trans, MarshallerFamily mf, bool withRedirection, Db4objects.Db4o.Internal.Buffer
			 buffer, bool toArray);

		object Write(MarshallerFamily mf, object obj, bool topLevel, StatefulBuffer buffer
			, bool withIndirection, bool restoreLinkOffset);

		QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer 
			buffer, QCandidates candidates, bool withIndirection);

		void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect);
	}
}
