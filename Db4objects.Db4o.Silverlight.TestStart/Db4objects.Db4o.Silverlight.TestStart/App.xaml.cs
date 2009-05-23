using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Browser;
using Db4oUnit.Extensions;
using Sharpen.Lang;

namespace Db4objects.Db4o.Silverlight.TestStart
{
	public partial class App
	{
		private static readonly IDictionary<string, Assembly> _cache = new Dictionary<string, Assembly>();
		static App()
		{
			AddAssemblyToCache(typeof(Db4oFactory));
			AddAssemblyToCache(typeof(Tests.AllTests));
			AddAssemblyToCache(typeof(Int32));
			AddAssemblyToCache(typeof(AbstractDb4oTestCase));

			TypeReference.AssemblyResolve += (sender, args) =>
			                                 	{
													if (!_cache.ContainsKey(args.Name))
													{
														throw new ArgumentException("Assembly not configured for silverlight: " + args.Name);	
													}

			                                 		args.Assembly = _cache[args.Name];
			                                 	};
		}

		private static void AddAssemblyToCache(Type type)
		{
			Assembly assembly = type.Assembly;
			_cache[AssemblyNameFor(assembly)] = assembly;
		}

		private static string AssemblyNameFor(Assembly assembly)
		{
			return new AssemblyName(assembly.FullName).Name;;
		}

		public App()
		{
			Startup += Application_Startup;
			Exit += Application_Exit;
			UnhandledException += Application_UnhandledException;

			InitializeComponent();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			RootVisual = new MainPage();
		}

		private void Application_Exit(object sender, EventArgs e)
		{

		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!Debugger.IsAttached)
			{

				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}
		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}
