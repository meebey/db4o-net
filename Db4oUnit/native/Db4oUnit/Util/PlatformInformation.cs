/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */
namespace Db4oUnit.Util
{
	public class PlatformInformation
	{
		public static bool IsJava
		{
			get { return false; }
		}

		public static bool IsDotNet
		{
			get { return true; }
		}
	}
}
