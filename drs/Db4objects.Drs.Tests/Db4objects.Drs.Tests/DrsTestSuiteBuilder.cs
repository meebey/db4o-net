/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class DrsTestSuiteBuilder : ReflectionTestSuiteBuilder
	{
		private DrsFixturePair _fixtures;

		public DrsTestSuiteBuilder(IDrsFixture a, IDrsFixture b, Type clazz) : this(a, b, 
			new Type[] { clazz })
		{
		}

		public DrsTestSuiteBuilder(IDrsFixture a, IDrsFixture b, Type[] classes) : base(classes
			)
		{
			_fixtures = new DrsFixturePair(a, b);
		}

		protected override object WithContext(IClosure4 closure)
		{
			return DrsFixtureVariable.With(_fixtures, closure);
		}
	}
}
