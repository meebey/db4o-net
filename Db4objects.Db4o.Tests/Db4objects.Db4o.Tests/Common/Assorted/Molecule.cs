/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class Molecule : Db4objects.Db4o.Tests.Common.Assorted.Atom
	{
		public Molecule()
		{
		}

		public Molecule(Db4objects.Db4o.Tests.Common.Assorted.Atom child) : base(child)
		{
		}

		public Molecule(string name) : base(name)
		{
		}

		public Molecule(Db4objects.Db4o.Tests.Common.Assorted.Atom child, string name) : 
			base(child, name)
		{
		}

		public override bool Equals(object obj)
		{
			if (obj is Molecule)
			{
				return base.Equals(obj);
			}
			return false;
		}

		public override string ToString()
		{
			string str = "Molecule(" + name + ")";
			if (child != null)
			{
				return str + "." + child.ToString();
			}
			return str;
		}
	}
}
