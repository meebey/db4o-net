/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit.Extensions.Foundation;
using Db4oUnit.Extensions.Mocking;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Extensions.Mocking
{
	public class MethodCallRecorder : IEnumerable
	{
		private readonly Collection4 _calls = new Collection4();

		public virtual IEnumerator GetEnumerator()
		{
			return _calls.GetEnumerator();
		}

		public virtual void Record(MethodCall call)
		{
			_calls.Add(call);
		}

		public virtual void Reset()
		{
			_calls.Clear();
		}

		/// <summary>Asserts that the method calls were the same as expectedCalls.</summary>
		/// <remarks>
		/// Asserts that the method calls were the same as expectedCalls.
		/// Unfortunately we cannot call this method 'assert' because
		/// it's a keyword starting with java 1.5.
		/// </remarks>
		/// <param name="expectedCalls"></param>
		public virtual void Verify(MethodCall[] expectedCalls)
		{
			Iterator4Assert.AreEqual(expectedCalls, GetEnumerator());
		}
	}
}
