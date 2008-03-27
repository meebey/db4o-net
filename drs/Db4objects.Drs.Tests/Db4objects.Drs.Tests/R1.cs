/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class R1 : Db4objects.Drs.Tests.R0
	{
		internal Db4objects.Drs.Tests.R0 circle1;

		internal Db4objects.Drs.Tests.R2 r2;

		public virtual Db4objects.Drs.Tests.R0 GetCircle1()
		{
			return circle1;
		}

		public virtual void SetCircle1(Db4objects.Drs.Tests.R0 circle1)
		{
			this.circle1 = circle1;
		}

		public virtual Db4objects.Drs.Tests.R2 GetR2()
		{
			return r2;
		}

		public virtual void SetR2(Db4objects.Drs.Tests.R2 r2)
		{
			this.r2 = r2;
		}
	}
}
