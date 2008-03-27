/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Fixtures
{
	public interface IFixtureProvider : IEnumerable
	{
		string Label();

		ContextVariable Variable();
	}
}
