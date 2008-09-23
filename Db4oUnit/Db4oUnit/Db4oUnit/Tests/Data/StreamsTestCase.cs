/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4oUnit.Data;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Tests.Data
{
	public class StreamsTestCase : ITestCase
	{
		public virtual void TestSeries()
		{
			Collection4 calls = new Collection4();
			IEnumerator series = Streams.Series(string.Empty, new _IFunction4_12(calls)).GetEnumerator
				();
			Assert.IsTrue(series.MoveNext());
			Assert.IsTrue(series.MoveNext());
			Iterator4Assert.AreEqual(new object[] { string.Empty, "*" }, calls.GetEnumerator(
				));
		}

		private sealed class _IFunction4_12 : IFunction4
		{
			public _IFunction4_12(Collection4 calls)
			{
				this.calls = calls;
			}

			public object Apply(object value)
			{
				calls.Add(value);
				return value + "*";
			}

			private readonly Collection4 calls;
		}
	}
}
