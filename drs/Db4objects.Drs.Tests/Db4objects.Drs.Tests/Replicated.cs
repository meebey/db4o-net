/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Sharpen;

namespace Db4objects.Drs.Tests
{
	public class Replicated
	{
		private string name;

		private Db4objects.Drs.Tests.Replicated link;

		public Replicated()
		{
		}

		public Replicated(string name)
		{
			this.SetName(name);
		}

		public override string ToString()
		{
			return GetName() + ", hashcode = " + GetHashCode() + ", identity = " + Runtime.IdentityHashCode
				(this);
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual Db4objects.Drs.Tests.Replicated GetLink()
		{
			return link;
		}

		public virtual void SetLink(Db4objects.Drs.Tests.Replicated link)
		{
			this.link = link;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			if (!(o is Db4objects.Drs.Tests.Replicated))
			{
				return false;
			}
			return ((Db4objects.Drs.Tests.Replicated)o).name.Equals(name);
		}

		public override int GetHashCode()
		{
			if (name == null)
			{
				return 0;
			}
			return name.GetHashCode();
		}
	}
}
