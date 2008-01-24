/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.TA
{
	public interface IRollbackStrategy
	{
		void Rollback(IObjectContainer container, IObjectInfo o);
	}
}
