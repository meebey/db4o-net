/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	public interface ITimeoutBlockingQueue4 : IPausableBlockingQueue4
	{
		void Check();

		void Reset();
	}
}
