/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	public interface IBlockingQueue4 : IQueue4
	{
		/// <summary><p>Returns the next queued item or waits for it to be available for the maximum of <code>timeout</code> miliseconds.
		/// 	</summary>
		/// <remarks><p>Returns the next queued item or waits for it to be available for the maximum of <code>timeout</code> miliseconds.
		/// 	</remarks>
		/// <param name="timeout">maximum time to wait for the next avilable item in miliseconds
		/// 	</param>
		/// <returns>the next item or <code>null</code> if <code>timeout</code> is reached</returns>
		/// <exception cref="BlockingQueueStoppedException">
		/// if the
		/// <see cref="Stop()">Stop()</see>
		/// is called.
		/// </exception>
		/// <exception cref="Db4objects.Db4o.Foundation.BlockingQueueStoppedException"></exception>
		object Next(long timeout);

		void Stop();
	}
}
