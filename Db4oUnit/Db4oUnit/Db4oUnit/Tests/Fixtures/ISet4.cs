/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4oUnit.Tests.Fixtures
{
	public interface ISet4
	{
		void Add(object value);

		bool Contains(object value);

		int Size();
	}
}
