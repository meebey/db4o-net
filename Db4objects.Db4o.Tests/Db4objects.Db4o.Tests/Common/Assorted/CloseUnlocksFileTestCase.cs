/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class CloseUnlocksFileTestCase : ITestCase
	{
		private static readonly string File = Path.GetTempFileName();

		public virtual void Test()
		{
			File4.Delete(File);
			Assert.IsFalse(System.IO.File.Exists(File));
			IObjectContainer oc = Db4oFactory.OpenFile(File);
			oc.Close();
			File4.Delete(File);
			Assert.IsFalse(System.IO.File.Exists(File));
		}
	}
}
