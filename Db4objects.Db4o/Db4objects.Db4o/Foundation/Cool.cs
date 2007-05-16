/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
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
	}
}
