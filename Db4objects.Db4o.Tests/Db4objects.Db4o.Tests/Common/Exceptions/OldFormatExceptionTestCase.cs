/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Exceptions;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	/// <exclude></exclude>
	public class OldFormatExceptionTestCase : ITestCase, IOptOutNoFileSystemData
	{
		public static void Main(string[] args)
		{
			new TestRunner(typeof(OldFormatExceptionTestCase)).Run();
		}

		public virtual void Test()
		{
			if (WorkspaceServices.WorkspaceRoot == null)
			{
				Sharpen.Runtime.Err.WriteLine("Build environment not available. Skipping test case..."
					);
				return;
			}
			Db4oFactory.Configure().ReflectWith(Platform4.ReflectorForType(typeof(OldFormatExceptionTestCase)
				));
			Db4oFactory.Configure().AllowVersionUpdates(false);
			string oldDatabaseFilePath = OldDatabaseFilePath();
			Assert.Expect(typeof(OldFormatException), new _ICodeBlock_39(this, oldDatabaseFilePath
				));
			Db4oFactory.Configure().AllowVersionUpdates(true);
			IObjectContainer container = null;
			try
			{
				container = Db4oFactory.OpenFile(oldDatabaseFilePath);
			}
			finally
			{
				if (container != null)
				{
					container.Close();
				}
			}
		}

		private sealed class _ICodeBlock_39 : ICodeBlock
		{
			public _ICodeBlock_39(OldFormatExceptionTestCase _enclosing, string oldDatabaseFilePath
				)
			{
				this._enclosing = _enclosing;
				this.oldDatabaseFilePath = oldDatabaseFilePath;
			}

			public void Run()
			{
				Db4oFactory.OpenFile(oldDatabaseFilePath);
			}

			private readonly OldFormatExceptionTestCase _enclosing;

			private readonly string oldDatabaseFilePath;
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
