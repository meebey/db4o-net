/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Config
{
	/// <summary>Interface to configure the cache configurations.</summary>
	/// <remarks>Interface to configure the cache configurations.</remarks>
	public interface ICacheConfiguration
	{
		/// <summary>
		/// configures the size of the slot cache to hold a number of
		/// slots in the cache.
		/// </summary>
		/// <remarks>
		/// configures the size of the slot cache to hold a number of
		/// slots in the cache.
		/// </remarks>
		/// <param name="size">the number of slots</param>
		int SlotCacheSize
		{
			set;
		}
	}
}
