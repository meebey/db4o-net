/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Util;

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

		protected static readonly string PATH = Path.Combine(Path.GetTempPath(), "test/db4oVersions"
			);

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
			return Path.Combine(PATH, FileNamePrefix() + versionName.Replace(' ', '_'));
		}

		public virtual void CreateDatabase()
		{
			CreateDatabase(FileName());
		}

		public virtual void CreateDatabaseFor(string versionName)
		{
			CreateDatabase(FileName(versionName));
		}

		private void CreateDatabase(string file)
		{
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

		public virtual void SetUp()
		{
			Configure();
			CreateDatabase();
		}

		public virtual void Test()
		{
			for (int i = 0; i < VersionNames().Length; i++)
			{
				string versionName = VersionNames()[i];
				Test(versionName);
			}
		}

		public virtual void Test(string versionName)
		{
			string testFileName = FileName(versionName);
			if (System.IO.File.Exists(testFileName))
			{
				Sharpen.Runtime.Out.WriteLine("Check database: " + testFileName);
				InvestigateFileHeaderVersion(testFileName);
				CheckDatabaseFile(testFileName);
				CheckDatabaseFile(testFileName);
			}
			else
			{
				Sharpen.Runtime.Out.WriteLine("Version upgrade check failed. File not found:" + testFileName
					);
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

		private void InvestigateFileHeaderVersion(string testFile)
		{
			_db4oHeaderVersion = VersionServices.FileHeaderVersion(testFile);
		}

		protected byte _db4oHeaderVersion;

		protected abstract string[] VersionNames();

		protected abstract string FileNamePrefix();

		protected abstract void Configure(IConfiguration config);

		protected abstract void Store(IExtObjectContainer objectContainer);

		protected abstract void AssertObjectsAreReadable(IExtObjectContainer objectContainer
			);
	}
}
