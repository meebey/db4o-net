using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
	class NullableArraysElementsTestCase : AbstractDb4oTestCase
	{
		protected override void Store()
		{
			Store(new TestSubject<int?>(CreateNullableIntArray(), 0xDB40, new VTTestSubject(0x04BD, "foo", true)));
			Store(new TestSubject<object>(CreateNullableObjectArray(), 0xDB40, new VTTestSubject(0x04BD, "bar", true)));
		}

        public void TestArrayType()
        {
            if (Db4objects.Db4o.Internal.NullableArrayHandling.Disabled())
            {
                return;
            }
            TestSubject<int?> testSubject = (TestSubject<int?>)RetrieveOnlyInstance(typeof(TestSubject<int?>));
            Assert.IsInstanceOf(typeof(int?[]), testSubject._elements);
        }

		public void _TestNullableItemsInUntypeArray()
		{
			TestSubject<object> testSubject = (TestSubject<object>) RetrieveOnlyInstance(typeof(TestSubject<object>));
			AssertNullableType(testSubject, "bar");
		}
		
		public void _TestNullableItemsInTypeArray()
		{
			// FIXME: Enable this test when arrays of nullable itens is supported.
			TestSubject<int?> testSubject = (TestSubject<int?>)RetrieveOnlyInstance(typeof(TestSubject<int?>));
			AssertNullableType(testSubject, "foo");
		}


		private static void AssertNullableType<T>(TestSubject<T> testSubject, string name)
		{
			Assert.IsNotNull(testSubject);
			Assert.AreEqual(0xDB40, testSubject._value);

			Assert.IsTrue(testSubject._nullableInt.HasValue);
			Assert.IsFalse(testSubject._valueType2.HasValue);
			Assert.IsNotNull(testSubject._valueType);
			Assert.AreEqual(0x04BD, testSubject._valueType.Value._value);
			Assert.AreEqual(name, testSubject._valueType.Value._name);
			Assert.IsTrue(testSubject._valueType.Value._nullableBool.HasValue);
			Assert.IsTrue(testSubject._valueType.Value._nullableBool.Value);

			Assert.IsNotNull(testSubject._elements);
			for(int i=0; i < testSubject._elements.Length; i += 2)
			{
				Assert.IsNull(testSubject._elements[i]);
			}

			for (int i = 1; i < testSubject._elements.Length; i += 2)
			{
				Assert.IsNotNull(testSubject._elements[i]);
				Assert.AreEqual(i, testSubject._elements[i]);
			}
		}

		private static object[] CreateNullableObjectArray()
		{
			object[] items = new object[10];
			for (int i = 1; i < items.Length; i += 2)
			{
				items[i] = new Nullable<int>(i);
			}

			return items;
		}

		private static int?[] CreateNullableIntArray()
		{
			int?[] items = new int?[10];
			for (int i = 1; i < items.Length; i += 2)
			{
				items[i] = new Nullable<int>(i);
			}

			return items;
		}
	}

	class TestSubject<T>
	{
		public T[] _elements;
		public int _value;
		public int? _nullableInt;
		public VTTestSubject? _valueType;
		public VTTestSubject? _valueType2;

		public TestSubject(T[] items, int value, VTTestSubject vtt)
		{
			_elements = items;
			_value = value;
			_nullableInt = value;
			_valueType = vtt;
			_valueType2 = null;
		}
	}

	struct VTTestSubject
	{
		public int _value;
		public string _name;
		public bool? _nullableBool;

		public VTTestSubject(int value, string name, bool aBool)
		{
			_value = value;
			_name = name;
			_nullableBool = aBool;
		}
	}
}