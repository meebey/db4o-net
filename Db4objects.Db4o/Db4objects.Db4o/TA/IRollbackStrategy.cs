/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;

namespace Db4objects.Db4o.TA
{
	public interface IRollbackStrategy
	{
		void Rollback(IObjectContainer container, object obj);
	}
}
