using System;
using System.Text;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class DecimalHandlerUpdateTestCase : HandlerUpdateTestCaseBase
    {
        public class Item
        {
            public decimal _typedPrimitive;

            public object _untyped;

#if NET_2_0 || CF_2_0
			public decimal? _nullablePrimitive;
#endif
        }

        public class ItemArrays
        {
            public decimal[] _typedPrimitiveArray;

            public object _primitiveArrayInObject;

#if NET_2_0 || CF_2_0
			public decimal?[] _nullableTypedPrimitiveArray;
#endif
        }

        private static readonly decimal[] data = new decimal[] {
            decimal.MinValue,
            decimal.MinValue + 1,
            -5,
            -1,
            0,
            1,
            5,
            decimal.MaxValue - 1,
            decimal.MaxValue,
        };

        protected override void AssertArrays(object obj)
        {
            ItemArrays itemArrays = (ItemArrays)obj;
            for (int i = 0; i < data.Length; i++)
            {
                AssertAreEqual(data[i], itemArrays._typedPrimitiveArray[i]);
                AssertAreEqual(data[i], ((decimal[])itemArrays._primitiveArrayInObject)[i]);
                //AssertAreEqual(data[i], (decimal) itemArrays._nullableTypedPrimitiveArray[i]);
            }
            AssertAreEqual(0, itemArrays._typedPrimitiveArray[data.Length]);
            AssertAreEqual(0, ((decimal[])itemArrays._primitiveArrayInObject)[data.Length]);
        }

        protected override void AssertValues(object[] values)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Item item = (Item)values[i];
                AssertAreEqual(data[i], item._typedPrimitive);
                AssertAreEqual(data[i], (decimal) item._untyped);
#if NET_2_0 || CF_2_0
                AssertAreEqual(data[i], (decimal) item._nullablePrimitive);
#endif
				}
            Item nullItem = (Item)values[data.Length];
            AssertAreEqual(0, nullItem._typedPrimitive);
            Assert.IsNull(nullItem._untyped);
#if NET_2_0 || CF_2_0
            Assert.IsNull(nullItem._nullablePrimitive);
#endif
			}

        private void AssertAreEqual(decimal expected, decimal actual)
        {
            Assert.AreEqual(expected, actual);
        }

        protected override object CreateArrays()
        {
            ItemArrays itemArrays = new ItemArrays();
            itemArrays._typedPrimitiveArray = new decimal[data.Length + 1];
            System.Array.Copy(data, 0, itemArrays._typedPrimitiveArray, 0, data.Length);

            decimal[] decimalArray = new decimal[data.Length + 1];
            System.Array.Copy(data, 0, decimalArray, 0, data.Length);
            itemArrays._primitiveArrayInObject = decimalArray;

#if NET_2_0 || CF_2_0
            itemArrays._nullableTypedPrimitiveArray = new decimal?[data.Length + 1];
            for (int i = 0; i < data.Length; i++)
            {
                itemArrays._nullableTypedPrimitiveArray[i] = data[i];
            }
#endif
			return itemArrays;
        }

        protected override object[] CreateValues()
        {
            Item[] values = new Item[data.Length + 1];
            for (int i = 0; i < data.Length; i++)
            {
                Item item = new Item();
                item._typedPrimitive = data[i];
                item._untyped = data[i];
#if NET_2_0 || CF_2_0
                item._nullablePrimitive = data[i];
#endif
				values[i] = item;
            }

            values[data.Length] = new Item();
            return values;
        }

        protected override string TypeName()
        {
            return "decimal";
        }
    }
}
