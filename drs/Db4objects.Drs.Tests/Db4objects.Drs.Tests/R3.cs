/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class R3 : Db4objects.Drs.Tests.R2
	{
		internal Db4objects.Drs.Tests.R0 circle3;

		internal Db4objects.Drs.Tests.R4 r4;

		public virtual Db4objects.Drs.Tests.R0 GetCircle3()
		{
			return circle3;
		}

		public virtual void SetCircle3(Db4objects.Drs.Tests.R0 circle3)
		{
			this.circle3 = circle3;
		}

		public virtual Db4objects.Drs.Tests.R4 GetR4()
		{
			return r4;
		}

		public virtual void SetR4(Db4objects.Drs.Tests.R4 r4)
		{
			this.r4 = r4;
		}
	}
}
