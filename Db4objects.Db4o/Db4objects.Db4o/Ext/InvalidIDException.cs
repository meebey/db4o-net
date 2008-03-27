/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the supplied object ID
	/// is incorrect (outside the scope of the database IDs).
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the supplied object ID
	/// is incorrect (outside the scope of the database IDs).
	/// </remarks>
	/// <seealso cref="IExtObjectContainer.Bind">IExtObjectContainer.Bind</seealso>
	/// <seealso cref="IExtObjectContainer.GetByID">IExtObjectContainer.GetByID</seealso>
	[System.Serializable]
	public class InvalidIDException : Db4oException
	{
		/// <summary>Constructor allowing to specify the exception cause</summary>
		/// <param name="cause">cause exception</param>
		public InvalidIDException(Exception cause) : base(cause)
		{
		}

		/// <summary>Constructor allowing to specify the offending id</summary>
		/// <param name="id">the offending id</param>
		public InvalidIDException(int id) : base("id: " + id)
		{
		}
	}
}
