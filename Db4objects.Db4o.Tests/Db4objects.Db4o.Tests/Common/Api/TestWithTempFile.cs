/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o.Foundation.IO;

namespace Db4objects.Db4o.Tests.Common.Api
{
	public class TestWithTempFile : ITestLifeCycle
	{
		private readonly string _tempFile = Path.GetTempFileName();

		protected virtual string TempFile()
		{
			return _tempFile;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
			File4.Delete(TempFile());
		}
	}
}
