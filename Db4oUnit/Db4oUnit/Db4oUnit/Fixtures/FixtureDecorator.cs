/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Fixtures;

namespace Db4oUnit.Fixtures
{
	internal sealed class FixtureDecorator : ITestDecorator
	{
		private readonly object _fixture;

		private readonly IFixtureProvider _provider;

		private readonly int _fixtureIndex;

		internal FixtureDecorator(IFixtureProvider provider, object fixture, int fixtureIndex
			)
		{
			_fixture = fixture;
			_provider = provider;
			_fixtureIndex = fixtureIndex;
		}

		public ITest Decorate(ITest test)
		{
			return new FixtureDecoration(test, _provider.Label() + "[" + _fixtureIndex + "]", 
				_provider.Variable(), _fixture);
		}
	}
}
