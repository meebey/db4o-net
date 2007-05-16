/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when a system IO exception
	/// is encounted by db4o process.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when a system IO exception
	/// is encounted by db4o process.
	/// </remarks>
	[System.Serializable]
	public class Db4oIOException : Db4oException
	{
		public Db4oIOException() : base()
		{
		}

		public Db4oIOException(Exception cause) : base(cause.Message, cause)
		{
		}
	}
}
