/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class ObjectReferenceContext : ObjectHeaderContext, IFieldListInfo, IMarshallingInfo
		, IHandlerVersionContext
	{
		protected readonly Db4objects.Db4o.Internal.ObjectReference _reference;

		public ObjectReferenceContext(Transaction transaction, IReadBuffer buffer, ObjectHeader
			 objectHeader, Db4objects.Db4o.Internal.ObjectReference reference) : base(transaction
			, buffer, objectHeader)
		{
			_reference = reference;
		}

		protected virtual Db4objects.Db4o.Internal.ByteArrayBuffer ByteArrayBuffer()
		{
			return (Db4objects.Db4o.Internal.ByteArrayBuffer)Buffer();
		}

		public virtual Db4objects.Db4o.Internal.StatefulBuffer StatefulBuffer()
		{
			Db4objects.Db4o.Internal.StatefulBuffer statefulBuffer = new Db4objects.Db4o.Internal.StatefulBuffer
				(Transaction(), ByteArrayBuffer().Length());
			statefulBuffer.SetID(ObjectID());
			statefulBuffer.SetInstantiationDepth(ActivationDepth());
			ByteArrayBuffer().CopyTo(statefulBuffer, 0, 0, ByteArrayBuffer().Length());
			statefulBuffer.Seek(ByteArrayBuffer().Offset());
			return statefulBuffer;
		}

		public virtual int ObjectID()
		{
			return _reference.GetID();
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _reference.ClassMetadata();
		}

		public virtual Db4objects.Db4o.Internal.ObjectReference ObjectReference()
		{
			return _reference;
		}
	}
}
