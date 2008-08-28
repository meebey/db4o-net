/*
 * Created by SharpDevelop.
 * User: rodrigob
 * Date: 11/5/2004
 * Time: 7:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Db4objects.Db4o.Tutorial
{
	/// <summary>
	/// Description of TutorialOutlineViewControl.
	/// </summary>
	public class TutorialOutlineViewControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView _tree;
		private System.Windows.Forms.PictureBox _logo;
		public TutorialOutlineViewControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			UIStyle.Apply(this);
			UIStyle.Apply(_tree);	
		}
		
		public TreeView TreeView
		{
			get
			{
				return _tree;
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TutorialOutlineViewControl));
			this._logo = new System.Windows.Forms.PictureBox();
			this._tree = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// _logo
			// 
			this._logo.Dock = System.Windows.Forms.DockStyle.Top;
			this._logo.Image = ((System.Drawing.Image)(resources.GetObject("_logo.Image")));
			this._logo.Location = new System.Drawing.Point(0, 0);
			this._logo.Name = "_logo";
			this._logo.Size = new System.Drawing.Size(165, 42);
			this._logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._logo.TabIndex = 0;
			this._logo.TabStop = false;
			// 
			// _tree
			// 
			this._tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tree.ImageIndex = -1;
			this._tree.Location = new System.Drawing.Point(0, 42);
			this._tree.Name = "_tree";
			this._tree.SelectedImageIndex = -1;
			this._tree.Size = new System.Drawing.Size(292, 246);
			this._tree.TabIndex = 1;
			// 
			// TutorialOutlineViewControl
			// 
			this.Controls.Add(this._tree);
			this.Controls.Add(this._logo);
			this.Name = "TutorialOutlineViewControl";
			this.Size = new System.Drawing.Size(292, 288);
			this.ResumeLayout(false);
		}
		#endregion
		
	}
}
