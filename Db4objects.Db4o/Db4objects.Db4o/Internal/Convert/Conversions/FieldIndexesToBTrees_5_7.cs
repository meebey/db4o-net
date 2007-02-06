namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public class FieldIndexesToBTrees_5_7 : Db4objects.Db4o.Internal.Convert.Conversion
	{
		public const int VERSION = 6;

		public override void Convert(Db4objects.Db4o.Internal.Convert.ConversionStage.SystemUpStage
			 stage)
		{
			stage.File().ClassCollection().WriteAllClasses();
			RebuildUUIDIndex(stage.File());
			FreeOldUUIDMetaIndex(stage.File());
		}

		private void RebuildUUIDIndex(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			Db4objects.Db4o.Internal.UUIDFieldMetadata uuid = file.GetUUIDIndex();
			Db4objects.Db4o.Internal.ClassMetadataIterator i = file.ClassCollection().Iterator
				();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.ClassMetadata clazz = i.CurrentClass();
				if (clazz.GenerateUUIDs())
				{
					uuid.RebuildIndexForClass(file, clazz);
				}
			}
		}

		private void FreeOldUUIDMetaIndex(Db4objects.Db4o.Internal.LocalObjectContainer file
			)
		{
			Db4objects.Db4o.Internal.Fileheader.FileHeader fh = file.GetFileHeader();
			if (!(fh is Db4objects.Db4o.Internal.Fileheader.FileHeader0))
			{
				return;
			}
			Db4objects.Db4o.MetaIndex metaIndex = ((Db4objects.Db4o.Internal.Fileheader.FileHeader0
				)fh).GetUUIDMetaIndex();
			if (metaIndex == null)
			{
				return;
			}
			file.Free(metaIndex.indexAddress, metaIndex.indexLength);
		}
	}
}
