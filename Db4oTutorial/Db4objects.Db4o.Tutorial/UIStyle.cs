/*
 * Created by SharpDevelop.
 * User: rodrigob
 * Date: 11/4/2004
 * Time: 3:27 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Db4objects.Db4o.Tutorial
{
	/// <summary>
	/// Description of UIStyle.
	/// </summary>
	public class UIStyle
	{
		public static readonly Color Db4oGrey = Color.FromArgb(0xFF, 0x66, 0x61, 0x77);
		
		public static readonly Color Db4oGreen = Color.FromArgb(0xFF, 0xAD, 0xD6, 0x5C);
    
		public static readonly Color BackColor = Color.FromArgb(0xFF, 0x83, 0x83, 0x83);
		
		public static readonly Color TextColor = Color.White;
		
		public static void Apply(Control control)
		{
			control.BackColor = UIStyle.BackColor;
			control.ForeColor = UIStyle.TextColor;
		}
		
		public static void ApplyConsoleStyle(Control control)
		{
			control.BackColor = UIStyle.BackColor;
			control.ForeColor = UIStyle.Db4oGreen;
		}
		
		public static void ApplyButtonStyle(Control control)
		{
			control.ForeColor = UIStyle.Db4oGrey;
			control.BackColor = UIStyle.Db4oGreen;
		}
		
		private UIStyle()
		{
		}
	}
}
