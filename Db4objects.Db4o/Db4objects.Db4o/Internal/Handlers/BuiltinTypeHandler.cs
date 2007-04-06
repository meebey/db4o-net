using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
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
	public abstract class BuiltinTypeHandler : ITypeHandler4
	{
		internal readonly ObjectContainerBase _stream;

		public BuiltinTypeHandler(ObjectContainerBase stream)
		{
			_stream = stream;
		}

		public virtual object Coerce(IReflectClass claxx, object obj)
		{
			return CanHold(claxx) ? obj : No4.INSTANCE;
		}

		public void CopyValue(object a_from, object a_to)
		{
		}

		public virtual bool HasFixedLength()
		{
			return false;
		}

		public int LinkLength()
		{
			return Const4.INT_LENGTH + Const4.ID_LENGTH;
		}

		public virtual IReflectClass PrimitiveClassReflector()
		{
			return null;
		}

		public virtual bool ReadArray(object array, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			return false;
		}

		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return Read(mf, a_writer, true);
		}

		public virtual bool WriteArray(object array, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			return false;
		}

		public abstract bool IsGreater(object obj);

		public abstract IComparable4 PrepareComparison(object obj);

		public abstract int CompareTo(object obj);

		public abstract bool IsEqual(object obj);

		public abstract bool IsSmaller(object obj);

		public abstract object ComparableObject(Transaction trans, object indexEntry);

		public abstract object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader);

		public abstract void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object);

		public abstract void Defrag(MarshallerFamily mf, ReaderPair readers, bool redirect
			);

		public abstract object Current();

		public abstract void DefragIndexEntry(ReaderPair arg1);

		public abstract void CalculateLengths(Transaction arg1, ObjectHeaderAttributes arg2
			, bool arg3, object arg4, bool arg5);

		public abstract bool CanHold(IReflectClass arg1);

		public abstract void CascadeActivation(Transaction arg1, object arg2, int arg3, bool
			 arg4);

		public abstract IReflectClass ClassReflector();

		public abstract void DeleteEmbedded(MarshallerFamily arg1, StatefulBuffer arg2);

		public abstract ClassMetadata GetClassMetadata(ObjectContainerBase arg1);

		public abstract int GetID();

		public abstract int GetTypeID();

		public abstract object IndexEntryToObject(Transaction arg1, object arg2);

		public abstract bool IndexNullHandling();

		public abstract bool IsEqual(ITypeHandler4 arg1);

		public abstract TernaryBool IsSecondClass();

		public abstract void PrepareComparison(Transaction arg1, object arg2);

		public abstract object Read(MarshallerFamily arg1, StatefulBuffer arg2, bool arg3
			);

		public abstract ITypeHandler4 ReadArrayHandler(Transaction arg1, MarshallerFamily
			 arg2, Db4objects.Db4o.Internal.Buffer[] arg3);

		public abstract void ReadCandidates(MarshallerFamily arg1, Db4objects.Db4o.Internal.Buffer
			 arg2, QCandidates arg3);

		public abstract object ReadQuery(Transaction arg1, MarshallerFamily arg2, bool arg3
			, Db4objects.Db4o.Internal.Buffer arg4, bool arg5);

		public abstract QCandidate ReadSubCandidate(MarshallerFamily arg1, Db4objects.Db4o.Internal.Buffer
			 arg2, QCandidates arg3, bool arg4);

		public abstract bool SupportsIndex();

		public abstract object WriteNew(MarshallerFamily arg1, object arg2, bool arg3, StatefulBuffer
			 arg4, bool arg5, bool arg6);
	}
}
