/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class PrimitiveMarshaller
	{
		public MarshallerFamily _family;

		public abstract bool UseNormalClassRead();

		public abstract DateTime ReadDate(BufferImpl bytes);

		public abstract object ReadShort(BufferImpl buffer);

		public abstract object ReadInteger(BufferImpl buffer);

		public abstract object ReadFloat(BufferImpl buffer);

		public abstract object ReadDouble(BufferImpl buffer);

		public abstract object ReadLong(BufferImpl buffer);
	}
}
