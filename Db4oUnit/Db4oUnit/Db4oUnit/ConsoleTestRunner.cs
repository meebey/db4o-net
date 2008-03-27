/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit
{
	public class ConsoleTestRunner
	{
		private readonly IEnumerable _suite;

		private readonly bool _reportToFile;

		public ConsoleTestRunner(IEnumerator suite) : this(suite, true)
		{
		}

		public ConsoleTestRunner(IEnumerator suite, bool reportToFile)
		{
			if (null == suite)
			{
				throw new ArgumentException("suite");
			}
			_suite = Iterators.Iterable(suite);
			_reportToFile = reportToFile;
		}

		public ConsoleTestRunner(IEnumerable suite) : this(suite, true)
		{
		}

		public ConsoleTestRunner(IEnumerable suite, bool reportToFile)
		{
			if (null == suite)
			{
				throw new ArgumentException("suite");
			}
			_suite = suite;
			_reportToFile = reportToFile;
		}

		public ConsoleTestRunner(Type clazz) : this(new ReflectionTestSuiteBuilder(clazz)
			)
		{
		}

		public virtual int Run()
		{
			return Run(TestPlatform.GetStdErr());
		}

		public virtual int Run(TextWriter writer)
		{
			TestResult result = new TestResult();
			new TestRunner(_suite).Run(new CompositeTestListener(new ConsoleListener(writer), 
				result));
			ReportResult(result, writer);
			return result.Failures.Size();
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
