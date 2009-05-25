/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Internal.CS.Objectexchange
{
	public interface IObjectExchangeStrategy
	{
		ByteArrayBuffer Marshall(LocalTransaction transaction, IIntIterator4 ids, int maxCount
			);

		IFixedSizeIntIterator4 Unmarshall(ClientTransaction transaction, ByteArrayBuffer 
			reader);
	}
}
