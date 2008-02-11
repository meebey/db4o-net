/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>In-memory mapping for IDs during a defragmentation run.</summary>
	/// <remarks>In-memory mapping for IDs during a defragmentation run.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public class TreeIDMapping : AbstractContextIDMapping
	{
		private Tree _tree;

		public override int MappedID(int oldID, bool lenient)
		{
			int classID = MappedClassID(oldID);
			if (classID != 0)
			{
				return classID;
			}
			TreeIntObject res = (TreeIntObject)TreeInt.Find(_tree, oldID);
			if (res != null)
			{
				return ((int)res._object);
			}
			if (lenient)
			{
				TreeIntObject nextSmaller = (TreeIntObject)Tree.FindSmaller(_tree, new TreeInt(oldID
					));
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
			_tree = Tree.Add(_tree, new TreeIntObject(origID, mappedID));
		}
	}
}
