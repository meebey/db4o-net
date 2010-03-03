/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using Db4objects.Db4o.Collections;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Collections.Transparent
{
	public partial class AbstractActivatableCollectionApiTestCase : AbstractDb4oTestCase
	{
		protected readonly IList<string> Names = new List<string>(new string[] {"one", "two", "three", "four"});

		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentPersistenceSupport());
		}

		protected override void Store()
		{
			ActivatableList<ICollectionElement> collection = NewActivatableCollection();
			CollectionHolder<IList<ICollectionElement>> item = new CollectionHolder<IList<ICollectionElement>>(collection);
			Store(item);
		}

		public void TestAdd()
		{
			AssertListOperation(delegate(IList<ICollectionElement> list)
			{
				list.Add(new Element("five"));
			});
		}

		public void TestClear()
		{
			SingleCollection().Clear();
			Reopen();
			IteratorAssert.SameContent(new List<Element>(), SingleCollection());
		}

		public void TestContains()
		{
			Assert.IsTrue(SingleCollection().Contains(new Element("one")));
			Assert.IsFalse(SingleCollection().Contains(new Element("five")));
		}

		public void TestCopyTo()
		{
			IList<ICollectionElement> plainCollection = NewPlainCollection();
			ICollectionElement[] target = new ICollectionElement[plainCollection.Count];

			IList<ICollectionElement> list = SingleCollection();
			list.CopyTo(target, 0);
			Assert.IsTrue(Db().IsActive(list));
			IteratorAssert.AreEqual(list.GetEnumerator(), target.GetEnumerator());
			IteratorAssert.AreEqual(plainCollection.GetEnumerator(), target.GetEnumerator());
		}

		public void TestIsReadOnly()
		{
			Assert.IsFalse(SingleCollection().IsReadOnly);
		}

		public void TestRemove()
		{
			AssertListOperation(delegate(IList<ICollectionElement> list)
			{
				list.Remove(new Element("one"));
			});
		}

		public void TestCount()
		{
			Assert.AreEqual(NewPlainCollection().Count, SingleCollection().Count);
		}

		protected ICollectionElement NewElement(int index)
		{
			return new Element(Names[index]);
		}

		protected IList<ICollectionElement> SingleCollection()
		{
			CollectionHolder<IList<ICollectionElement>> holder = (CollectionHolder<IList<ICollectionElement>>) RetrieveOnlyInstance(typeof(CollectionHolder<IList<ICollectionElement>>));
			return holder.Collection;
		}

		private ActivatableList<ICollectionElement> NewActivatableCollection()
		{
			ActivatableList<ICollectionElement> activatableList = new ActivatableList<ICollectionElement>(NewPlainCollection());
			return activatableList;
		}

		protected List<ICollectionElement> NewPlainCollection()
		{
			List<ICollectionElement> plainList = new List<ICollectionElement>();
			foreach (string name in Names)
			{
				plainList.Add(new Element(name));
			}

			foreach (string name in Names)
			{
				plainList.Add(new ActivatableElement(name));
			}

			return plainList;
		}

		protected int LastIndex()
		{
			return Names.Count * 2 - 1;
		}

		protected void AssertListOperation(Action<IList<ICollectionElement>> action)
		{
			action(SingleCollection());
			Reopen();

			List<ICollectionElement> expected = NewPlainCollection();
			action(expected);

			IteratorAssert.AreEqual(expected.GetEnumerator(), SingleCollection().GetEnumerator());
		}
	
	}
}
