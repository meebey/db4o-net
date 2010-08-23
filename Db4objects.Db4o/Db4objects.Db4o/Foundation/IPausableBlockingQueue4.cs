/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	public interface IPausableBlockingQueue4 : IBlockingQueue4
	{
		void Pause();

		void Resume();

		bool IsPaused();

		/// <summary>
		/// <p>
		/// Returns the next element in queue if there is one available, returns null
		/// otherwise.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Returns the next element in queue if there is one available, returns null
		/// otherwise.
		/// <p>
		/// This method will not never block, regardless of the queue being paused or
		/// no elements are available.
		/// </remarks>
		/// <returns>next element, if available and queue not paused.</returns>
		object TryNext();
	}
}
