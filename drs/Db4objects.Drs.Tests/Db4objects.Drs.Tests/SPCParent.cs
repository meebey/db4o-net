/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class SPCParent
	{
		private Db4objects.Drs.Tests.SPCChild child;

		private string name;

		public SPCParent()
		{
		}

		public SPCParent(string name)
		{
			this.name = name;
		}

		public SPCParent(Db4objects.Drs.Tests.SPCChild child, string name)
		{
			this.child = child;
			this.name = name;
		}

		public virtual Db4objects.Drs.Tests.SPCChild GetChild()
		{
			return child;
		}

		public virtual void SetChild(Db4objects.Drs.Tests.SPCChild child)
		{
			this.child = child;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return "SPCParent{" + "child=" + child + ", name='" + name + '\'' + '}';
		}
	}
}
