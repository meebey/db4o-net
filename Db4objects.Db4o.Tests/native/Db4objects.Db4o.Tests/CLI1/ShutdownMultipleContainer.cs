/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;

using Db4objects.Db4o;

using Db4oUnit;

namespace Db4objects.Db4o.Tests
{
#if !CF
	public class ShutdownMultipleContainer : ITestLifeCycle
	{
		private static readonly string _firstFile = "first.db4o";
		private static readonly string _secondFile = "second.db4o";

		public class Runner : MarshalByRefObject
		{
			public void Run(TextWriter err)
			{
				Console.SetError(err);
				Db4oFactory.OpenFile(_firstFile);
				Db4oFactory.OpenFile(_secondFile);
			}
		}

		public void Test()
		{
			TextWriter stderr = Console.Error;
			StringWriter output = new StringWriter();
			try
			{
				Console.SetError(output);
				RunTestInAnotherDomain();
				CheckOutput(output.ToString());
			}
			finally
			{
				Console.SetError(stderr);
			}
		}

		void CheckOutput(string output)
		{
			Assert.AreNotEqual(-1, output.IndexOf(_firstFile));
			Assert.AreNotEqual(-1, output.IndexOf(_secondFile));
		}

		void RunTestInAnotherDomain()
		{
			AppDomain testDomain = AppDomain.CreateDomain("testDomain");

			Runner r = (Runner)testDomain.CreateInstanceAndUnwrap(
				GetType().Assembly.FullName, typeof(Runner).FullName);

			r.Run(Console.Error);

			AppDomain.Unload(testDomain);
		}

		public void SetUp()
		{
		}

		public void TearDown()
		{
			Clean(_firstFile);
			Clean(_secondFile);
		}

		static void Clean(string file)
		{
			if (File.Exists(file)) File.Delete(file);
		}
	}
#endif
}
