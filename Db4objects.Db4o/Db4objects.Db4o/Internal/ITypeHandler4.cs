/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.IX;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ITypeHandler4 : IIndexable4
	{
		bool CanHold(IReflectClass claxx);

		void CascadeActivation(Transaction a_trans, object a_object, int a_depth, bool a_activate
			);

		IReflectClass ClassReflector();

		object Coerce(IReflectClass claxx, object obj);

		void CopyValue(object a_from, object a_to);

		void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes);

		int GetID();

		bool IsEqual(ITypeHandler4 a_dataType);

		bool HasFixedLength();

		bool IndexNullHandling();

		TernaryBool IsSecondClass();

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

		object IndexEntryToObject(Transaction trans, object indexEntry);

		void PrepareComparison(Transaction a_trans, object obj);

		IReflectClass PrimitiveClassReflector();

		object Read(MarshallerFamily mf, StatefulBuffer writer, bool redirect);

		object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer writer);

		object ReadQuery(Transaction trans, MarshallerFamily mf, bool withRedirection, Db4objects.Db4o.Internal.Buffer
			 reader, bool toArray);

		bool SupportsIndex();

		object WriteNew(MarshallerFamily mf, object a_object, bool topLevel, StatefulBuffer
			 a_bytes, bool withIndirection, bool restoreLinkOffset);

		int GetTypeID();

		ClassMetadata GetClassMetadata(ObjectContainerBase a_stream);

		/// <summary>performance optimized read (only used for byte[] so far)</summary>
		bool ReadArray(object array, Db4objects.Db4o.Internal.Buffer reader);

		void ReadCandidates(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer reader, 
			QCandidates candidates);

		ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer[]
			 a_bytes);

		/// <summary>performance optimized write (only used for byte[] so far)</summary>
		bool WriteArray(object array, Db4objects.Db4o.Internal.Buffer reader);

		QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer 
			reader, QCandidates candidates, bool withIndirection);

		void Defrag(MarshallerFamily mf, ReaderPair readers, bool redirect);
	}
}
