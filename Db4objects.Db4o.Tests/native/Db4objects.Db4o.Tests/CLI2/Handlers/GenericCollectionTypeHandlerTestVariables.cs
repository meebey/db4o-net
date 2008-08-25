/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using Db4oUnit.Fixtures;

namespace Db4objects.Db4o.Tests.CLI2.Handlers
{

	public static class GenericCollectionTypeHandlerTestVariables
	{
		public static readonly FixtureVariable CollectionImplementation = new FixtureVariable("collections");

		public static readonly IFixtureProvider CollectionFixtureProvider = new SimpleFixtureProvider(
				CollectionImplementation,
				new object[]
					{
						new LinkedListItemFactory(),
						new ListItemFactory(),
                        new UntypedLinkedListItemFactory(),
					});

		public static readonly GenericCollectionTestElementSpec<string> StringElementSpec = new GenericCollectionTestElementSpec<string>(new string[] { "zero", "one" }, "two", "zzz");
		public static readonly GenericCollectionTestElementSpec<int> IntElementSpec = new GenericCollectionTestElementSpec<int>(new int[] { 0, 1 }, 2, int.MaxValue);
		public static readonly GenericCollectionTestElementSpec<int?> NullableIntElementSpec = new GenericCollectionTestElementSpec<int?>(new int?[] { 0, null }, 2, int.MaxValue);

		public static readonly FixtureVariable ElementSpec = new FixtureVariable("elements");
		public static readonly IFixtureProvider ElementsFixtureProvider = new SimpleFixtureProvider(
				ElementSpec,
				new object[]
				{
					StringElementSpec,
					IntElementSpec,
					NullableIntElementSpec,
					new GenericCollectionTestElementSpec<FirstClassElement>(new FirstClassElement[] { new FirstClassElement(0), new FirstClassElement(1) }, new FirstClassElement(2), null),
				}
			);

		public class FirstClassElement
		{
			public int _id;

			public FirstClassElement(int id)
			{
				_id = id;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				FirstClassElement other = (FirstClassElement) obj;
				return _id == other._id;
			}

			public override int GetHashCode()
			{
				return _id;
			}

			public override string ToString()
			{
				return "FCE#" + _id;
			}
		}

		private class ListItemFactory : GenericCollectionTestFactory
		{
			public override object NewItem<T>()
			{
				return new Item<T>();
			}

			public override Type ContainerType()
			{
				return typeof(List<>);
			}

			public override string Label()
			{
				return "List<>";
			}

			private class Item<T>
			{
				public List<T> _coll = new List<T>();
			}
		}

		private class LinkedListItemFactory : GenericCollectionTestFactory
		{
			public override object NewItem<T>()
			{
				return new Item<T>();
			}

			public override Type ContainerType()
			{
				return typeof(LinkedList<>);
			}

			public override string Label()
			{
				return "LinkedList<>";
			}

			private class Item<T>
			{
				public LinkedList<T> _coll = new LinkedList<T>();
			}
		}

        private class UntypedLinkedListItemFactory : GenericCollectionTestFactory
        {
            public override object NewItem<T>()
            {
                return new Item<T>();
            }

            public override Type ContainerType()
            {
                return typeof(LinkedList<>);
            }

            public override string Label()
            {
                return "LinkedList<>(object)";
            }

            private class Item<T>
            {
                public object _coll = new LinkedList<T>();
            }
        }
    }
}