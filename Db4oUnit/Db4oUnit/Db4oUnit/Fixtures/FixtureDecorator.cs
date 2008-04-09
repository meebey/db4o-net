/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Fixtures;

namespace Db4oUnit.Fixtures
{
	internal sealed class FixtureDecorator : ITestDecorator
	{
		private readonly object _fixture;

		private readonly FixtureVariable _provider;

		private readonly int _fixtureIndex;

		internal FixtureDecorator(FixtureVariable provider, object fixture, int fixtureIndex
			)
		{
			_fixture = fixture;
			_provider = provider;
			_fixtureIndex = fixtureIndex;
		}

		public ITest Decorate(ITest test)
		{
			string label = _provider.Label + "[" + _fixtureIndex + "]";
			if (_fixture is ILabeled)
			{
				label += ":" + ((ILabeled)_fixture).Label();
			}
			return new FixtureDecoration(test, label, _provider, _fixture);
		}
	}
}
