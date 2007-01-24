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
			if (obj is Db4objects.Db4o.Tests.Common.Sampledata.MoleculeData)
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
