using System;
using System.Collections.Generic;
using System.IO;
using Db4objects.Db4o.Config;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI1.Reflect.Net
{
	class UninitializedObjectsWithFinalizerTestCase : ITestCase
	{
#if !CF
		public void TestUninitilizedObjects()
		{
			string databaseFileName = Path.GetTempFileName();
			File.Delete(databaseFileName);

			using (IObjectContainer db = Db4oFactory.OpenFile(NewConfiguration(), databaseFileName))
			{
				db.Store(new TestSubject("Test"));
				
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			using (IObjectContainer db = Db4oFactory.OpenFile(NewConfiguration(), databaseFileName))
			{
				IList <TestSubject> result = db.Query<TestSubject>();
				
				Assert.AreEqual(1, result.Count);
				db.Activate(result[0], 2);
				Assert.AreEqual("Test", result[0].name);
            }

			File.Delete(databaseFileName);
		}

		private static IConfiguration NewConfiguration()
		{
			return Db4oFactory.NewConfiguration();
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
