namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface IIDMapping
	{
		/// <returns>a mapping for the given id. if it does refer to a system handler or the empty reference (0), returns the given id.
		/// 	</returns>
		/// <exception cref="Db4objects.Db4o.MappingNotFoundException">if the given id does not refer to a system handler or the empty reference (0) and if no mapping is found
		/// 	</exception>
		int MappedID(int oldID);

		void MapIDs(int oldID, int newID);
	}
}
