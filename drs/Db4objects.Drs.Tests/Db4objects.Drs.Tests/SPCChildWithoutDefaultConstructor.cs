/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class SPCChildWithoutDefaultConstructor : Db4objects.Drs.Tests.SPCChild
	{
		public SPCChildWithoutDefaultConstructor(string name) : base(name)
		{
		}
	}
}
