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

		private static void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (!Debugger.IsAttached)
			{
				MessageBox.Show(e.ToString());
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}
		private static void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
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
