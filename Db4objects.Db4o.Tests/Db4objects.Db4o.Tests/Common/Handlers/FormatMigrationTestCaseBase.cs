/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public abstract class FormatMigrationTestCaseBase : ITestLifeCycle
	{
		public virtual void Configure()
		{
			IConfiguration config = Db4oFactory.Configure();
			Reflection4.Invoke(config, "allowVersionUpdates", new Type[] { typeof(bool) }, new 
				object[] { true });
			Configure(config);
		}

		protected static readonly string PATH = "./test/db4oVersions/";

		protected virtual string FileName()
		{
			return FileName(Db4oFactory.Version());
		}

		protected virtual string FileName(string versionName)
		{
			return OldVersionFileName(versionName) + ".yap";
		}

		protected virtual string OldVersionFileName(string versionName)
		{
			return PATH + FileNamePrefix() + versionName.Replace(' ', '_');
		}

		public virtual void SetUp()
		{
			Configure();
			string file = FileName();
			System.IO.Directory.CreateDirectory(PATH);
			if (System.IO.File.Exists(file))
			{
				File4.Delete(file);
			}
			IExtObjectContainer objectContainer = Db4oFactory.OpenFile(file).Ext();
			try
			{
				Store(objectContainer);
			}
			finally
			{
				objectContainer.Close();
			}
		}

		public virtual void Test()
		{
			for (int i = 0; i < VersionNames().Length; i++)
			{
				string fileName = OldVersionFileName(VersionNames()[i]);
				if (System.IO.File.Exists(fileName))
				{
					string testFileName = FileName(VersionNames()[i]);
					File4.Delete(testFileName);
					File4.Copy(fileName, testFileName);
					CheckDatabaseFile(testFileName);
					CheckDatabaseFile(testFileName);
				}
				else
				{
					Sharpen.Runtime.Err.WriteLine("Version upgrade check failed. File not found:");
					Sharpen.Runtime.Err.WriteLine(fileName);
				}
			}
		}

		public virtual void TearDown()
		{
		}

		private void CheckDatabaseFile(string testFile)
		{
			Configure();
			IExtObjectContainer objectContainer = Db4oFactory.OpenFile(testFile).Ext();
			try
			{
				AssertObjectsAreReadable(objectContainer);
			}
			finally
			{
				objectContainer.Close();
			}
		}

		protected abstract string[] VersionNames();

		protected abstract string FileNamePrefix();

		protected abstract void Configure(IConfiguration config);

		protected abstract void Store(IExtObjectContainer objectContainer);

		protected abstract void AssertObjectsAreReadable(IExtObjectContainer objectContainer
			);
	}
}
