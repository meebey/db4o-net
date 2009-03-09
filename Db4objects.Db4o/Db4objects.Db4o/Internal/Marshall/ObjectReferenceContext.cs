/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class ObjectReferenceContext : ObjectHeaderContext
	{
		protected readonly Db4objects.Db4o.Internal.ObjectReference _reference;

		public ObjectReferenceContext(Transaction transaction, IReadBuffer buffer, ObjectHeader
			 objectHeader, Db4objects.Db4o.Internal.ObjectReference reference) : base(transaction
			, buffer, objectHeader)
		{
			_reference = reference;
		}

		public virtual int ObjectID()
		{
			return _reference.GetID();
		}

		public override Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _reference.ClassMetadata();
		}

		public virtual Db4objects.Db4o.Internal.ObjectReference ObjectReference()
		{
			return _reference;
		}

		protected virtual Db4objects.Db4o.Internal.ByteArrayBuffer ByteArrayBuffer()
		{
			return (Db4objects.Db4o.Internal.ByteArrayBuffer)Buffer();
		}
	}
}
