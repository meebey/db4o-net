﻿/* Copyright (C) 2004-2007   db4objects Inc.   http://www.db4o.com */

using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.TA.Tests
{
	class MainTestSuite : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new MainTestSuite().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(AllTests),
					typeof(CLI1.AllTests),
				};
		}
	}
}
