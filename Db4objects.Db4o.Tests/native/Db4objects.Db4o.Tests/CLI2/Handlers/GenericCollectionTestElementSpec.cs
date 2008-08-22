/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */
using System;
using Db4oUnit.Fixtures;

namespace Db4objects.Db4o.Tests.CLI2.Handlers
{
	class GenericCollectionTestElementSpec<T> : ILabeled//, ITypeAware
	{
		public readonly T[] _elements;

		public readonly T _notContained;

		public readonly T _largeElement;

		public GenericCollectionTestElementSpec(T[] elements, T notContained, T largeElement)
		{
			_elements = elements;
			_notContained = notContained;
			_largeElement = largeElement;
		}

		public virtual string Label()
		{
			return typeof(T).Name;
		}

		//public Type Type
		//{
		//    get
		//    {
		//        return typeof (T);
		//    }
		//}
	}

	//interface ITypeAware
	//{
	//    Type Type
	//    { 
	//        get;
	//    }
	//}
}
