/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// db4o-specific exception.&lt;br&gt;
	/// &lt;br&gt;
	/// This exception is thrown when one of the db4o reflection methods fails.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.&lt;br&gt;
	/// &lt;br&gt;
	/// This exception is thrown when one of the db4o reflection methods fails.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Reflect">Db4objects.Db4o.Reflect</seealso>
	[System.Serializable]
	public class ReflectException : Db4oException
	{
		/// <summary>Constructor with the cause exception</summary>
		/// <param name="cause">cause exception</param>
		public ReflectException(Exception cause) : base(cause)
		{
		}
	}
}
