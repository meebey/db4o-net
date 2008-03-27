/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4oUnit.Tests.Fixtures;

namespace Db4oUnit.Tests.Fixtures
{
	public class Set4TestUnit : ITestLifeCycle
	{
		private readonly ISet4 subject = (ISet4)SubjectFixtureProvider.Value();

		private readonly object[] data = MultiValueFixtureProvider.Value();

		public virtual void SetUp()
		{
			for (int i = 0; i < data.Length; ++i)
			{
				object element = data[i];
				subject.Add(element);
			}
		}

		public virtual void TestSize()
		{
			Assert.AreEqual(data.Length, subject.Size());
		}

		public virtual void TestContains()
		{
			for (int i = 0; i < data.Length; ++i)
			{
				object element = data[i];
				Assert.IsTrue(subject.Contains(element));
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
		}
	}
}
