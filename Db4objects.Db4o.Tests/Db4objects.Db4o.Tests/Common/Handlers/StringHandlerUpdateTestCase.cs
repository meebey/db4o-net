/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class StringHandlerUpdateTestCase : HandlerUpdateTestCaseBase
	{
		private static readonly string[] data = new string[] { "one", "aAzZ|!§$%&/()=?ßöäüÄÖÜYZ;:-_+*~#^°'@"
			, string.Empty, null };

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(StringHandlerUpdateTestCase)).Run();
		}

		protected override string TypeName()
		{
			return "string";
		}

		public class Item
		{
			public string _typed;

			public object _untyped;
		}

		public class ItemArrays
		{
			public string[] _typedArray;

			public object[] _untypedArray;

			public object _arrayInObject;
		}

		protected override object[] CreateValues()
		{
			StringHandlerUpdateTestCase.Item[] values = new StringHandlerUpdateTestCase.Item[
				data.Length + 1];
			for (int i = 0; i < data.Length; i++)
			{
				StringHandlerUpdateTestCase.Item item = new StringHandlerUpdateTestCase.Item();
				values[i] = item;
				item._typed = data[i];
				item._untyped = data[i];
			}
			values[values.Length - 1] = new StringHandlerUpdateTestCase.Item();
			return values;
		}

		protected override object CreateArrays()
		{
			StringHandlerUpdateTestCase.ItemArrays item = new StringHandlerUpdateTestCase.ItemArrays
				();
			CreateTypedArray(item);
			CreateUntypedArray(item);
			CreateArrayInObject(item);
			return item;
		}

		private void CreateUntypedArray(StringHandlerUpdateTestCase.ItemArrays item)
		{
			item._untypedArray = new string[data.Length + 1];
			for (int i = 0; i < data.Length; i++)
			{
				item._untypedArray[i] = data[i];
			}
		}

		private void CreateTypedArray(StringHandlerUpdateTestCase.ItemArrays item)
		{
			item._typedArray = new string[data.Length];
			System.Array.Copy(data, 0, item._typedArray, 0, data.Length);
		}

		private void CreateArrayInObject(StringHandlerUpdateTestCase.ItemArrays item)
		{
			string[] arr = new string[data.Length];
			System.Array.Copy(data, 0, arr, 0, data.Length);
			item._arrayInObject = arr;
		}

		protected override void AssertValues(object[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				StringHandlerUpdateTestCase.Item item = (StringHandlerUpdateTestCase.Item)values[
					i];
				AssertAreEqual(data[i], item._typed);
				AssertAreEqual(data[i], (string)item._untyped);
			}
			StringHandlerUpdateTestCase.Item nullItem = (StringHandlerUpdateTestCase.Item)values
				[values.Length - 1];
			Assert.IsNull(nullItem._typed);
			Assert.IsNull(nullItem._untyped);
		}

		protected override void AssertArrays(object obj)
		{
			StringHandlerUpdateTestCase.ItemArrays item = (StringHandlerUpdateTestCase.ItemArrays
				)obj;
			AssertTypedArray(item);
			AssertUntypedArray(item);
			AssertArrayInObject(item);
		}

		private void AssertTypedArray(StringHandlerUpdateTestCase.ItemArrays item)
		{
			AssertData(item._typedArray);
		}

		protected virtual void AssertUntypedArray(StringHandlerUpdateTestCase.ItemArrays 
			item)
		{
			for (int i = 0; i < data.Length; i++)
			{
				AssertAreEqual(data[i], (string)item._untypedArray[i]);
			}
			Assert.IsNull(item._untypedArray[item._untypedArray.Length - 1]);
		}

		private void AssertArrayInObject(StringHandlerUpdateTestCase.ItemArrays item)
		{
			AssertData((string[])item._arrayInObject);
		}

		private void AssertData(string[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				AssertAreEqual(data[i], values[i]);
			}
		}

		private void AssertAreEqual(string expected, string actual)
		{
			Assert.AreEqual(expected, actual);
		}
	}
}
