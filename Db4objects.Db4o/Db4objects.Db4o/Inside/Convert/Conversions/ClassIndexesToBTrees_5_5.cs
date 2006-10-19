namespace Db4objects.Db4o.Inside.Convert.Conversions
{
	/// <exclude></exclude>
	public class ClassIndexesToBTrees_5_5 : Db4objects.Db4o.Inside.Convert.Conversion
	{
		public const int VERSION = 5;

		public virtual void Convert(Db4objects.Db4o.YapFile yapFile, int classIndexId, Db4objects.Db4o.Inside.Btree.BTree
			 bTree)
		{
			Db4objects.Db4o.Transaction trans = yapFile.GetSystemTransaction();
			Db4objects.Db4o.YapReader reader = yapFile.ReadReaderByID(trans, classIndexId);
			if (reader == null)
			{
				return;
			}
			int entries = reader.ReadInt();
			for (int i = 0; i < entries; i++)
			{
				bTree.Add(trans, reader.ReadInt());
			}
		}

		public override void Convert(Db4objects.Db4o.Inside.Convert.ConversionStage.SystemUpStage
			 stage)
		{
			stage.File().StoredClasses();
		}
	}
}
