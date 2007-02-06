namespace Db4objects.Db4o.Defragment
{
	/// <summary>In-memory mapping for IDs during a defragmentation run.</summary>
	/// <remarks>In-memory mapping for IDs during a defragmentation run.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public class TreeIDMapping : Db4objects.Db4o.Defragment.AbstractContextIDMapping
	{
		private Db4objects.Db4o.Foundation.Tree _tree;

		public override int MappedID(int oldID, bool lenient)
		{
			int classID = MappedClassID(oldID);
			if (classID != 0)
			{
				return classID;
			}
			Db4objects.Db4o.Internal.TreeIntObject res = (Db4objects.Db4o.Internal.TreeIntObject
				)Db4objects.Db4o.Internal.TreeInt.Find(_tree, oldID);
			if (res != null)
			{
				return ((int)res._object);
			}
			if (lenient)
			{
				Db4objects.Db4o.Internal.TreeIntObject nextSmaller = (Db4objects.Db4o.Internal.TreeIntObject
					)Db4objects.Db4o.Foundation.Tree.FindSmaller(_tree, new Db4objects.Db4o.Internal.TreeInt
					(oldID));
				if (nextSmaller != null)
				{
					int baseOldID = nextSmaller._key;
					int baseNewID = ((int)nextSmaller._object);
					return baseNewID + oldID - baseOldID;
				}
			}
			return 0;
		}

		public override void Open()
		{
		}

		public override void Close()
		{
		}

		protected override void MapNonClassIDs(int origID, int mappedID)
		{
			_tree = Db4objects.Db4o.Foundation.Tree.Add(_tree, new Db4objects.Db4o.Internal.TreeIntObject
				(origID, mappedID));
		}
	}
}
