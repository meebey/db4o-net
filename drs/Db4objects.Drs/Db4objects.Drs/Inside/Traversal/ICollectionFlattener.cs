/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside.Traversal
{
	public interface ICollectionFlattener : Db4objects.Drs.Inside.Traversal.ICollectionHandler
	{
		bool CanHandle(object obj);

		bool CanHandle(System.Type c);
	}
}
