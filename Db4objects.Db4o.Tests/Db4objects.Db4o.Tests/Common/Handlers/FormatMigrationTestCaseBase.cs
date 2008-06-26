/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public abstract partial class FormatMigrationTestCaseBase : ITestLifeCycle, IOptOutNoFileSystemData
	{
		private string _db4oVersion;

		public virtual void Configure()
		{
			IConfiguration config = Db4oFactory.Configure();
			config.AllowVersionUpdates(true);
			ConfigureForTest(config);
		}

		protected static readonly string TempPath;

		static FormatMigrationTestCaseBase()
		{
			TempPath = Path.Combine(GetTempPath(), "test/db4oVersions");
		}

		protected virtual string FileName()
		{
			_db4oVersion = Db4oVersion.Name;
			return FileName(_db4oVersion);
		}

		protected virtual string FileName(string versionName)
		{
			return OldVersionFileName(versionName) + ".yap";
		}

		protected virtual string OldVersionFileName(string versionName)
		{
			return Path.Combine(TempPath, FileNamePrefix() + versionName.Replace(' ', '_'));
		}

		public virtual void CreateDatabase()
		{
			CreateDatabase(FileName());
		}

		public virtual void CreateDatabaseFor(string versionName)
		{
			_db4oVersion = versionName;
			IConfiguration config = Db4oFactory.Configure();
			try
			{
				ConfigureForStore(config);
			}
			catch
			{
			}
			// Some old database engines may throw NoSuchMethodError
			// for configuration methods they don't know yet. Ignore,
			// but tell the implementor:
			// System.out.println("Exception in configureForStore for " + versionName + " in " + getClass().getName());
			CreateDatabase(FileName(versionName));
		}

		private void CreateDatabase(string file)
		{
			System.IO.Directory.CreateDirectory(TempPath);
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

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			Configure();
			CreateDatabase();
		}

		/// <exception cref="IOException"></exception>
		public virtual void Test()
		{
			for (int i = 0; i < VersionNames().Length; i++)
			{
				string versionName = VersionNames()[i];
				Test(versionName);
			}
		}

		/// <exception cref="IOException"></exception>
		public virtual void Test(string versionName)
		{
			_db4oVersion = versionName;
			string testFileName = FileName(versionName);
			if (System.IO.File.Exists(testFileName))
			{
				//		    System.out.println("Check database: " + testFileName);
				InvestigateFileHeaderVersion(testFileName);
				RunDefrag(testFileName);
				CheckDatabaseFile(testFileName);
				// Twice, to ensure everything is fine after opening, converting and closing.
				CheckDatabaseFile(testFileName);
				UpdateDatabaseFile(testFileName);
				CheckUpdatedDatabaseFile(testFileName);
			}
			else
			{
				Sharpen.Runtime.Out.WriteLine("Version upgrade check failed. File not found:" + testFileName
					);
			}
		}

		// FIXME: The following fails the CC build since not all files are there on .NET.
		//        Change back when we have all files.
		// Assert.fail("Version upgrade check failed. File not found:" + testFileName);
		/// <exception cref="IOException"></exception>
		private void RunDefrag(string testFileName)
		{
			Db4oFactory.Configure().AllowVersionUpdates(true);
			IObjectContainer oc = Db4oFactory.OpenFile(testFileName);
			oc.Close();
			string backupFileName = Path.GetTempFileName();
			try
			{
				DefragmentConfig defragConfig = new DefragmentConfig(testFileName, backupFileName
					);
				defragConfig.ForceBackupDelete(true);
				defragConfig.ReadOnly(!DefragmentInReadWriteMode());
				Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig);
			}
			finally
			{
				File4.Delete(backupFileName);
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
		}

		// do nothing
		private void CheckDatabaseFile(string testFile)
		{
			WithDatabase(testFile, new _IFunction4_154(this));
		}

		private sealed class _IFunction4_154 : IFunction4
		{
			public _IFunction4_154(FormatMigrationTestCaseBase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object objectContainer)
			{
				this._enclosing.AssertObjectsAreReadable((IExtObjectContainer)objectContainer);
				return null;
			}

			private readonly FormatMigrationTestCaseBase _enclosing;
		}

		private void UpdateDatabaseFile(string testFile)
		{
			WithDatabase(testFile, new _IFunction4_163(this));
		}

		private sealed class _IFunction4_163 : IFunction4
		{
			public _IFunction4_163(FormatMigrationTestCaseBase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object objectContainer)
			{
				this._enclosing.Update((IExtObjectContainer)objectContainer);
				return null;
			}

			private readonly FormatMigrationTestCaseBase _enclosing;
		}

		private void CheckUpdatedDatabaseFile(string testFile)
		{
			WithDatabase(testFile, new _IFunction4_173(this));
		}

		private sealed class _IFunction4_173 : IFunction4
		{
			public _IFunction4_173(FormatMigrationTestCaseBase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object objectContainer)
			{
				this._enclosing.AssertObjectsAreUpdated((IExtObjectContainer)objectContainer);
				return null;
			}

			private readonly FormatMigrationTestCaseBase _enclosing;
		}

		private void WithDatabase(string file, IFunction4 function)
		{
			Configure();
			IExtObjectContainer objectContainer = Db4oFactory.OpenFile(file).Ext();
			try
			{
				function.Apply(objectContainer);
			}
			finally
			{
				objectContainer.Close();
			}
		}

		/// <exception cref="IOException"></exception>
		private void InvestigateFileHeaderVersion(string testFile)
		{
			_db4oHeaderVersion = VersionServices.FileHeaderVersion(testFile);
		}

		protected virtual int Db4oMajorVersion()
		{
			if (_db4oVersion != null)
			{
				return System.Convert.ToInt32(Sharpen.Runtime.Substring(_db4oVersion, 0, 1));
			}
			return System.Convert.ToInt32(Sharpen.Runtime.Substring(Db4oFactory.Version(), 5, 
				6));
		}

		private byte _db4oHeaderVersion;

		protected virtual string[] VersionNames()
		{
			return new string[] { Sharpen.Runtime.Substring(Db4oFactory.Version(), 5) };
		}

		protected abstract string FileNamePrefix();

		protected virtual void ConfigureForTest(IConfiguration config)
		{
		}

		// Override for special testing configuration.
		protected virtual void ConfigureForStore(IConfiguration config)
		{
		}

		// Override for special storage configuration.
		protected abstract void Store(IExtObjectContainer objectContainer);

		protected virtual void StoreObject(IExtObjectContainer objectContainer, object obj
			)
		{
			// code MUST use the deprecated API here
			// because it will be run against old db4o versions
			objectContainer.Set(obj);
		}

		protected abstract void AssertObjectsAreReadable(IExtObjectContainer objectContainer
			);

		protected virtual byte Db4oHeaderVersion()
		{
			return _db4oHeaderVersion;
		}

		protected virtual void Update(IExtObjectContainer objectContainer)
		{
		}

		// Override to do updates also
		protected virtual void AssertObjectsAreUpdated(IExtObjectContainer objectContainer
			)
		{
		}

		// Override to check updates also
		/// <summary>override and return true for database updates that produce changed class metadata
		/// 	</summary>
		protected virtual bool DefragmentInReadWriteMode()
		{
			return false;
		}
	}
}
