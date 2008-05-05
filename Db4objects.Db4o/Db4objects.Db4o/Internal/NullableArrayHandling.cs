/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class NullableArrayHandling
	{
		private static bool enabled = false;

		public static bool UseJavaHandling()
		{
			return enabled;
			return true;
		}

		public static bool UseOldNetHandling()
		{
			return !UseJavaHandling();
		}

		public static bool Disabled()
		{
			return !enabled;
		}

		public static bool Enabled()
		{
			return enabled;
		}
	}
}
