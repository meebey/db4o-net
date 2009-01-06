/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Typehandlers;
using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{

    class EnumTypeHandlerTestCase : AbstractDb4oTestCase
    {
        enum EnumAsByte : byte
        {
            First,
            Second,
            Third
        }

        enum EnumAsInteger
        {
            First = -42,
            Second,
        }

        enum EnumAsUInt : uint
        {
            First = 42,
            Second,
        }

        enum EnumAsLong : long
        {
            First = -42,
            Second = 37,
            Third
        }

        enum EnumAsULong : ulong
        {
            First = 42,
            Second = 37
        }

        [Flags]
        enum ByteFlags : byte
        {
            First = 0x01,
            Second = 0x02,
            Third = 0x04
        }

        enum EnumAsSByte : sbyte
        {
            First = 2,
            Second
        }

        class Item
        {
			public readonly EnumAsByte _asByte;
            public readonly EnumAsInteger _asInteger;
            public readonly EnumAsLong _asLong;

            public Item(EnumAsByte asByte, EnumAsInteger asInteger, EnumAsLong asLong)
            {
                _asByte = asByte;
                _asInteger = asInteger;
                _asLong = asLong;
            }

            public override bool Equals(object obj)
            {
                Item rhs = (Item)obj;
                if (rhs == null) return false;

                if (rhs.GetType() != GetType()) return false;

                return _asByte == rhs._asByte && _asInteger == rhs._asInteger && _asLong == rhs._asLong;
            }

            public override string ToString()
            {
                return _asByte + "/" + _asInteger + "/" + _asLong;
            }
        }

        private static readonly Item[] _items = new Item[]
        {
			new Item(EnumAsByte.First, EnumAsInteger.Second, EnumAsLong.Third),
			new Item(EnumAsByte.Second, EnumAsInteger.Second, EnumAsLong.Second),
			new Item(EnumAsByte.Third, EnumAsInteger.Second, EnumAsLong.First),
			new Item((EnumAsByte)99, (EnumAsInteger) 98, (EnumAsLong) 97),
        };

        protected override void Configure(IConfiguration config)
        {
			base.Configure(config);
			config.RegisterTypeHandler(new EnumTypeHandlerPredicate(), new EnumTypeHandler());
        }

        protected override void Store()
        {
            foreach (Item item in _items)
            {
                Store(item);
            }
        }

		public void TestEnumsAreNotStoredAsObjects()
		{
			Assert.AreEqual(0, Db().Query<EnumAsByte>().Count);
		}

		public void TestNativeQuery()
        {
            AssertItem(EnumAsByte.Second, AsByteFinder(), AsByteSelectorFor);
            AssertItem(EnumAsByte.Third, AsByteFinder(), AsByteSelectorFor);
        }

        public void TestSODAQuery()
        {
            AssertAsByte();
            AssertAsLong();
        }

        public void TestInvalidEnumValue()
        {
            AssertItem((EnumAsByte)99, AsByteFinder());
        }

        public void TestRetrieveAll()
        {
            AssertCanRetrieveAll();
        }

        public void TestQueryByExample()
        {
            Item item = new Item(EnumAsByte.Second, 0, 0);
            IObjectSet result = Db().QueryByExample(item);
            Assert.AreEqual(1, result.Count);
            Item itemFound = (Item) result[0];
            Assert.AreEqual(EnumAsByte.Second, itemFound._asByte);
            Assert.AreEqual(EnumAsInteger.Second, itemFound._asInteger);
            Assert.AreEqual(EnumAsLong.Second, itemFound._asLong);
        }

        public void TestQueryByExampleAll()
        {
            // Just like in primitives, if enum 0 is used, the 
            // constraint is ignored.
            Item item = new Item(EnumAsByte.First, 0, 0);
            IObjectSet result = Db().QueryByExample(item);
            Assert.AreEqual(4, result.Count);
        }

        private void AssertCanRetrieveAll()
        {
            IQuery query = NewQuery(typeof(Item));
            IObjectSet result = query.Execute();
            Assert.AreEqual(_items.Length, result.Count);

            Iterator4Assert.SameContent(result.GetEnumerator(), _items.GetEnumerator());
        }

        public void TestDefragment()
        {
            Defragment();
            AssertCanRetrieveAll();
        }

        public void TestDelete()
        {
            IQuery query = NewQuery(typeof(Item));
            IObjectSet result = query.Execute();
            while(result.HasNext())
            {
                Item item = (Item)result.Next();
                Db().Delete(item);
                Db().Delete(item._asInteger);
            }
        }

    	private void AssertAsLong()
        {
            Func<object, Item> itemFinder = delegate(object value)
            {
                return Find(_items,
                                    delegate(Item candidate)
                                    {
                                        return candidate._asLong == (EnumAsLong)value;
                                    });
            };

            AssertItem(EnumAsLong.First, itemFinder);
            AssertItem(EnumAsLong.Second, itemFinder);
        }

        private void AssertAsByte()
        {
            AssertItem(EnumAsByte.Second, AsByteFinder());
            AssertItem(EnumAsByte.Third, AsByteFinder());
        }

        private void AssertItem<T>(T expectedEnumValue, Func<object, Item> itemFinder)
        {
            IQuery query = NewQuery(typeof(Item));
            query.Descend(FieldNameFor(typeof(T))).Constrain(expectedEnumValue);

            IObjectSet result = query.Execute();

            Assert.AreEqual(1, result.Count);
            Item actual = (Item)result[0];
            Assert.IsNotNull(actual);

            Item expected = itemFinder(expectedEnumValue);
            Assert.IsNotNull(expected);

            Assert.AreEqual(expected, actual);
        }

        private void AssertItem<T>(T expectedEnumValue, Func<object, Item> itemFinder, Func<T, Predicate<Item>> selectorBuilder)
        {
            IList<Item> items = Db().Query(selectorBuilder(expectedEnumValue));
            Assert.AreEqual(1, items.Count);
            Assert.IsNotNull(items[0]);

            Item expected = itemFinder(expectedEnumValue);
            Assert.IsNotNull(expected);

            Assert.AreEqual(expected, items[0]);
        }

        private static string FieldNameFor(Type type)
        {
            return type.Name.Replace("EnumA", "_a");
        }

        static Predicate<Item> AsByteSelectorFor(EnumAsByte value)
        {
            return delegate(Item candidate)
            {
                return candidate._asByte == value;
            };

        }

        private static Func<object, Item> AsByteFinder()
        {
            return delegate(object value)
            {
                return Find(_items, delegate(Item candidate)
                    {
                        return candidate._asByte == (EnumAsByte)value;
                    });
            };
        }

        private static T Find<T>(T[] array, System.Predicate<T> expected)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (expected(array[i]))
                {
                    return array[i];
                }
            }
            return default(T);
        }

        // for .NET 2.0 compatibility
        public delegate R Func<T, R>(T value);

    }
}
