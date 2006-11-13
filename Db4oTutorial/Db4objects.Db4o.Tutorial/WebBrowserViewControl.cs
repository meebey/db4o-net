/*
 * Created by SharpDevelop.
 * User: rodrigob
 * Date: 11/4/2004
 * Time: 5:32 AM
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
	/// Description of WebBrowserViewControl.
	/// </summary>
	public class WebBrowserViewControl : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.IContainer components;
		private AxSHDocVw.AxWebBrowser _webBrowser;
		private System.Windows.Forms.ToolBar _toolbar;
		private System.Windows.Forms.ToolBarButton _buttonBack;
		private System.Windows.Forms.ImageList _toolbarImages;
		private System.Windows.Forms.ToolBarButton _buttonForward;
		public WebBrowserViewControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public AxSHDocVw.AxWebBrowser WebBrowser
		{
			get
			{
				return _webBrowser;
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WebBrowserViewControl));
			this._buttonForward = new System.Windows.Forms.ToolBarButton();
			this._toolbarImages = new System.Windows.Forms.ImageList(this.components);
			this._buttonBack = new System.Windows.Forms.ToolBarButton();
			this._toolbar = new System.Windows.Forms.ToolBar();
			this._webBrowser = new AxSHDocVw.AxWebBrowser();
			((System.ComponentModel.ISupportInitialize)(this._webBrowser)).BeginInit();
			this.SuspendLayout();
			// 
			// _buttonForward
			// 
			this._buttonForward.ImageIndex = 1;
			this._buttonForward.ToolTipText = "Forward";
			// 
			// _toolbarImages
			// 
			this._toolbarImages.ImageSize = new System.Drawing.Size(16, 16);
			this._toolbarImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_toolbarImages.ImageStream")));
			this._toolbarImages.TransparentColor = System.Drawing.Color.White;
			// 
			// _buttonBack
			// 
			this._buttonBack.ImageIndex = 0;
			this._buttonBack.ToolTipText = "Back";
			// 
			// _toolbar
			// 
			this._toolbar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this._toolbar.AutoSize = false;
			this._toolbar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this._buttonBack,
																						this._buttonForward});
			this._toolbar.ButtonSize = new System.Drawing.Size(18, 18);
			this._toolbar.Divider = false;
			this._toolbar.DropDownArrows = true;
			this._toolbar.ImageList = this._toolbarImages;
			this._toolbar.Location = new System.Drawing.Point(0, 0);
			this._toolbar.Name = "_toolbar";
			this._toolbar.ShowToolTips = true;
			this._toolbar.Size = new System.Drawing.Size(544, 24);
			this._toolbar.TabIndex = 0;
			this._toolbar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this._toolbarButtonClick);
			// 
			// _webBrowser
			// 
			this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._webBrowser.Enabled = true;
			this._webBrowser.Location = new System.Drawing.Point(0, 24);
			this._webBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("_webBrowser.OcxState")));
			this._webBrowser.Size = new System.Drawing.Size(544, 392);
			this._webBrowser.TabIndex = 1;
			this._webBrowser.CommandStateChange += new AxSHDocVw.DWebBrowserEvents2_CommandStateChangeEventHandler(this._webBrowserCommandStateChange);
			// 
			// WebBrowserViewControl
			// 
			this.Controls.Add(this._webBrowser);
			this.Controls.Add(this._toolbar);
			this.Name = "WebBrowserViewControl";
			this.Size = new System.Drawing.Size(544, 416);
			((System.ComponentModel.ISupportInitialize)(this._webBrowser)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		void _webBrowserCommandStateChange(object sender, AxSHDocVw.DWebBrowserEvents2_CommandStateChangeEvent e)
		{
			switch (e.command)
			{
				case (int)SHDocVw.CommandStateChangeConstants.CSC_NAVIGATEFORWARD:
				{		
					_buttonForward.Enabled = e.enable;
					break;
				}
					
				case (int)SHDocVw.CommandStateChangeConstants.CSC_NAVIGATEBACK:
				{
					_buttonBack.Enabled = e.enable;
					break;
				}
			}
		}		
		void _toolbarButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (_buttonBack == e.Button)
			{
				_webBrowser.GoBack();
			}
			else if (_buttonForward == e.Button)
			{
				_webBrowser.GoForward();
			}
		}
		
	}
}
