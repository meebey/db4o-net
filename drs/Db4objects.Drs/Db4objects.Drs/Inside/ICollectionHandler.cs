/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside
{
	public interface ICollectionHandler : Db4objects.Drs.Inside.Traversal.ICollectionFlattener
	{
		object EmptyClone(object originalCollection, Db4objects.Db4o.Reflect.IReflectClass
			 originalCollectionClass);

		void CopyState(object original, object dest, Db4objects.Drs.Inside.ICounterpartFinder
			 finder);

		object CloneWithCounterparts(object original, Db4objects.Db4o.Reflect.IReflectClass
			 claxx, Db4objects.Drs.Inside.ICounterpartFinder elementCloner);
	}
}
