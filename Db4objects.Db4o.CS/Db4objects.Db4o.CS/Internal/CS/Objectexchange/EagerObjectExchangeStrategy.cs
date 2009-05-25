/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Objectexchange;

namespace Db4objects.Db4o.Internal.CS.Objectexchange
{
	public class EagerObjectExchangeStrategy : IObjectExchangeStrategy
	{
		private ObjectExchangeConfiguration _config;

		public EagerObjectExchangeStrategy(ObjectExchangeConfiguration config)
		{
			_config = config;
		}

		public virtual ByteArrayBuffer Marshall(LocalTransaction transaction, IIntIterator4
			 ids, int maxCount)
		{
			return new EagerObjectWriter(_config, transaction).Write(ids, maxCount);
		}

		public virtual IFixedSizeIntIterator4 Unmarshall(ClientTransaction transaction, ByteArrayBuffer
			 reader)
		{
			return new EagerObjectReader(transaction, reader).Iterator();
		}
	}
}
