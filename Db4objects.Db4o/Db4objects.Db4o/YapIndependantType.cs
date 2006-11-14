namespace Db4objects.Db4o
{
	/// <summary>
	/// Common base class for YapString and YapArray:
	/// There is one indirection in the database file to this.
	/// </summary>
	/// <remarks>
	/// Common base class for YapString and YapArray:
	/// There is one indirection in the database file to this.
	/// </remarks>
	/// <exclude></exclude>
	public abstract class YapIndependantType : Db4objects.Db4o.ITypeHandler4
	{
		internal readonly Db4objects.Db4o.YapStream _stream;

		public YapIndependantType(Db4objects.Db4o.YapStream stream)
		{
			_stream = stream;
		}

		public virtual object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object 
			obj)
		{
			return CanHold(claxx) ? obj : Db4objects.Db4o.Foundation.No4.INSTANCE;
		}

		public void CopyValue(object a_from, object a_to)
		{
		}

		public abstract void DeleteEmbedded(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_bytes);

		public virtual bool HasFixedLength()
		{
			return false;
		}

		public int LinkLength()
		{
			return Db4objects.Db4o.YapConst.INT_LENGTH + Db4objects.Db4o.YapConst.ID_LENGTH;
		}

		public Db4objects.Db4o.Reflect.IReflectClass PrimitiveClassReflector()
		{
			return null;
		}

		public virtual bool ReadArray(object array, Db4objects.Db4o.YapReader reader)
		{
			return false;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_writer)
		{
			return Read(mf, a_writer, true);
		}

		public virtual bool WriteArray(object array, Db4objects.Db4o.YapReader reader)
		{
			return false;
		}

		public abstract bool IsGreater(object obj);

		public abstract Db4objects.Db4o.IYapComparable PrepareComparison(object obj);

		public abstract int CompareTo(object obj);

		public abstract bool IsEqual(object obj);

		public abstract bool IsSmaller(object obj);

		public abstract object ComparableObject(Db4objects.Db4o.Transaction trans, object
			 indexEntry);

		public abstract object ReadIndexEntry(Db4objects.Db4o.YapReader a_reader);

		public abstract void WriteIndexEntry(Db4objects.Db4o.YapReader a_writer, object a_object
			);

		public abstract void Defrag(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, 
			Db4objects.Db4o.ReaderPair readers, bool redirect);

		public abstract object Current();

		public abstract void DefragIndexEntry(Db4objects.Db4o.ReaderPair arg1);

		public abstract void CalculateLengths(Db4objects.Db4o.Transaction arg1, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 arg2, bool arg3, object arg4, bool arg5);

		public abstract bool CanHold(Db4objects.Db4o.Reflect.IReflectClass arg1);

		public abstract void CascadeActivation(Db4objects.Db4o.Transaction arg1, object arg2
			, int arg3, bool arg4);

		public abstract Db4objects.Db4o.Reflect.IReflectClass ClassReflector();

		public abstract bool Equals(Db4objects.Db4o.ITypeHandler4 arg1);

		public abstract int GetID();

		public abstract int GetTypeID();

		public abstract Db4objects.Db4o.YapClass GetYapClass(Db4objects.Db4o.YapStream arg1
			);

		public abstract object IndexEntryToObject(Db4objects.Db4o.Transaction arg1, object
			 arg2);

		public abstract bool IndexNullHandling();

		public abstract int IsSecondClass();

		public abstract void PrepareComparison(Db4objects.Db4o.Transaction arg1, object arg2
			);

		public abstract object Read(Db4objects.Db4o.Inside.Marshall.MarshallerFamily arg1
			, Db4objects.Db4o.YapWriter arg2, bool arg3);

		public abstract Db4objects.Db4o.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Transaction
			 arg1, Db4objects.Db4o.Inside.Marshall.MarshallerFamily arg2, Db4objects.Db4o.YapReader[]
			 arg3);

		public abstract void ReadCandidates(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 arg1, Db4objects.Db4o.YapReader arg2, Db4objects.Db4o.QCandidates arg3);

		public abstract object ReadQuery(Db4objects.Db4o.Transaction arg1, Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 arg2, bool arg3, Db4objects.Db4o.YapReader arg4, bool arg5);

		public abstract Db4objects.Db4o.QCandidate ReadSubCandidate(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 arg1, Db4objects.Db4o.YapReader arg2, Db4objects.Db4o.QCandidates arg3, bool arg4
			);

		public abstract bool SupportsIndex();

		public abstract object WriteNew(Db4objects.Db4o.Inside.Marshall.MarshallerFamily 
			arg1, object arg2, bool arg3, Db4objects.Db4o.YapWriter arg4, bool arg5, bool arg6
			);
	}
}
