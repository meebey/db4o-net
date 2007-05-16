/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o exception wrapper: Exceptions occurring during internal processing
	/// will be proliferated to the client calling code encapsulated in an exception
	/// of this type.
	/// </summary>
	/// <remarks>
	/// db4o exception wrapper: Exceptions occurring during internal processing
	/// will be proliferated to the client calling code encapsulated in an exception
	/// of this type. The original exception, if any, is available through
	/// <see cref="Exception.InnerException">Exception.InnerException</see>
	/// .
	/// </remarks>
	[System.Serializable]
	public class Db4oException : Exception
	{
		public Db4oException() : this(null, null)
		{
		}

		public Db4oException(string msg) : this(msg, null)
		{
		}

		public Db4oException(Exception cause) : this(cause.Message, cause)
		{
		}

		public Db4oException(int messageConstant) : this(Db4objects.Db4o.Internal.Messages
			.Get(messageConstant))
		{
		}

		public Db4oException(string msg, Exception cause) : base(msg, cause)
		{
		}
	}
}
