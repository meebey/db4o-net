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

		public virtual void TestFailingQuery()
		{
			AssertQuery(false, _helper.NotContained, false);
		}

		public virtual void TestSuccessfulContainsQuery()
		{
			AssertQuery(true, FirstElement(_helper.Elements), true);
		}

		public virtual void TestFailingContainsQuery()
		{
			AssertQuery(false, _helper.NotContained, true);
		}

		public virtual void TestCompareItems()
		{
			AssertCompareItems(FirstElement(_helper.Elements), true);
		}

		public virtual void TestFailingCompareItems()
		{
			AssertCompareItems(_helper.NotContained, false);
		}

		public virtual void TestDeletion()
		{
			AssertFirstClassElementCount(Count(_helper.Elements));
			object item = RetrieveOnlyInstance(_helper.ItemType);
			Db().Delete(item);
			Db().Purge();
			Db4oAssert.PersistedCount(0, _helper.ItemType);
			AssertFirstClassElementCount(0);
		}

		private static void AssertFirstClassElementCount(int expected)
		{
			if (!IsFirstClass(CollectionItemType()))
			{
				return;
			}
			Db4oAssert.PersistedCount(expected, CollectionItemType());
		}

		private static bool IsFirstClass(Type collectionItemType)
		{
			return typeof(GenericCollectionTypeHandlerTestVariables.FirstClassElement) == collectionItemType;
		}

		public void TestGreaterSmaller()
		{
			IQuery query = NewQuery(_helper.ItemType);
			query.Descend(GenericCollectionTestFactory.FieldName).Constrain(_helper.LargeElement).Smaller();

			AssertQueryResult(query.Execute(), 1);
		}

		public virtual void TestActivation()
		{
			object item = RetrieveOnlyInstance(_helper.ItemType);
			ICollection coll = CollectionFor(item);
			Assert.AreEqual(Count(_helper.Elements), coll.Count);
			object element = FirstElement(coll);

			if (Db().IsActive(element))
			{
				Db().Deactivate(item, int.MaxValue);
				Assert.IsFalse(Db().IsActive(element));
				Db().Activate(item, int.MaxValue);
				Assert.IsTrue(Db().IsActive(element));
			}
		}

		private void AssertQueryResult(IList result, int count)
		{
			Assert.AreEqual(count, result.Count);
			_helper.AssertCollection(result[0]);
		}

		private void AssertQuery(bool successful, object element, bool withContains)
		{
			IQuery q = NewQuery(_helper.ItemType);
			IConstraint constraint = q.Descend(GenericCollectionTestFactory.FieldName).Constrain(element);
			if (withContains)
			{
				constraint.Contains();
			}
			AssertQueryResult(q, successful);
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

		private static void AssertEmptyQueryResult(IQuery q)
		{
			IObjectSet set = q.Execute();
			Assert.AreEqual(0, set.Size());
		}

		private void AssertSuccessfulQueryResult(IQuery q)
		{
			IObjectSet set = q.Execute();
			Assert.AreEqual(1, set.Size());

			_helper.AssertCollection(set.Next());
		}

		protected void AssertCompareItems(object element, bool successful)
		{
			object item = _helper.NewItem(element);
			IQuery q = NewQuery();
			q.Constrain(item);
			AssertQueryResult(q, successful);
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
		}

		private static int Count(IEnumerable items)
		{
			int count = 0;
			for (IEnumerator iterator = items.GetEnumerator(); iterator.MoveNext(); count++) 
				;

			return count;
		}

		private static object FirstElement(IEnumerable items)
		{
			IEnumerator enumerator = items.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				throw new ArgumentException("Collection has zero items.", "items");
			}

			return enumerator.Current;
		}

		private static Type CollectionItemType()
		{
			return GenericCollectionTypeHandlerTestVariables.ElementSpec.Value.GetType().GetGenericArguments()[0];
		}

		private static ICollection CollectionFor(object item)
		{
			return (ICollection) item.GetType().GetField(GenericCollectionTestFactory.FieldName).GetValue(item);
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

		public object NewItem(object element)
		{
			object item = NewItem();
			ICollection<T> coll = CollectionFor(item);

			coll.Add((T) element);

			return item;
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
				return ElementSpec<T>()._largeElement;				
			}
		}

		public IEnumerable Elements
		{
			get
			{
				return ElementSpec<T>()._elements;
			}
		}

		public object NotContained
		{
			get
			{
				return ElementSpec<T>()._notContained;
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
		object NewItem(object element);

		Type ItemType
		{ 
			get;
		}

		object LargeElement { get; }
		IEnumerable Elements { get; }
		object NotContained { get; }
	}
}
