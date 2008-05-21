/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class DrsFixtureVariable
	{
		private static readonly FixtureVariable _variable = new FixtureVariable("drs");

		public static DrsFixturePair Value()
		{
			return (DrsFixturePair)_variable.Value;
		}

		public static object With(DrsFixturePair pair, IClosure4 closure)
		{
			return _variable.With(pair, closure);
		}
	}
}
