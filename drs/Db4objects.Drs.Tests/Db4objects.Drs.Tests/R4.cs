/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class R4 : Db4objects.Drs.Tests.R3
	{
		internal Db4objects.Drs.Tests.R0 circle4;

		public virtual Db4objects.Drs.Tests.R0 GetCircle4()
		{
			return circle4;
		}

		public virtual void SetCircle4(Db4objects.Drs.Tests.R0 circle4)
		{
			this.circle4 = circle4;
		}
	}
}
