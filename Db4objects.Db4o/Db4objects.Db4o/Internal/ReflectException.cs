/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// db4o-specific exception.<br />
	/// <br />
	/// This exception is thrown when one of the db4o reflection methods fails.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br />
	/// <br />
	/// This exception is thrown when one of the db4o reflection methods fails.
	/// </remarks>
	[System.Serializable]
	public class ReflectException : Db4oException
	{
		public ReflectException(Exception cause) : base(cause)
		{
		}
	}
}
