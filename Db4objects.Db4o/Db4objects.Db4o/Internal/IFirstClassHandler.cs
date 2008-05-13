/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IFirstClassHandler : ICascadingTypeHandler
	{
		/// <exception cref="Db4oIOException"></exception>
		void ReadCandidates(QueryingReadContext context);

		ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily mf, ByteArrayBuffer
			[] a_bytes);
	}
}
