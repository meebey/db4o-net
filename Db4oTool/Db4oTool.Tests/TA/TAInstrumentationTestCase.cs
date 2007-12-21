/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System.Reflection;
using Db4oTool.Tests.Core;

namespace Db4oTool.Tests.TA
{
	class TAInstrumentationTestCase : AbstractCommandLineInstrumentationTestCase
	{
		protected override string CommandLine
		{
			get
			{
				return "-ta -by-name:FilteredOut -not";
			}
		}

		protected override string[] Resources
		{
			get
            {
                return new string[]
                {
                	"TAInstrumentationSubject",
					"TAAssemblyReferenceSubject",
                    "TAFieldSetterSubject",
				};
            }
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
