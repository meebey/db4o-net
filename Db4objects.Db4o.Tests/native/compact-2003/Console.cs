/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

#if CF_1_0

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;

namespace Db4objects.Db4o.Tests {
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Console: System.Windows.Forms.Form {
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MainMenu mainMenu1;

        public Console() {
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
        protected override void Dispose( bool disposing ) {
            base.Dispose( disposing );
        }
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
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
            // Console
            // 
            this.Menu = this.mainMenu1;
            this.Text = "db4o test console";

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static void Main() {
			Db4oUnit.TestPlatform.Out = new WindowWriter();
            Application.Run(new Console());
        }

        static void CheckStatus(Object state) {
            WriteLine("Cool");
        }

        public static void WriteLine(object obj){
            String str = obj.ToString();
            int pos = str.IndexOf("\n");
            if(pos > -1){
                WriteLine(str.Substring(0, pos));
                WriteLine(str.Substring(pos + 1));
            }else{
                lines.Enqueue(obj.ToString());
                if(lines.Count > 23){
                    lines.Dequeue();
                }
				if (null == staticThis) {
					foreach (string line in lines) {
						System.Console.WriteLine(line);
					}
				} else {
					staticThis.PrintLines();
				}
            }
        }

        public static void WriteLine(){
            WriteLine("");
        }

        private void PrintLines(){
            Graphics g = this.CreateGraphics();
            Font font = new Font("Verdana", 8, FontStyle.Regular);
            Brush brush = new SolidBrush(Color.Black);
            Brush backBrush = new SolidBrush(Color.White);
            IEnumerator e = lines.GetEnumerator();
            int y = 1;
            try{
                while(e.MoveNext()){
                    g.FillRectangle(backBrush, 1, (y -1)* 11, 1000, 13);
                    g.DrawString( (string)e.Current, font, brush, 1, (y -1)* 11);
                    y++;
                }
            }catch(Exception ex){
            }
            g.Dispose();
        }

        static Queue lines = new Queue();
        static Console staticThis;
        static int entry = 0;

        private void _menuItem2_Click(object sender, System.EventArgs e) {
            AllTests.Main(null);
        }

        private void _menuItem3_Click(object sender, System.EventArgs e) {
            Application.Exit();
        }
    }
}

#endif
