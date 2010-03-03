/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */
using System;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Collections.Transparent
{
	partial class AbstractActivatableCollectionApiTestCase
	{
		protected void AssertCopy(Action<ICollectionElement[]> copyAction)
		{
			ICollectionElement[] elements = new ICollectionElement[NewPlainCollection().Count];

			copyAction(elements);

			foreach (string name in Names)
			{
				Assert.IsGreaterOrEqual(0, Array.IndexOf<ICollectionElement>(elements, new Element(name)));
				Assert.IsGreaterOrEqual(0, Array.IndexOf<ICollectionElement>(elements, new ActivatableElement(name)));
			}
		}
	}
}
