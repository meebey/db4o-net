/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Ext
{
	[System.Serializable]
	public class Db4oFatalException : Db4objects.Db4o.Ext.Db4oException
	{
		public Db4oFatalException() : base()
		{
		}

		public Db4oFatalException(int messageConstant) : base(messageConstant)
		{
		}

		public Db4oFatalException(string msg, Exception cause) : base(msg, cause)
		{
		}

		public Db4oFatalException(string msg) : base(msg)
		{
		}

		public Db4oFatalException(Exception cause) : base(cause)
		{
		}
	}
}
