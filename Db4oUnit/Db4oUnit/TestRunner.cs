/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;

namespace Db4oUnit
{
	public class TestRunner
	{
		private ITestSuiteBuilder _suiteBuilder;

		public TestRunner(TestSuite suite)
		{
			if (null == suite)
			{
				throw new ArgumentException("suite");
			}
			_suiteBuilder = new NullTestSuiteBuilder(suite);
		}

		public TestRunner(ITestSuiteBuilder builder)
		{
			if (null == builder)
			{
				throw new ArgumentException("suite");
			}
			_suiteBuilder = builder;
		}

		public TestRunner(Type clazz) : this(new ReflectionTestSuiteBuilder(clazz))
		{
		}

		public virtual int Run()
		{
			return Run(true);
		}

		private int Run(bool printLabels)
		{
			TestSuite suite = BuildTestSuite();
			if (null == suite)
			{
				return 1;
			}
			TestResult result = new TestResult(printLabels);
			result.RunStarted();
			suite.Run(result);
			result.RunFinished();
			Report(result);
			return result.Failures().Size();
		}

		private TestSuite BuildTestSuite()
		{
			try
			{
				return _suiteBuilder.Build();
			}
			catch (Exception x)
			{
				Report(x);
			}
			return null;
		}

		private void Report(Exception x)
		{
			TestPlatform.PrintStackTrace(TestPlatform.GetStdOut(), x);
		}

		private void Report(TestResult result)
		{
			ReportToTextFile(result);
			ReportToStdErr(result);
		}

		private void ReportToTextFile(TestResult result)
		{
			try
			{
				TextWriter writer = TestPlatform.OpenTextFile("db4ounit.log");
				try
				{
					Report(writer, result);
				}
				finally
				{
					writer.Close();
				}
			}
			catch (IOException e)
			{
				Report(e);
			}
		}

		private void ReportToStdErr(TestResult result)
		{
			Report(TestPlatform.GetStdErr(), result);
		}

		private void Report(TextWriter writer, TestResult result)
		{
			try
			{
				result.Print(writer);
				writer.Flush();
			}
			catch (IOException e)
			{
				Report(e);
			}
		}
	}
}
