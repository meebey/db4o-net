/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller0 : ArrayMarshaller
	{
		/// <exception cref="Db4oIOException"></exception>
		protected override BufferImpl PrepareIDReader(Transaction trans, BufferImpl reader
			)
		{
			return reader.ReadEmbeddedObject(trans);
		}
	}
}
