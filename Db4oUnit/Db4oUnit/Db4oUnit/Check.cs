/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4oUnit
{
	/// <summary>
	/// Utility class to enable the reuse of object comparison and checking
	/// methods without asserting.
	/// </summary>
	/// <remarks>
	/// Utility class to enable the reuse of object comparison and checking
	/// methods without asserting.
	/// </remarks>
	public class Check
	{
		public static bool ObjectsAreEqual(object expected, object actual)
		{
			return expected == actual || (expected != null && actual != null && expected.Equals
				(actual));
		}
	}
}
