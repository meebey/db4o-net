/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Tests.Common.Sampledata;

namespace Db4objects.Db4o.Tests.Common.Sampledata
{
	public class MoleculeData : Db4objects.Db4o.Tests.Common.Sampledata.AtomData
	{
		public MoleculeData()
		{
		}

		public MoleculeData(Db4objects.Db4o.Tests.Common.Sampledata.AtomData child) : base
			(child)
		{
		}

		public MoleculeData(string name) : base(name)
		{
		}

		public MoleculeData(Db4objects.Db4o.Tests.Common.Sampledata.AtomData child, string
			 name) : base(child, name)
		{
		}

		public override bool Equals(object obj)
		{
			if (obj is MoleculeData)
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
