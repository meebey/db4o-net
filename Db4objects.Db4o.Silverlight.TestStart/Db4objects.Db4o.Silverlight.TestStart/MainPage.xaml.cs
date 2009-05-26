/* Copyright (C) 2009 Versant Inc.   http://www.db4o.com */
using System;
using System.Threading;
using System.Windows.Browser;
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

		private static Db4oTestSuiteBuilder SoloSuite(bool independentConfig, params Type[] testCases)
		{
			return new Db4oTestSuiteBuilder(Db4oFixtures.NewSolo(independentConfig), testCases);
		}
	}
}
