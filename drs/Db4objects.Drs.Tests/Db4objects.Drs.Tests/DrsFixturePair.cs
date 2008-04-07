/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class DrsFixturePair
	{
		public readonly IDrsFixture a;

		public readonly IDrsFixture b;

		public DrsFixturePair(IDrsFixture fixtureA, IDrsFixture fixtureB)
		{
			if (null == fixtureA)
			{
				throw new ArgumentException("fixtureA");
			}
			if (null == fixtureB)
			{
				throw new ArgumentException("fixtureB");
			}
			a = fixtureA;
			b = fixtureB;
		}
	}
}
