using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Fixtures;

namespace Db4objects.Db4o.Tests.CLI2.Handlers
{
	class GenericCollectionTypeHandlerTestSuite : FixtureBasedTestSuite, IDb4oTestCase
	{
		public override Type[] TestUnits()
		{
			return new Type[]
			       	{
			       		typeof(GenericCollectionTypeHandlerTestUnit),
			       	};
		}

		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[]
			       	{
			       		new Db4oFixtureProvider(),
						GenericCollectionTypeHandlerTestVariables.CollectionFixtureProvider,
						GenericCollectionTypeHandlerTestVariables.ElementsFixtureProvider,
			       	};
		}
	}

	class GenericCollectionTypeHandlerTestUnit : AbstractDb4oTestCase
	{
		public void Test()
		{
			object item = RetrieveOnlyInstance(_helper.ItemType);
			_helper.AssertCollection(item);
		}

		public void TestGreaterSmaller()
		{
			IQuery query = NewQuery(_helper.ItemType);
			query.Descend(GenericCollectionTestFactory.FieldName).Constrain(_helper.LargeElement).Smaller();

			//AssertQueryResult(query.Execute(), 1);
		}

		private void AssertQueryResult(IList result, int count)
		{
			Assert.AreEqual(count, result.Count);
			_helper.AssertCollection(result[0]);
		}

		protected override void Store()
		{
			_helper = NewCollectionHelper();
			Store(_helper.NewItem());
		}

		private static ICollectionHelper NewCollectionHelper()
		{
			Type type = GenericCollectionTypeHandlerTestVariables.ElementSpec.Value.GetType().GetGenericArguments()[0];
			return (ICollectionHelper)Activator.CreateInstance(typeof(CollectionHelper<>).MakeGenericType(type));
			
			//ITypeAware typeAware = (ITypeAware) GenericCollectionTypeHandlerTestVariables.ElementSpec.Value;
			//return (ICollectionHelper) Activator.CreateInstance(typeof (CollectionHelper<>).MakeGenericType(typeAware.Type));
		}

		private ICollectionHelper _helper;
	}

	class CollectionHelper<T> : ICollectionHelper
	{
		public void AssertCollection(object item)
		{
			IEnumerable actual = CollectionFor(item);
			IEnumerable expected = ElementSpec<T>()._elements;

			Iterator4Assert.AreEqual(expected.GetEnumerator(), actual.GetEnumerator());
		}

		public object NewItem()
		{
			object item = ItemFactory().NewItem<T>();
			Fill(CollectionFor(item), ElementSpec<T>()._elements);

			_itemType = item.GetType();

			return item;
		}

		public Type ItemType
		{
			get
			{
				if (_itemType == null)
				{
					throw new NullReferenceException("NewItem() must be called first.");
				}

				return _itemType;
			}
		}

		public object LargeElement
		{
			get
			{
				return ((GenericCollectionTestElementSpec<T>) GenericCollectionTypeHandlerTestVariables.ElementSpec.Value)._largeElement;				
			}
		}

		private static void Fill(ICollection<T> collection, IEnumerable<T> elements)
		{
			foreach(T item in elements)
			{
				collection.Add(item);
			}
		}

		private static ICollection<T> CollectionFor(object item)
		{
			return (ICollection<T>)item.GetType().GetField(GenericCollectionTestFactory.FieldName).GetValue(item);
		}

		private static GenericCollectionTestFactory ItemFactory()
		{
			return (GenericCollectionTestFactory)GenericCollectionTypeHandlerTestVariables.CollectionImplementation.Value;
		}

		private static GenericCollectionTestElementSpec<T> ElementSpec<T>()
		{
			return (GenericCollectionTestElementSpec<T>)GenericCollectionTypeHandlerTestVariables.ElementSpec.Value;
		}

		private Type _itemType;
	}

	internal interface ICollectionHelper
	{
		void AssertCollection(object item);

		object NewItem();

		Type ItemType
		{ 
			get;
		}

		object LargeElement { get; }
	}
}
