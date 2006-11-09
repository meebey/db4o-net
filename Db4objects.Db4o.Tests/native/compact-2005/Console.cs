/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

#if CF_2_0

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;

namespace Db4objects.Db4o.Tests
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Console : System.Windows.Forms.Form
    {
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private TextBox _console;
        private System.Windows.Forms.MainMenu mainMenu1;

        public Console()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            staticThis = this;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this._console = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.menuItem2);
            this.menuItem1.MenuItems.Add(this.menuItem3);
            this.menuItem1.Text = "db4o";
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Run Regression Tests";
            this.menuItem2.Click += new System.EventHandler(this._menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "Exit";
            this.menuItem3.Click += new System.EventHandler(this._menuItem3_Click);
            // 
            // _console
            // 
            this._console.Dock = System.Windows.Forms.DockStyle.Fill;
            this._console.Location = new System.Drawing.Point(0, 0);
            this._console.Multiline = true;
            this._console.Name = "_console";
            this._console.ReadOnly = true;
            this._console.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._console.Size = new System.Drawing.Size(240, 268);
            this._console.TabIndex = 0;
            // 
            // Console
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this._console);
            this.Menu = this.mainMenu1;
            this.Name = "Console";
            this.Text = "db4o test console";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static void Main()
        {
            Db4oUnit.TestPlatform.Out = new WindowWriter();
            Application.Run(new Console());
        }

        static void CheckStatus(Object state)
        {
            WriteLine("Cool");
        }

        public static void WriteLine(string s)
        {
            TextBox console = staticThis._console;
            console.SelectedText = s.Replace("\r", "").Replace("\n", "\r\n");
            console.SelectedText = "\r\n";
            Application.DoEvents();
        }

        public static void WriteLine(object obj)
        {
            WriteLine(obj != null ? obj.ToString() : "");
        }

        public static void WriteLine()
        {
            WriteLine("");
        }

        static Console staticThis;
        static int entry = 0;

        private void _menuItem2_Click(object sender, System.EventArgs e)
        {
            _console.Text = "";
            AllTests.Main(null);
            // com.db4o.test.nativequeries.cats.TestCatSpeed.Main(null);
        }

        private void _menuItem3_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}

#endif
