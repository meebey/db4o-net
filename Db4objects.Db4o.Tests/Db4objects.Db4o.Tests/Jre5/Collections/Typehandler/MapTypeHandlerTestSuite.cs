/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Jre5.Collections.Typehandler;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
	/// <decaf.ignore></decaf.ignore>
	public class MapTypeHandlerTestSuite : FixtureBasedTestSuite, IDb4oTestCase
	{
		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[] { new Db4oFixtureProvider(), MapTypeHandlerTestVariables
				.MapFixtureProvider, MapTypeHandlerTestVariables.MapKeysProvider, MapTypeHandlerTestVariables
				.MapValuesProvider, MapTypeHandlerTestVariables.TypehandlerFixtureProvider };
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(MapTypeHandlerTestSuite.MapTypeHandlerUnitTestCase) };
		}

		public class MapTypeHandlerUnitTestCase : TypeHandlerUnitTest
		{
			protected override void FillItem(object item)
			{
				FillMapItem(item);
			}

			protected override void AssertContent(object item)
			{
				AssertMapContent(item);
			}

			protected override AbstractItemFactory ItemFactory()
			{
				return (AbstractItemFactory)MapTypeHandlerTestVariables.MapImplementation.Value;
			}

			protected override ITypeHandler4 TypeHandler()
			{
				return (ITypeHandler4)MapTypeHandlerTestVariables.MapTypehander.Value;
			}

			protected override ListTypeHandlerTestElementsSpec ElementsSpec()
			{
				return (ListTypeHandlerTestElementsSpec)MapTypeHandlerTestVariables.MapKeysSpec.Value;
			}

			protected override void AssertCompareItems(object element, bool successful)
			{
				IQuery q = NewQuery();
				object item = ItemFactory().NewItem();
				IDictionary map = MapFromItem(item);
				map.Add(element, Values()[0]);
				q.Constrain(item);
				AssertQueryResult(q, successful);
			}

			//TODO: remove when COR-1311 solved 
			/// <exception cref="Exception"></exception>
			public override void TestSuccessfulQuery()
			{
				if (Elements()[0] is ListTypeHandlerTestVariables.FirstClassElement)
				{
					return;
				}
				base.TestSuccessfulQuery();
			}

			//TODO: remove when COR-1311 solved 
			/// <exception cref="Exception"></exception>
			public override void TestFailingQuery()
			{
				if (Elements()[0] is ListTypeHandlerTestVariables.FirstClassElement)
				{
					return;
				}
				base.TestFailingQuery();
			}

			//TODO: remove when COR-1311 solved 
			/// <exception cref="Exception"></exception>
			public override void TestFailingContainsQuery()
			{
				if (Elements()[0] is ListTypeHandlerTestVariables.FirstClassElement)
				{
					return;
				}
				base.TestFailingContainsQuery();
			}

			//TODO: remove when COR-1311 solved 
			/// <exception cref="Exception"></exception>
			public override void TestFailingCompareItems()
			{
				if (Elements()[0] is ListTypeHandlerTestVariables.FirstClassElement)
				{
					return;
				}
				base.TestFailingCompareItems();
			}

			//TODO: remove when COR-1311 solved 
			/// <exception cref="Exception"></exception>
			public override void TestCompareItems()
			{
				if (Elements()[0] is ListTypeHandlerTestVariables.FirstClassElement)
				{
					return;
				}
				base.TestCompareItems();
			}

			//TODO: remove when COR-1311 solved 
			/// <exception cref="Exception"></exception>
			public override void TestSuccessfulContainsQuery()
			{
				if (Elements()[0] is ListTypeHandlerTestVariables.FirstClassElement)
				{
					return;
				}
				base.TestSuccessfulContainsQuery();
			}
		}
	}
}
