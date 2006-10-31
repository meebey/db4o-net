namespace Db4objects.Db4o
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
		/// <exception cref="Db4objects.Db4o.MappingNotFoundException">if the given id does not refer to a system handler or the empty reference (0) and if no mapping is found
		/// 	</exception>
		int MappedID(int oldID);

		void MapIDs(int oldID, int newID, bool isClassID, bool seen);
	}
}
