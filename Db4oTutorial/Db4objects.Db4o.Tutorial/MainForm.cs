/*
 * Created by SharpDevelop.
 * User: rodrigob
 * Date: 11/1/2004
 * Time: 1:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using Db4objects.Db4o.Tutorial.F1;

namespace Db4objects.Db4o.Tutorial
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		DockPanel _dockPanel;
		OutputView _outputView;
		TutorialOutlineView _outlineView;
		WebBrowserView _webBrowserView;
		ExampleRunner _exampleRunner;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_dockPanel = new DockPanel();
			_dockPanel.Dock = DockStyle.Fill;
			
			this.Controls.Add(_dockPanel);
			
			_outlineView = new TutorialOutlineView(this);
			_outputView = new OutputView(this);
			_webBrowserView = new WebBrowserView();			
			_webBrowserView.External = this;
			
			_exampleRunner = new ExampleRunner();
		}
		
		override protected void OnLoad(EventArgs args)
		{
			base.OnLoad(args);
			LoadViews();
		}
		
		public void NavigateTutorial(string href)
		{
			_webBrowserView.Navigate(GetTutorialFilePath(href));
		}
		
		public string GetTutorialFilePath(string fname)
		{
			return Path.Combine(GetTutorialBasePath(), fname);
		}
		
		public string GetTutorialBasePath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "docs/");
		}
		
		public void ResetDatabase()
		{
			_outputView.WriteLine("[FULL RESET]");
			_exampleRunner.Reset();
		}
		
		public void RunExample(string typeName, string method)
		{	
			_outputView.WriteLine("[" + method + "]");
			Cursor current = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				StringWriter output = new StringWriter();
				_exampleRunner.Run(typeName, method, output);
				_outputView.AppendText(output.ToString());
			}
			catch (Exception x)
			{
				_outputView.AppendText(x.ToString());
			}
			finally
			{
				Cursor.Current = current;
			}
			_outputView.WriteLine("");
		}
		
		[STAThread]
		public static void Main(string[] args)
		{
			Application.Run(new MainForm());
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "db4o tutorial";
		}
		#endregion
		
		void LoadViews()
		{	
			_outputView.Show(_dockPanel);
			_outlineView.Show(_dockPanel);
			_webBrowserView.Show(_dockPanel);
			
		}
		
	}
}
