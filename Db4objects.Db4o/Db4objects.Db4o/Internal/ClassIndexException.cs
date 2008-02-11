/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Internal
{
	[System.Serializable]
	public class ClassIndexException : Exception
	{
		private string _className;

		public ClassIndexException(string className) : this(null, null, className)
		{
		}

		public ClassIndexException(string msg, string className) : this(msg, null, className
			)
		{
		}

		public ClassIndexException(Exception cause, string className) : this(null, cause, 
			className)
		{
		}

		public ClassIndexException(string msg, Exception cause, string className) : base(
			EnhancedMessage(msg, className), cause)
		{
			_className = className;
		}

		public virtual string ClassName()
		{
			return _className;
		}

		private static string EnhancedMessage(string msg, string className)
		{
			string enhancedMessage = "Class index for " + className;
			if (msg != null)
			{
				enhancedMessage += ": " + msg;
			}
			return enhancedMessage;
		}
	}
}
