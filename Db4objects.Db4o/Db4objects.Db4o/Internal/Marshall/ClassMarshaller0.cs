namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ClassMarshaller0 : Db4objects.Db4o.Internal.Marshall.ClassMarshaller
	{
		protected override void ReadIndex(Db4objects.Db4o.Internal.ObjectContainerBase stream
			, Db4objects.Db4o.Internal.ClassMetadata clazz, Db4objects.Db4o.Internal.Buffer 
			reader)
		{
			int indexID = reader.ReadInt();
			if (!stream.MaintainsIndices() || !(stream is Db4objects.Db4o.Internal.LocalObjectContainer
				))
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
				new Db4objects.Db4o.Internal.Convert.Conversions.ClassIndexesToBTrees_5_5().Convert
					((Db4objects.Db4o.Internal.LocalObjectContainer)stream, indexID, Btree(clazz));
				stream.SetDirtyInSystemTransaction(clazz);
			}
		}

		private Db4objects.Db4o.Internal.Btree.BTree Btree(Db4objects.Db4o.Internal.ClassMetadata
			 clazz)
		{
			return Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy.Btree(clazz);
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
