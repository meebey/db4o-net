namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public class ClassIndexesToBTrees_5_5 : Db4objects.Db4o.Internal.Convert.Conversion
	{
		public const int VERSION = 5;

		public virtual void Convert(Db4objects.Db4o.Internal.LocalObjectContainer yapFile
			, int classIndexId, Db4objects.Db4o.Internal.Btree.BTree bTree)
		{
			Db4objects.Db4o.Internal.Transaction trans = yapFile.GetSystemTransaction();
			Db4objects.Db4o.Internal.Buffer reader = yapFile.ReadReaderByID(trans, classIndexId
				);
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

		public override void Convert(Db4objects.Db4o.Internal.Convert.ConversionStage.SystemUpStage
			 stage)
		{
			stage.File().StoredClasses();
		}
	}
}
