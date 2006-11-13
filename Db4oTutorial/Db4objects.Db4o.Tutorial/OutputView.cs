/*
 * Created by SharpDevelop.
 * User: rodrigob
 * Date: 11/1/2004
 * Time: 2:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI;

namespace Db4objects.Db4o.Tutorial
{
	/// <summary>
	/// Description of OutputView.
	/// </summary>
	public class OutputView : DockContent
	{
		OutputViewControl _console;
		
		public OutputView(MainForm main)
		{
			this.CloseButton = false;
			this.DockableAreas = (
					DockAreas.Float |
					DockAreas.DockBottom |
					DockAreas.DockTop |
					DockAreas.DockLeft |
					DockAreas.DockRight);
			this.ShowHint = DockState.DockBottom;
			this.Text = "Output";
			
			_console = new OutputViewControl();
			_console.MainForm = main;
			_console.Dock = DockStyle.Fill;
			this.Controls.Add(_console);
		}
		
		public void AppendText(string text)
		{
			_console.AppendText(text);
		}
		
		public void WriteLine(string line)
		{
			_console.WriteLine(line);
		}
	}
}
