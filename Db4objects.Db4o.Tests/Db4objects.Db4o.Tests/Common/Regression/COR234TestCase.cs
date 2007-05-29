/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Regression;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Regression
{
	/// <exclude></exclude>
	public class COR234TestCase : ITestCase, IOptOutNoFileSystemData
	{
		public virtual void Test()
		{
			if (WorkspaceServices.WorkspaceRoot == null)
			{
				Sharpen.Runtime.Err.WriteLine("Build environment not available. Skipping test case..."
					);
				return;
			}
			Db4oFactory.Configure().AllowVersionUpdates(false);
			Db4oFactory.Configure().ReflectWith(Platform4.ReflectorForType(typeof(COR234TestCase)
				));
			Assert.Expect(typeof(OldFormatException), new _ICodeBlock_32(this));
		}

		private sealed class _ICodeBlock_32 : ICodeBlock
		{
			public _ICodeBlock_32(COR234TestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oFactory.OpenFile(this._enclosing.OldDatabaseFilePath());
			}

			private readonly COR234TestCase _enclosing;
		}

		protected virtual string OldDatabaseFilePath()
		{
			string oldFile = IOServices.BuildTempPath("old_db.yap");
			File4.Copy(WorkspaceServices.WorkspaceTestFilePath("db4oVersions/db4o_3.0.3"), oldFile
				);
			return oldFile;
		}
	}
}
