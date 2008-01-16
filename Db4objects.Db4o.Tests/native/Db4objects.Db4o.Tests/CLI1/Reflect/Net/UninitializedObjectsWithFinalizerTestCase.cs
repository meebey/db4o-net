using System;
using System.Collections.Generic;
using System.IO;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI1.Reflect.Net
{
	class UninitializedObjectsWithFinalizerTestCase : ITestCase
	{
#if !CF_2_0
		private const string DatabaseFileName = "TestUnitializedObjects.odb";
		public void TestUninitilizedObjects()
		{
			bool exceptionNotThrown = true;
			try
			{
				File.Delete(DatabaseFileName);

				using (IObjectContainer db = Db4oFactory.OpenFile(DatabaseFileName))
				{
					db.Store(new TestSubject("Test"));
					
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}

				using (IObjectContainer db = Db4oFactory.OpenFile(DatabaseFileName))
				{
					IList <TestSubject> result = db.Query<TestSubject>();
					
					Assert.AreEqual(1, result.Count);
					db.Activate(result[0], 2);
					Assert.AreEqual("Test", result[0].name);
                }

				File.Delete(DatabaseFileName);
			}
			catch
			{
				exceptionNotThrown = false;
			}

			Assert.IsTrue(exceptionNotThrown);
		}
	}

	internal class TestSubject
	{
		public string name;

		public TestSubject(string _name)
		{
			name = _name;
		}

		~TestSubject()
		{
            // Just access an object method...
            name = name.ToUpper();
		}
#endif
    }
}
