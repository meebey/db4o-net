/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;
using Sharpen;
using Sharpen.Util;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class DateHandlerUpdateTestCase : HandlerUpdateTestCaseBase
	{
		public class Item
		{
			public Date _date;

			public object _untyped;
		}

		public class ItemArrays
		{
			public Date[] _dateArray;

			public object[] _untypedObjectArray;

			public object _arrayInObject;
		}

		private static readonly Date[] data = new Date[] { new Date(long.MinValue), new Date
			(long.MinValue + 1), new Date(-1), new Date(0), new Date(1), new Date(long.MaxValue
			 - 1), new Date(long.MaxValue) };

		public static void Main(string[] args)
		{
			new TestRunner(typeof(DateHandlerUpdateTestCase)).Run();
		}

		protected override void AssertArrays(object obj)
		{
			DateHandlerUpdateTestCase.ItemArrays itemArrays = (DateHandlerUpdateTestCase.ItemArrays
				)obj;
			Date[] dateArray = (Date[])itemArrays._arrayInObject;
			for (int i = 0; i < data.Length; i++)
			{
				AssertAreEqual(data[i], itemArrays._dateArray[i]);
				AssertAreEqual(data[i], (Date)itemArrays._untypedObjectArray[i]);
				AssertAreEqual(data[i], dateArray[i]);
			}
			Assert.IsNull(itemArrays._untypedObjectArray[data.Length]);
			Assert.IsNull(dateArray[data.Length]);
		}

		protected override void AssertValues(object[] values)
		{
			for (int i = 0; i < data.Length; i++)
			{
				DateHandlerUpdateTestCase.Item item = (DateHandlerUpdateTestCase.Item)values[i];
				AssertAreEqual(data[i], item._date);
				AssertAreEqual(data[i], (Date)item._untyped);
			}
			DateHandlerUpdateTestCase.Item nullItem = (DateHandlerUpdateTestCase.Item)values[
				values.Length - 1];
			Assert.IsNull(nullItem._date);
			Assert.IsNull(nullItem._untyped);
		}

		private void AssertAreEqual(Date expected, Date actual)
		{
			if (expected.Equals(new Date(long.MaxValue)) && _handlerVersion == 0)
			{
				expected = null;
			}
			Assert.AreEqual(expected, actual);
		}

		protected override object CreateArrays()
		{
			DateHandlerUpdateTestCase.ItemArrays itemArrays = new DateHandlerUpdateTestCase.ItemArrays
				();
			itemArrays._dateArray = new Date[data.Length + 1];
			System.Array.Copy(data, 0, itemArrays._dateArray, 0, data.Length);
			itemArrays._untypedObjectArray = new object[data.Length + 1];
			System.Array.Copy(data, 0, itemArrays._untypedObjectArray, 0, data.Length);
			Date[] dateArray = new Date[data.Length + 1];
			System.Array.Copy(data, 0, dateArray, 0, data.Length);
			itemArrays._arrayInObject = dateArray;
			return itemArrays;
		}

		protected override object[] CreateValues()
		{
			DateHandlerUpdateTestCase.Item[] values = new DateHandlerUpdateTestCase.Item[data
				.Length + 1];
			for (int i = 0; i < data.Length; i++)
			{
				DateHandlerUpdateTestCase.Item item = new DateHandlerUpdateTestCase.Item();
				item._date = data[i];
				item._untyped = data[i];
				values[i] = item;
			}
			values[values.Length - 1] = new DateHandlerUpdateTestCase.Item();
			return values;
		}

		protected override string TypeName()
		{
			return "date";
		}
	}
}
