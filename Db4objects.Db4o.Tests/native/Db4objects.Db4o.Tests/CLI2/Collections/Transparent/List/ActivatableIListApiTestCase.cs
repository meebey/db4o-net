/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */

using System;
using System.Collections.Generic;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Collections.Transparent.List
{
	internal class ActivatableIListApiTestCase : AbstractActivatableCollectionApiTestCase
	{
		public void TestCorrectContent()
		{
			IteratorAssert.AreEqual(NewPlainCollection().GetEnumerator(), SingleCollection().GetEnumerator());
		}

		public void TestCollectionIsNotActivated()
		{
			Assert.IsFalse(Db().IsActive(SingleCollection()));
		}
	
		public void TestIndexOf()
		{
			const int itemIndex = 2;
			Assert.AreEqual(itemIndex, SingleCollection().IndexOf(NewElement(itemIndex)));
		}

		public void TestIndexerGetter()
		{
			const int indexToTest = 1;
			Assert.AreEqual(NewElement(indexToTest), SingleCollection()[indexToTest]);
		}

		public void TestCopyTo()
		{
			AssertCopy(delegate(ICollectionElement[] elements)
			{
				SingleCollection().CopyTo(elements, 0);
			});
		}

		public void TestIndexerSetter()
		{
			AssertListOperation(delegate(IList<ICollectionElement> list)
			                    	{
										const int indexToTest = 1;
			                    		list[indexToTest] = new Element("one-and-half");
			                    	});
		}

		public void TestInsert()
		{
			AssertListOperation(	delegate(IList<ICollectionElement> list)
			                    	{
										const int insertionIndex = 2;
										const string newItemName = "two-and-half";

										list.Insert(insertionIndex, new Element(newItemName));
			                    	});
		}

		public void TestRemoveAt()
		{
			AssertListOperation(delegate(IList<ICollectionElement> list)
			{
				list.RemoveAt(0);                    		
			});
		}

		public void TestRepeatedAdd()
		{
			ICollectionElement four = new Element("four");
			SingleCollection().Add(four);
			Db().Purge();
			
			ICollectionElement five = new Element("five");
			SingleCollection().Add(five);
			Reopen();

			IList<ICollectionElement> retrieved = SingleCollection();
			Assert.IsTrue(retrieved.Contains(four));
			Assert.IsTrue(retrieved.Contains(five));
		}
	}
}
