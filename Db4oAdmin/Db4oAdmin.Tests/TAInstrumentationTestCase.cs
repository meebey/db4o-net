
using System;
using System.Collections.Generic;
using System.Reflection;
using Db4oUnit;

namespace Db4oAdmin.Tests
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
