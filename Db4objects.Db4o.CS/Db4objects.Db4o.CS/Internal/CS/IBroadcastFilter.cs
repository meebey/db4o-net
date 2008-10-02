/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Internal.CS
{
	public interface IBroadcastFilter
	{
		bool Accept(IServerMessageDispatcher dispatcher);
	}
}
