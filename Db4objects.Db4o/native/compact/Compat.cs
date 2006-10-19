/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Db4objects.Db4o.Reflect;
using Sharpen.Lang;

#if CF_1_0
// not need for CF_2_0
namespace System.Runtime.CompilerServices
{
	internal class IsVolatile
	{
	}
}

namespace System
{
	class NotImplementedException : Exception
	{
		internal NotImplementedException ()
		{
		}

		internal NotImplementedException (string message) : base (message)
		{
		}
	}

	class CompactFramework1Console
	{
		private static TextWriter _out = new DebugTextWriter();

		private static TextWriter _error = new DebugTextWriter();

		public static TextWriter Out
		{
			get
			{
				return _out;
			}
			set
			{
				_out = value;
			}
		}

		public static TextWriter Error
		{
			get
			{
				return _error;
			}
			set
			{
				_error = value;
			}
		}
	}
}

namespace System.IO
{
	class DebugTextWriter : TextWriter
	{
		public override System.Text.Encoding Encoding
		{
			get { return System.Text.Encoding.UTF8; }
		}

		public override void Write(string s)
		{
			Debug.Write(s);
		}

		public override void WriteLine(string s)
		{
			Debug.WriteLine(s);
		}
	}
}

#endif

#if CF_1_0 || CF_2_0
namespace System
{
	class SerializableAttribute : Attribute
	{
	}
}

namespace Db4objects.Db4o
{
	/// <exclude />
	public class Compat
	{
	}
}
#endif