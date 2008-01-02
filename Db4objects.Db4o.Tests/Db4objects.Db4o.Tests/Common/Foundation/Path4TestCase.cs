/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o.Foundation.IO;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class Path4TestCase : ITestCase
	{
		public virtual void TestGetTempFileName()
		{
			string tempFileName = Path.GetTempFileName();
			Assert.IsTrue(System.IO.File.Exists(tempFileName));
		}
	}
}
