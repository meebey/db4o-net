/* Copyright (C) 2009 Versant Corporation.   http://www.db4o.com */
using Mono.Cecil;

namespace Db4oTool.Tests.TA
{
	class TAWarningOnNonPrivateFieldsTestCase : TAOutputListenerTestCaseBase
	{
		public void TestWarningOnNonPrivateFields()
		{
			AssemblyDefinition assembly = GenerateAssembly("TAClassWithNonPrivateFieldsSubject");
			InstrumentAndAssert(true, assembly.MainModule.Image.FileInformation.FullName, WarningMessageFor("TAClassWithPublicFieldSubject"), WarningMessageFor("TAClassWithProtectedFieldSubject"));
		}

		private static string WarningMessageFor(string className)
		{
			return string.Format("'{0}' has non-private fields", className);
		}
	}
}
