/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.IO
{
	/// <summary>Strategy for file/byte array growth.</summary>
	/// <remarks>Strategy for file/byte array growth.</remarks>
	public interface IGrowthStrategy
	{
		/// <param name="curSize">The current size of the entity</param>
		/// <returns>The new size of the entity, must be bigger than curSize</returns>
		long NewSize(long curSize);
	}
}
