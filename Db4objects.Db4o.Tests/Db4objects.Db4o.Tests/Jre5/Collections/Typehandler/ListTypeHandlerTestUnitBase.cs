/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Jre5.Collections.Typehandler;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
	public abstract class ListTypeHandlerTestUnitBase : AbstractDb4oTestCase, IOptOutDefragSolo
	{
		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(CreateItemFactory(
				).ListClass()), ListTypeHandler());
			config.ObjectClass(CreateItemFactory().ItemClass()).CascadeOnDelete(true);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			ItemFactory factory = CreateItemFactory();
			object item = factory.NewItem();
			IList list = ListFromItem(item);
			for (int eltIdx = 0; eltIdx < Elements().Length; eltIdx++)
			{
				list.Add(Elements()[eltIdx]);
			}
			list.Add(null);
			Store(item);
		}

		protected virtual ItemFactory CreateItemFactory()
		{
			return (ItemFactory)ListTypeHandlerTestVariables.ListImplementation.Value;
		}

		protected virtual ITypeHandler4 ListTypeHandler()
		{
			return (ITypeHandler4)ListTypeHandlerTestVariables.ListTypehander.Value;
		}

		protected virtual object[] Elements()
		{
			return ElementsSpec()._elements;
		}

		protected virtual object NotContained()
		{
			return ElementsSpec()._notContained;
		}

		protected virtual object LargeElement()
		{
			return ElementsSpec()._largeElement;
		}

		protected virtual Type ElementClass()
		{
			return ElementsSpec()._notContained.GetType();
		}

		protected virtual void AssertQueryResult(IQuery q, bool successful)
		{
			if (successful)
			{
				AssertSuccessfulQueryResult(q);
			}
			else
			{
				AssertEmptyQueryResult(q);
			}
		}

		protected virtual void AssertListContent(object item)
		{
			IList list = ListFromItem(item);
			Assert.AreEqual(CreateItemFactory().ListClass(), list.GetType());
			Assert.AreEqual(Elements().Length + 1, list.Count);
			for (int eltIdx = 0; eltIdx < Elements().Length; eltIdx++)
			{
				Assert.AreEqual(Elements()[eltIdx], list[eltIdx]);
			}
			Assert.IsNull(list[Elements().Length]);
		}

		protected virtual IList ListFromItem(object item)
		{
			try
			{
				return (IList)item.GetType().GetField(ItemFactory.ListFieldName).GetValue(item);
			}
			catch (Exception exc)
			{
				throw new Exception(string.Empty, exc);
			}
		}

		private void AssertEmptyQueryResult(IQuery q)
		{
			IObjectSet set = q.Execute();
			Assert.AreEqual(0, set.Size());
		}

		private void AssertSuccessfulQueryResult(IQuery q)
		{
			IObjectSet set = q.Execute();
			Assert.AreEqual(1, set.Size());
			object item = set.Next();
			AssertListContent(item);
		}

		private ListTypeHandlerTestElementsSpec ElementsSpec()
		{
			return (ListTypeHandlerTestElementsSpec)ListTypeHandlerTestVariables.ElementsSpec
				.Value;
		}
	}
}
