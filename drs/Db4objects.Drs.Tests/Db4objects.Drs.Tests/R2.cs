/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class R2 : Db4objects.Drs.Tests.R1
	{
		internal Db4objects.Drs.Tests.R0 circle2;

		internal Db4objects.Drs.Tests.R3 r3;

		public virtual Db4objects.Drs.Tests.R0 GetCircle2()
		{
			return circle2;
		}

		public virtual void SetCircle2(Db4objects.Drs.Tests.R0 circle2)
		{
			this.circle2 = circle2;
		}

		public virtual Db4objects.Drs.Tests.R3 GetR3()
		{
			return r3;
		}

		public virtual void SetR3(Db4objects.Drs.Tests.R3 r3)
		{
			this.r3 = r3;
		}
	}
}
