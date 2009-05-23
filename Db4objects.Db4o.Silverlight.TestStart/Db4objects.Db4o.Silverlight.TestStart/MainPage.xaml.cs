using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Browser;
using System.Windows.Threading;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;

namespace Db4objects.Db4o.Silverlight.TestStart
{
	public partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
			ThreadPool.QueueUserWorkItem(unused => TestRunnerEntry());
		}

		private void TestRunnerEntry()
		{
			try
			{
				//Type[] testCases = new[] { typeof(Db4objects.Db4o.Tests.Silverlight.AllTests) };
				Type[] testCases = new[] { typeof(Tests.Common.AllTests) };
				new TestRunner(SoloSuite(true, testCases)).Run(new SilverlightTestListener(Dispatcher));
			}
			catch(Exception ex)
			{
				AppendException(ex);
			}
		}

		private void AppendException(Exception exception)
		{
			Dispatcher.BeginInvoke(() => HtmlPage.Window.Eval("appendException(\"" + exception.ToJScriptString() + "\");"));
		}

		private Db4oTestSuiteBuilder SoloSuite(bool independentConfig, params Type[] testCases)
		{
			return new Db4oTestSuiteBuilder(Db4oFixtures.NewSolo(independentConfig), testCases);
		}
	}

	public class SilverlightTestListener : ITestListener
	{
		public void RunStarted()
		{
			Run("append", "Tests started...");
			_start = DateTime.Now;
		}

		public void TestStarted(ITest test)
		{
			++_testCount;
			NewTest(_testCount + ")" + test.Label());
		}

		public void TestFailed(ITest test, Exception failure)
		{
			_errorCount++;
			_failures.Add(new TestFailure(test.Label(), failure));
			MarkLastAsError();
		}

		public void RunFinished()
		{
			AppendTestsResult();
			Append(FailuresMessage());
		}

		private void Append(string message)
		{
			Run("append", message);
		}

		private void AppendTestsResult()
		{
			Run("appendTestsResult", _testCount,  _errorCount, DateTime.Now.Subtract(_start).TotalSeconds);
			//Dispatch(() => HtmlPage.Window.Eval("appendTestsResult(" + _testCount  + "," + _errorCount + ", " + DateTime.Now.Subtract(_start).TotalSeconds + ");"));
		}

		private void NewTest(string message)
		{
			_latestAppended = Run("newTest", message);
		}

		private static string RemoveExtraCommaAtStart(string arguments)
		{
			return arguments.Remove(0, 1);
		}

		private void Dispatch(Action action)
		{
			_dispatcher.BeginInvoke( action);
		}

		private void MarkLastAsError()
		{
			_dispatcher.BeginInvoke(() => HtmlPage.Window.Invoke("markAsFailure", _latestAppended));
		}

		private string FailuresMessage()
		{
			int count = 1;
			return
				_failures.Aggregate(new StringBuilder(),
				                    (acc, failure) =>
				                    acc.AppendFormat("{0}) {1} {2}<br /><br />", count++, failure.TestLabel, failure.Reason)).Replace("\r\n", "<br />").ToJScriptString();
		}
		
		private object Run(string functionName, params object[] args)
		{
			Dispatch(delegate
			         	{
			         		object result = HtmlPage.Window.Eval(functionName + "(" + ToStringArgumentList(args) + ");");
							if (result != null)
							{
								_latestAppended = result;
							}
			         	});
			return _latestAppended;
		}

		private static string ToStringArgumentList(IEnumerable<object> objects)
		{
			string arguments = objects.Aggregate("", (acc, current) => acc + "," + AddQuotes(current.ToJScriptString()));
			return RemoveExtraCommaAtStart(arguments);
		}

		private static string AddQuotes(object item)
		{
			return "\"" + item + "\"";
		}

		private object _latestAppended;
		private int _testCount;
		private int _errorCount;
		private DateTime _start;
		private readonly IList<TestFailure> _failures = new List<TestFailure>();
		private readonly Dispatcher _dispatcher;

		public SilverlightTestListener(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}
	}
}
