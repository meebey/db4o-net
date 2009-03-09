/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class DbPathDoesNotExistTestCase : ITestCase
	{
		public virtual void Test()
		{
			string tempPath = Path.GetTempPath();
			string nonExistantPath = Path.Combine(tempPath, "/folderdoesnotexistneverever/filedoesnotexist.db4o"
				);
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_16(nonExistantPath));
		}

		private sealed class _ICodeBlock_16 : ICodeBlock
		{
			public _ICodeBlock_16(string nonExistantPath)
			{
				this.nonExistantPath = nonExistantPath;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oEmbedded.OpenFile(Db4oEmbedded.NewConfiguration(), nonExistantPath);
			}

			private readonly string nonExistantPath;
		}
	}
}
