/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class R0
	{
		internal string name;

		internal Db4objects.Drs.Tests.R0 r0;

		internal Db4objects.Drs.Tests.R1 r1;

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual Db4objects.Drs.Tests.R0 GetR0()
		{
			return r0;
		}

		public virtual void SetR0(Db4objects.Drs.Tests.R0 r0)
		{
			this.r0 = r0;
		}

		public virtual Db4objects.Drs.Tests.R1 GetR1()
		{
			return r1;
		}

		public virtual void SetR1(Db4objects.Drs.Tests.R1 r1)
		{
			this.r1 = r1;
		}
	}
}
