/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Mapping;

namespace Db4objects.Db4o.Internal.Mapping
{
	/// <summary>A mapping from yap file source IDs/addresses to target IDs/addresses, used for defragmenting.
	/// 	</summary>
	/// <remarks>A mapping from yap file source IDs/addresses to target IDs/addresses, used for defragmenting.
	/// 	</remarks>
	/// <exclude></exclude>
	public interface IIDMapping
	{
		/// <returns>a mapping for the given id. if it does refer to a system handler or the empty reference (0), returns the given id.
		/// 	</returns>
		/// <exception cref="MappingNotFoundException">if the given id does not refer to a system handler or the empty reference (0) and if no mapping is found
		/// 	</exception>
		int MappedID(int oldID);

		void MapIDs(int oldID, int newID, bool isClassID);
	}
}
