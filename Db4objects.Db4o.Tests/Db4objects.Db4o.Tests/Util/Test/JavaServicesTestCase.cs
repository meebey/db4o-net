/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Util.Test
{
	public class JavaServicesTestCase : ITestCase
	{
		public class ShortProgram
		{
			public static readonly string OUTPUT = "XXshortXX";

			public static void Main(string[] arguments)
			{
				Sharpen.Runtime.Out.WriteLine(OUTPUT);
			}
		}

		public class LongProgram
		{
			public static readonly string OUTPUT = "XXlongXX";

			public static void Main(string[] arguments)
			{
				Sharpen.Runtime.Out.WriteLine(OUTPUT);
				try
				{
					Thread.Sleep(long.MaxValue);
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
