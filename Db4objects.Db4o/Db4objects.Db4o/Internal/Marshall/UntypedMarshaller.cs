/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class UntypedMarshaller
	{
		internal MarshallerFamily _family;

		public abstract ITypeHandler4 ReadArrayHandler(Transaction a_trans, BufferImpl[] 
			a_bytes);

		public abstract bool UseNormalClassRead();
	}
}
