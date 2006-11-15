namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface ITypeHandler4 : Db4objects.Db4o.Inside.IX.IIndexable4
	{
		bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx);

		void CascadeActivation(Db4objects.Db4o.Transaction a_trans, object a_object, int 
			a_depth, bool a_activate);

		Db4objects.Db4o.Reflect.IReflectClass ClassReflector();

		object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object obj);

		void CopyValue(object a_from, object a_to);

		void DeleteEmbedded(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapWriter
			 a_bytes);

		int GetID();

		bool Equals(Db4objects.Db4o.ITypeHandler4 a_dataType);

		bool HasFixedLength();

		bool IndexNullHandling();

		int IsSecondClass();

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
		void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, bool topLevel, object obj, bool withIndirection);

		object IndexEntryToObject(Db4objects.Db4o.Transaction trans, object indexEntry);

		void PrepareComparison(Db4objects.Db4o.Transaction a_trans, object obj);

		Db4objects.Db4o.Reflect.IReflectClass PrimitiveClassReflector();

		object Read(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapWriter
			 writer, bool redirect);

		object ReadIndexEntry(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapWriter
			 writer);

		object ReadQuery(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, bool withRedirection, Db4objects.Db4o.YapReader reader, bool toArray);

		bool SupportsIndex();

		object WriteNew(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, object a_object
			, bool topLevel, Db4objects.Db4o.YapWriter a_bytes, bool withIndirection, bool restoreLinkOffset
			);

		int GetTypeID();

		Db4objects.Db4o.YapClass GetYapClass(Db4objects.Db4o.YapStream a_stream);

		/// <summary>performance optimized read (only used for byte[] so far)</summary>
		bool ReadArray(object array, Db4objects.Db4o.YapReader reader);

		void ReadCandidates(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapReader
			 reader, Db4objects.Db4o.QCandidates candidates);

		Db4objects.Db4o.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Transaction a_trans
			, Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapReader[]
			 a_bytes);

		/// <summary>performance optimized write (only used for byte[] so far)</summary>
		bool WriteArray(object array, Db4objects.Db4o.YapReader reader);

		Db4objects.Db4o.QCandidate ReadSubCandidate(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapReader reader, Db4objects.Db4o.QCandidates candidates, bool
			 withIndirection);

		void Defrag(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.ReaderPair
			 readers, bool redirect);
	}
}
