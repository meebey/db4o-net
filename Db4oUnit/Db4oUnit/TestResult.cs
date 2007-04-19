using System;
using System.IO;
using Db4oUnit;
using Db4oUnit.Util;

namespace Db4oUnit
{
	public class TestResult : Printable
	{
		private TestFailureCollection _failures = new TestFailureCollection();

		private int _testCount = 0;

		private readonly StopWatch _watch = new StopWatch();

		private readonly TextWriter _stdout;

		public TestResult(bool printLabels)
		{
			_stdout = printLabels ? TestPlatform.GetStdOut() : null;
		}

		public TestResult() : this(false)
		{
		}

		public virtual void TestStarted(ITest test)
		{
			++_testCount;
			Print(test.GetLabel());
		}

		public virtual void TestFailed(ITest test, Exception failure)
		{
			PrintFailure(failure);
			_failures.Add(new TestFailure(test, failure));
		}

		private void PrintFailure(Exception failure)
		{
			if (failure == null)
			{
				Print("\t!");
			}
			else
			{
				Print("\t! " + failure.Message);
			}
		}

		public virtual bool Green()
		{
			return _failures.Size() == 0;
		}

		public virtual TestFailureCollection Failures()
		{
			return _failures;
		}

		public override void Print(TextWriter writer)
		{
			if (Green())
			{
				writer.Write("GREEN (" + _testCount + " tests) - " + ElapsedString() + "\n");
				return;
			}
			writer.Write("RED (" + _failures.Size() + " out of " + _testCount + " tests failed) - "
				 + ElapsedString() + "\n");
			_failures.Print(writer);
		}

		private string ElapsedString()
		{
			return _watch.ToString();
		}

		public virtual int Assertions()
		{
			return 0;
		}

		public virtual void RunStarted()
		{
			_watch.Start();
		}

		public virtual void RunFinished()
		{
			_watch.Stop();
		}

		private void Print(string message)
		{
			if (null != _stdout)
			{
				try
				{
					_stdout.Write(message + "\n");
					_stdout.Flush();
				}
				catch (IOException)
				{
				}
			}
		}
	}
}
