/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.&lt;br&gt;&lt;br&gt;
	/// This exception is thrown when a client tries to connect
	/// to a server with a wrong password or null password.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.&lt;br&gt;&lt;br&gt;
	/// This exception is thrown when a client tries to connect
	/// to a server with a wrong password or null password.
	/// </remarks>
	[System.Serializable]
	public class InvalidPasswordException : Db4oException
	{
	}
}
