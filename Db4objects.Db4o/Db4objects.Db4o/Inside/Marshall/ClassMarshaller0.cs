namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public class ClassMarshaller0 : Db4objects.Db4o.Inside.Marshall.ClassMarshaller
	{
		protected override void ReadIndex(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapClass
			 clazz, Db4objects.Db4o.YapReader reader)
		{
			int indexID = reader.ReadInt();
			if (!stream.MaintainsIndices() || !(stream is Db4objects.Db4o.YapFile))
			{
				return;
			}
			if (Btree(clazz) != null)
			{
				return;
			}
			clazz.Index().Read(stream, ValidIndexId(indexID));
			if (IsOldClassIndex(indexID))
			{
				new Db4objects.Db4o.Inside.Convert.Conversions.ClassIndexesToBTrees_5_5().Convert
					((Db4objects.Db4o.YapFile)stream, indexID, Btree(clazz));
				stream.SetDirtyInSystemTransaction(clazz);
			}
		}

		private Db4objects.Db4o.Inside.Btree.BTree Btree(Db4objects.Db4o.YapClass clazz)
		{
			return Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy.Btree(clazz);
		}

		private int ValidIndexId(int indexID)
		{
			return IsOldClassIndex(indexID) ? 0 : -indexID;
		}

		private bool IsOldClassIndex(int indexID)
		{
			return indexID > 0;
		}

		protected override int IndexIDForWriting(int indexID)
		{
			return indexID;
		}
	}
}
