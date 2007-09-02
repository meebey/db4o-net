/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <summary>
	/// Common base class for StringHandler and ArrayHandler:
	/// The common pattern for both is that a slot  is one indirection in the database file to this.
	/// </summary>
	/// <remarks>
	/// Common base class for StringHandler and ArrayHandler:
	/// The common pattern for both is that a slot  is one indirection in the database file to this.
	/// </remarks>
	/// <exclude></exclude>
	public abstract class BuiltinTypeHandler : ITypeHandler4
	{
		internal readonly ObjectContainerBase _stream;

		public BuiltinTypeHandler(ObjectContainerBase stream)
		{
			_stream = stream;
		}

		public bool HasFixedLength()
		{
			return false;
		}

		public int LinkLength()
		{
			return Const4.INT_LENGTH + Const4.ID_LENGTH;
		}

		public abstract IComparable4 PrepareComparison(object obj);

		public abstract int CompareTo(object obj);

		public abstract void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect
			);

		public abstract void CalculateLengths(Transaction arg1, ObjectHeaderAttributes arg2
			, bool arg3, object arg4, bool arg5);

		public abstract void CascadeActivation(Transaction arg1, object arg2, int arg3, bool
			 arg4);

		public abstract IReflectClass ClassReflector();

		public abstract void DeleteEmbedded(MarshallerFamily arg1, StatefulBuffer arg2);

		public abstract int GetID();

		public abstract object Read(MarshallerFamily arg1, StatefulBuffer arg2, bool arg3
			);

		public abstract object Read(IReadContext arg1);

		public abstract object ReadQuery(Transaction arg1, MarshallerFamily arg2, bool arg3
			, Db4objects.Db4o.Internal.Buffer arg4, bool arg5);

		public abstract QCandidate ReadSubCandidate(MarshallerFamily arg1, Db4objects.Db4o.Internal.Buffer
			 arg2, QCandidates arg3, bool arg4);

		public abstract object Write(MarshallerFamily arg1, object arg2, bool arg3, StatefulBuffer
			 arg4, bool arg5, bool arg6);

		public abstract void Write(IWriteContext arg1, object arg2);
	}
}
