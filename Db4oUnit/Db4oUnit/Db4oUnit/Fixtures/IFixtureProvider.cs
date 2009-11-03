/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit.Fixtures;

namespace Db4oUnit.Fixtures
{
	public interface IFixtureProvider : IEnumerable
	{
		FixtureVariable Variable();
	}
}
