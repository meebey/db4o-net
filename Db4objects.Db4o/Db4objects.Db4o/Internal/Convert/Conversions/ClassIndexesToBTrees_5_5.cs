/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Convert;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public class ClassIndexesToBTrees_5_5 : Conversion
	{
		public const int VERSION = 5;

		public virtual void Convert(LocalObjectContainer yapFile, int classIndexId, BTree
			 bTree)
		{
			Transaction trans = yapFile.SystemTransaction();
			BufferImpl reader = yapFile.ReadReaderByID(trans, classIndexId);
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

		public override void Convert(ConversionStage.SystemUpStage stage)
		{
			stage.File().StoredClasses();
		}
	}
}
