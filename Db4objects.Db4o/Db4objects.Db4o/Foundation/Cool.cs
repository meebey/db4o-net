/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Foundation
{
	/// <summary>A collection of cool static methods that should be part of the runtime environment but are not.
	/// 	</summary>
	/// <remarks>A collection of cool static methods that should be part of the runtime environment but are not.
	/// 	</remarks>
	/// <exclude></exclude>
	public class Cool
	{
		public static void SleepIgnoringInterruption(long millis)
		{
			try
			{
				Thread.Sleep(millis);
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Keeps executing a block of code until it either returns false or millisecondsTimeout
		/// elapses.
		/// </summary>
		/// <remarks>
		/// Keeps executing a block of code until it either returns false or millisecondsTimeout
		/// elapses.
		/// </remarks>
		/// <param name="millisecondsTimeout"></param>
		/// <param name="block"></param>
		public static void LoopWithTimeout(long millisecondsTimeout, IConditionalBlock block
			)
		{
			StopWatch watch = new AutoStopWatch();
			do
			{
				if (!block.Run())
				{
					break;
				}
			}
			while (watch.Peek() < millisecondsTimeout);
		}
	}
}
