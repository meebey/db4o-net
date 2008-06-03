/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Jre5.Collections.Typehandler;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
	public class ListTypeHandlerTestSuite : FixtureBasedTestSuite, IDb4oTestCase
	{
		public override IFixtureProvider[] FixtureProviders()
		{
			ListTypeHandlerTestElementsSpec[] elementSpecs = new ListTypeHandlerTestElementsSpec
				[] { ListTypeHandlerTestVariables.StringElementsSpec, ListTypeHandlerTestVariables
				.IntElementsSpec, ListTypeHandlerTestVariables.ObjectElementsSpec };
			return new IFixtureProvider[] { new Db4oFixtureProvider(), ListTypeHandlerTestVariables
				.ListFixtureProvider, new SimpleFixtureProvider(ListTypeHandlerTestVariables.ElementsSpec
				, elementSpecs), ListTypeHandlerTestVariables.TypehandlerFixtureProvider };
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(ListTypeHandlerTestSuite.ListTypeHandlerTestUnit) };
		}

		public class ListTypeHandlerTestUnit : ListTypeHandlerTestUnitBase
		{
			public virtual void TestRetrieveInstance()
			{
				Type itemClass = CreateItemFactory().ItemClass();
				object item = RetrieveOnlyInstance(itemClass);
				AssertListContent(item);
			}

			/// <exception cref="Exception"></exception>
			public virtual void TestSuccessfulQuery()
			{
				AssertQuery(true, Elements()[0], false);
			}

			/// <exception cref="Exception"></exception>
			public virtual void TestFailingQuery()
			{
				AssertQuery(false, NotContained(), false);
			}

			/// <exception cref="Exception"></exception>
			public virtual void TestSuccessfulContainsQuery()
			{
				AssertQuery(true, Elements()[0], true);
			}

			/// <exception cref="Exception"></exception>
			public virtual void TestFailingContainsQuery()
			{
				AssertQuery(false, NotContained(), true);
			}

			/// <exception cref="Exception"></exception>
			public virtual void TestCompareItems()
			{
				AssertCompareItems(Elements()[0], true);
			}

			/// <exception cref="Exception"></exception>
			public virtual void TestFailingCompareItems()
			{
				AssertCompareItems(NotContained(), false);
			}

			/// <exception cref="Exception"></exception>
			public virtual void TestDeletion()
			{
				AssertFirstClassElementCount(Elements().Length);
				object item = RetrieveOnlyInstance(CreateItemFactory().ItemClass());
				Db().Delete(item);
				Db().Purge();
				Db4oAssert.PersistedCount(0, CreateItemFactory().ItemClass());
				AssertFirstClassElementCount(0);
			}

			private void AssertFirstClassElementCount(int expected)
			{
				if (!IsFirstClass(ElementClass()))
				{
					return;
				}
				Db4oAssert.PersistedCount(expected, ElementClass());
			}

			private bool IsFirstClass(Type elementClass)
			{
				return typeof(ListTypeHandlerTestVariables.FirstClassElement) == elementClass;
			}

			private void AssertCompareItems(object element, bool successful)
			{
				IQuery q = NewQuery();
				object item = CreateItemFactory().NewItem();
				IList list = ListFromItem(item);
				list.Add(element);
				q.Constrain(item);
				AssertQueryResult(q, successful);
			}

			private void AssertQuery(bool successful, object element, bool withContains)
			{
				IQuery q = NewQuery(CreateItemFactory().ItemClass());
				IConstraint constraint = q.Descend(ItemFactory.ListFieldName).Constrain(element);
				if (withContains)
				{
					constraint.Contains();
				}
				AssertQueryResult(q, successful);
			}
		}
	}
}
