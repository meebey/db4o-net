/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside.Traversal
{
	public interface ICollectionHandler
	{
		bool CanHandle(Db4objects.Db4o.Reflect.IReflectClass claxx);

		System.Collections.IEnumerator IteratorFor(object collection);
	}
}
