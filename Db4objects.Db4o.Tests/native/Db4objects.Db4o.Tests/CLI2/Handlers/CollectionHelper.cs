using System;
using System.Collections;
using System.Collections.Generic;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Handlers
{
	internal class CollectionHelper<T> : ICollectionHelper
	{
		public void AssertCollection(object item)
		{
			System.Collections.IEnumerable actual = CollectionFor(item);
			System.Collections.IEnumerable expected = ElementSpec<T>()._elements;

			Iterator4Assert.AreEqual(expected.GetEnumerator(), actual.GetEnumerator());
		}

		public object NewItem(object element)
		{
			object item = NewItem();
			ICollection<T> coll = CollectionFor(item);

			coll.Add((T)element);

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
					_itemType = ItemFactory().NewItem<T>().GetType();
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
			foreach (T item in elements)
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
		System.Collections.IEnumerable Elements { get; }
		object NotContained { get; }
	}
}
