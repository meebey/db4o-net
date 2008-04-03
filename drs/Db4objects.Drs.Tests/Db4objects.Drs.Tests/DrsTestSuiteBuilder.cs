/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class DrsTestSuiteBuilder : ReflectionTestSuiteBuilder
	{
		private IDrsFixture _a;

		private IDrsFixture _b;

		public DrsTestSuiteBuilder(IDrsFixture a, IDrsFixture b, Type clazz) : base(clazz
			)
		{
			A(a);
			B(b);
		}

		public DrsTestSuiteBuilder(IDrsFixture a, IDrsFixture b, Type[] classes) : base(classes
			)
		{
			A(a);
			B(b);
		}

		private void A(IDrsFixture fixture)
		{
			if (null == fixture)
			{
				throw new ArgumentException("fixture");
			}
			_a = fixture;
		}

		private void B(IDrsFixture fixture)
		{
			if (null == fixture)
			{
				throw new ArgumentException("fixture");
			}
			_b = fixture;
		}

		protected override object NewInstance(Type clazz)
		{
			object instance = base.NewInstance(clazz);
			if (instance is DrsTestCase)
			{
				DrsTestCase testCase = (DrsTestCase)instance;
				testCase.A(_a);
				testCase.B(_b);
			}
			return instance;
		}
	}
}
