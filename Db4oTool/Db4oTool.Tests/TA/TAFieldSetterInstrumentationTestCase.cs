/* Copyright (C) 2009   db4objects Inc.   http://www.db4o.com */
namespace Db4oTool.Tests.TA
{
	class TAFieldSetterInstrumentationTestCase : TAInstrumentationTestCaseBase
	{
		protected override string[] Resources
		{
			get
            {
                return new string[] { "TAFieldSetterSubject", };
            }
 		}
	}
}
