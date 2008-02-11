/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;

namespace Db4oUnit
{
	public class TestRunner
	{
		private ITestSuiteBuilder _suiteBuilder;

		private bool _reportToFile = true;

		public TestRunner(TestSuite suite) : this(suite, true)
		{
		}

		public TestRunner(TestSuite suite, bool reportToFile)
		{
			if (null == suite)
			{
				throw new ArgumentException("suite");
			}
			_suiteBuilder = new NullTestSuiteBuilder(suite);
			_reportToFile = reportToFile;
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
			return Run(TestPlatform.GetStdErr());
		}

		public virtual int Run(TextWriter writer)
		{
			TestSuite suite = BuildTestSuite();
			if (null == suite)
			{
				return 1;
			}
			TestResult result = new TestResult(writer);
			result.RunStarted();
			suite.Run(result);
			result.RunFinished();
			ReportResult(result, writer);
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
			TestPlatform.PrintStackTrace(TestPlatform.GetStdErr(), x);
		}

		private void ReportResult(TestResult result, TextWriter writer)
		{
			if (_reportToFile)
			{
				ReportToTextFile(result);
			}
			Report(result, writer);
		}

		private void ReportToTextFile(TestResult result)
		{
			try
			{
				TextWriter writer = TestPlatform.OpenTextFile("db4ounit.log");
				try
				{
					Report(result, writer);
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

		private void Report(TestResult result, TextWriter writer)
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
