/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller1 : ArrayMarshaller
	{
		protected override ByteArrayBuffer PrepareIDReader(Transaction trans, ByteArrayBuffer
			 reader)
		{
			reader._offset = reader.ReadInt();
			return reader;
		}
	}
}
