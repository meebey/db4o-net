/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class UntypedMarshaller
	{
		internal MarshallerFamily _family;

		/// <exception cref="Db4oIOException"></exception>
		public abstract void DeleteEmbedded(StatefulBuffer reader);

		public abstract ITypeHandler4 ReadArrayHandler(Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			[] a_bytes);

		public abstract bool UseNormalClassRead();

		public abstract void Defrag(BufferPair readers);
	}
}
