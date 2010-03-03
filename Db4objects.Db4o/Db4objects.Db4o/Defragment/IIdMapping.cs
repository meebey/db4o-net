/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>The ID mapping used internally during a defragmentation run.</summary>
	/// <remarks>The ID mapping used internally during a defragmentation run.</remarks>
	/// <seealso cref="Defragment">Defragment</seealso>
	public interface IIdMapping
	{
		/// <summary>Returns a previously registered mapping ID for the given ID if it exists.
		/// 	</summary>
		/// <remarks>
		/// Returns a previously registered mapping ID for the given ID if it exists.
		/// If lenient mode is set to true, will provide the mapping ID for the next
		/// smaller original ID a mapping exists for, plus the difference between the
		/// found ID and the original ID. Otherwise it returns 0.
		/// </remarks>
		/// <param name="origID">The original ID</param>
		/// <param name="lenient">If true, lenient mode will be used for lookup, strict mode otherwise.
		/// 	</param>
		/// <returns>The mapping ID for the given original ID or 0, if none has been registered.
		/// 	</returns>
		int MappedId(int origId, bool lenient);

		/// <summary>Registers a mapping for the given IDs.</summary>
		/// <remarks>Registers a mapping for the given IDs.</remarks>
		/// <param name="origID">The original ID</param>
		/// <param name="mappedID">The ID to be mapped to the original ID.</param>
		/// <param name="isClassID">true if the given original ID specifies a class slot, false otherwise.
		/// 	</param>
		void MapId(int origId, int mappedId, bool isClassId);

		/// <summary>Maps an ID to a slot</summary>
		/// <param name="id"></param>
		/// <param name="slot"></param>
		void MapId(int id, Slot slot);

		/// <summary>provides a Visitable of all mappings of IDs to slots.</summary>
		/// <remarks>provides a Visitable of all mappings of IDs to slots.</remarks>
		IVisitable SlotChanges();

		/// <summary>Prepares the mapping for use.</summary>
		/// <remarks>Prepares the mapping for use.</remarks>
		/// <exception cref="System.IO.IOException"></exception>
		void Open();

		/// <summary>Shuts down the mapping after use.</summary>
		/// <remarks>Shuts down the mapping after use.</remarks>
		void Close();
	}
}
