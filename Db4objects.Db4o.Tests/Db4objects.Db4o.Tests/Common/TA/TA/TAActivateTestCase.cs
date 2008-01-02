/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	public class TAActivateTestCase : TAItemTestCaseBase
	{
		public static void Main(string[] args)
		{
			new TAActivateTestCase().RunAll();
		}

		private const int ITEM_DEPTH = 10;

		/// <exception cref="Exception"></exception>
		protected override void AssertItemValue(object obj)
		{
			TAActivateTestCase.TAItem taItem = (TAActivateTestCase.TAItem)obj;
			for (int i = 0; i < ITEM_DEPTH - 1; i++)
			{
				Assert.AreEqual("TAItem " + (ITEM_DEPTH - i), taItem.GetName());
				Assert.AreEqual(ITEM_DEPTH - i, taItem.GetValue());
				Assert.IsNotNull(taItem.Next());
				taItem = taItem.Next();
			}
			Assert.AreEqual("TAItem 1", taItem.GetName());
			Assert.AreEqual(1, taItem.GetValue());
			Assert.IsNull(taItem.Next());
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertRetrievedItem(object obj)
		{
			TAActivateTestCase.TAItem taItem = (TAActivateTestCase.TAItem)obj;
			AssertNullItem(taItem);
			Db().Activate(taItem, 0);
			AssertNullItem(taItem);
			Db().Activate(taItem, 1);
			AssertActivatedItem(taItem, 0, 1);
			Db().Activate(taItem, 5);
			AssertActivatedItem(taItem, 0, 5);
			Db().Activate(taItem, ITEM_DEPTH + 100);
			AssertActivatedItem(taItem, 0, ITEM_DEPTH);
		}

		private void AssertActivatedItem(TAActivateTestCase.TAItem item, int from, int depth
			)
		{
			if (depth > ITEM_DEPTH)
			{
				throw new ArgumentException("depth should not be greater than ITEM_DEPTH.");
			}
			TAActivateTestCase.TAItem next = item;
			for (int i = from; i < depth; i++)
			{
				Assert.AreEqual("TAItem " + (ITEM_DEPTH - i), next._name);
				Assert.AreEqual(ITEM_DEPTH - i, next._value);
				if (i < ITEM_DEPTH - 1)
				{
					Assert.IsNotNull(next._next);
				}
				next = next._next;
			}
			if (depth < ITEM_DEPTH)
			{
				AssertNullItem(next);
			}
		}

		private void AssertNullItem(TAActivateTestCase.TAItem taItem)
		{
			Assert.IsNull(taItem._name);
			Assert.IsNull(taItem._next);
			Assert.AreEqual(0, taItem._value);
		}

		public override object RetrieveOnlyInstance(Type clazz)
		{
			IQuery q = Db().Query();
			q.Constrain(clazz);
			q.Descend("_isRoot").Constrain(true);
			return q.Execute().Next();
		}

		/// <exception cref="Exception"></exception>
		protected override object CreateItem()
		{
			TAActivateTestCase.TAItem taItem = TAActivateTestCase.TAItem.NewTAItem(ITEM_DEPTH
				);
			taItem._isRoot = true;
			return taItem;
		}

		public class TAItem : ActivatableImpl
		{
			public string _name;

			public int _value;

			public TAActivateTestCase.TAItem _next;

			public bool _isRoot;

			public static TAActivateTestCase.TAItem NewTAItem(int depth)
			{
				if (depth == 0)
				{
					return null;
				}
				TAActivateTestCase.TAItem root = new TAActivateTestCase.TAItem();
				root._name = "TAItem " + depth;
				root._value = depth;
				root._next = NewTAItem(depth - 1);
				return root;
			}

			public virtual string GetName()
			{
				Activate(ActivationPurpose.READ);
				return _name;
			}

			public virtual int GetValue()
			{
				Activate(ActivationPurpose.READ);
				return _value;
			}

			public virtual TAActivateTestCase.TAItem Next()
			{
				Activate(ActivationPurpose.READ);
				return _next;
			}
		}
	}
}
