/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System.Reflection;
using Db4oAdmin.Tests.Core;

namespace Db4oAdmin.Tests.TA
{
	class TAInstrumentationTestCase : AbstractCommandLineInstrumentationTestCase
	{
		protected override string CommandLine
		{
			get { return "-ta"; }
		}

		protected override string ResourceName
		{
			get { return "TAInstrumentationSubject";  }
		}

		protected override Assembly[] Dependencies
		{
			get
			{
				return ArrayServices.Append(base.Dependencies, typeof(Db4objects.Db4o.TA.IActivatable).Assembly);
			}
		}
	}
}
