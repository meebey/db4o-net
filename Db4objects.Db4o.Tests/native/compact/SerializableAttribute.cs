/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

#if CF_1_0 || CF_2_0

namespace System
{
	public class SerializableAttribute : Attribute
	{
	}
}

namespace Db4objects.Db4o.Tests
{
	class WindowWriter : System.IO.TextWriter 
	{
		public override System.Text.Encoding Encoding
		{
			get
			{
				return System.Text.Encoding.UTF8;
			}
		}

		public override void Write(string s)
		{
			Console.WriteLine(s);
		}

		public override void Write(object o)
		{
			Console.WriteLine(o);
		}

		public override void WriteLine(string s)
		{
			Console.WriteLine(s);
		}

		public override void WriteLine(object o)
		{
			Console.WriteLine(o);
		}
	}
}

#endif