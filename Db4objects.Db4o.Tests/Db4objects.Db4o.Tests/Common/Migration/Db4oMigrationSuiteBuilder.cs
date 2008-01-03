/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Tests.Common.Migration;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class Db4oMigrationSuiteBuilder : ReflectionTestSuiteBuilder
	{
		/// <summary>Runs the tests against all archived libraries + the current one</summary>
		public static readonly string[] All = null;

		/// <summary>Runs the tests against the current version only.</summary>
		/// <remarks>Runs the tests against the current version only.</remarks>
		public static readonly string[] Current = new string[0];

		private readonly Db4oLibraryEnvironmentProvider _environmentProvider = new Db4oLibraryEnvironmentProvider
			(PathProvider.TestCasePath());

		private readonly string[] _specificLibraries;

		/// <summary>
		/// Creates a suite builder for the specific FormatMigrationTestCaseBase derived classes
		/// and specific db4o libraries.
		/// </summary>
		/// <remarks>
		/// Creates a suite builder for the specific FormatMigrationTestCaseBase derived classes
		/// and specific db4o libraries. If no libraries are specified (either null or empty array)
		/// <see cref="Db4oLibrarian.Libraries">Db4oLibrarian.Libraries</see>
		/// is used to find archived libraries.
		/// </remarks>
		/// <param name="classes"></param>
		/// <param name="specificLibraries"></param>
		public Db4oMigrationSuiteBuilder(Type[] classes, string[] specificLibraries) : base
			(classes)
		{
			_specificLibraries = specificLibraries;
		}

		protected override TestSuite FromClass(Type clazz)
		{
			AssertMigrationTestCase(clazz);
			TestSuite defaultTestSuite = base.FromClass(clazz);
			try
			{
				TestSuite migrationTestSuite = MigrationTestSuite(clazz, Db4oLibraries());
				return new TestSuite(new ITest[] { migrationTestSuite, defaultTestSuite });
			}
			catch (Exception e)
			{
				return new TestSuite(new ITest[] { new FailingTest(clazz.FullName, e), defaultTestSuite
					 });
			}
		}

		/// <exception cref="Exception"></exception>
		private TestSuite MigrationTestSuite(Type clazz, Db4oLibrary[] libraries)
		{
			ITest[] migrationTests = new ITest[libraries.Length];
			for (int i = 0; i < libraries.Length; i++)
			{
				migrationTests[i] = MigrationTest(libraries[i], clazz);
			}
			return new TestSuite(migrationTests);
		}

		/// <exception cref="Exception"></exception>
		private Db4oMigrationSuiteBuilder.Db4oMigrationTest MigrationTest(Db4oLibrary library
			, Type clazz)
		{
			FormatMigrationTestCaseBase instance = (FormatMigrationTestCaseBase)NewInstance(clazz
				);
			return new Db4oMigrationSuiteBuilder.Db4oMigrationTest(instance, library);
		}

		/// <exception cref="Exception"></exception>
		private Db4oLibrary[] Db4oLibraries()
		{
			if (HasSpecificLibraries())
			{
				return SpecificLibraries();
			}
			return Librarian().Libraries();
		}

		/// <exception cref="Exception"></exception>
		private Db4oLibrary[] SpecificLibraries()
		{
			Db4oLibrary[] libraries = new Db4oLibrary[_specificLibraries.Length];
			for (int i = 0; i < libraries.Length; i++)
			{
				libraries[i] = Librarian().ForFile(_specificLibraries[i]);
			}
			return libraries;
		}

		private bool HasSpecificLibraries()
		{
			return null != _specificLibraries;
		}

		private Db4oLibrarian Librarian()
		{
			return new Db4oLibrarian(_environmentProvider);
		}

		private void AssertMigrationTestCase(Type clazz)
		{
			if (!typeof(FormatMigrationTestCaseBase).IsAssignableFrom(clazz))
			{
				throw new ArgumentException();
			}
		}

		private sealed class Db4oMigrationTest : TestAdapter
		{
			private readonly FormatMigrationTestCaseBase _test;

			private readonly Db4oLibrary _library;

			private readonly string _version;

			/// <exception cref="Exception"></exception>
			public Db4oMigrationTest(FormatMigrationTestCaseBase test, Db4oLibrary library)
			{
				_library = library;
				_test = test;
				_version = Environment().Version();
			}

			public override string GetLabel()
			{
				return "[" + _version + "] " + _test.GetType().FullName;
			}

			/// <exception cref="Exception"></exception>
			protected override void RunTest()
			{
				CreateDatabase();
				Test();
			}

			/// <exception cref="IOException"></exception>
			private void Test()
			{
				_test.Test(_version);
			}

			/// <exception cref="Exception"></exception>
			private void CreateDatabase()
			{
				Environment().InvokeInstanceMethod(_test.GetType(), "createDatabaseFor", new object
					[] { _version });
			}

			private Db4oLibraryEnvironment Environment()
			{
				return _library.environment;
			}
		}
	}
}
