namespace Db4objects.Db4o.Inside.Convert.Conversions
{
	/// <exclude></exclude>
	public class FieldIndexesToBTrees_5_7 : Db4objects.Db4o.Inside.Convert.Conversion
	{
		public const int VERSION = 6;

		public override void Convert(Db4objects.Db4o.Inside.Convert.ConversionStage.SystemUpStage
			 stage)
		{
			stage.File().ClassCollection().WriteAllClasses();
			RebuildUUIDIndex(stage.File());
			FreeOldUUIDMetaIndex(stage.File());
		}

		private void RebuildUUIDIndex(Db4objects.Db4o.YapFile file)
		{
			Db4objects.Db4o.YapFieldUUID uuid = file.GetFieldUUID();
			Db4objects.Db4o.YapClassCollectionIterator i = file.ClassCollection().Iterator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.YapClass clazz = i.CurrentClass();
				if (clazz.GenerateUUIDs())
				{
					uuid.RebuildIndexForClass(file, clazz);
				}
			}
		}

		private void FreeOldUUIDMetaIndex(Db4objects.Db4o.YapFile file)
		{
			Db4objects.Db4o.Header.FileHeader fh = file.GetFileHeader();
			if (!(fh is Db4objects.Db4o.Header.FileHeader0))
			{
				return;
			}
			Db4objects.Db4o.MetaIndex metaIndex = ((Db4objects.Db4o.Header.FileHeader0)fh).GetUUIDMetaIndex
				();
			if (metaIndex == null)
			{
				return;
			}
			file.Free(metaIndex.indexAddress, metaIndex.indexLength);
		}
	}
}
