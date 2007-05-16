/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the supplied object ID
	/// is incorrect (ouside the scope of the database IDs).
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the supplied object ID
	/// is incorrect (ouside the scope of the database IDs).
	/// </remarks>
	/// <seealso cref="IExtObjectContainer.Bind">IExtObjectContainer.Bind</seealso>
	/// <seealso cref="IExtObjectContainer.GetByID">IExtObjectContainer.GetByID</seealso>
	[System.Serializable]
	public class InvalidIDException : Db4oException
	{
		public InvalidIDException(Exception cause) : base(cause)
		{
		}
	}
}
