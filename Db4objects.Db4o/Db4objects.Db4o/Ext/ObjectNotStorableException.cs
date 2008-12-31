/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// this Exception is thrown, if objects can not be stored and if
	/// db4o is configured to throw Exceptions on storage failures.
	/// </summary>
	/// <remarks>
	/// this Exception is thrown, if objects can not be stored and if
	/// db4o is configured to throw Exceptions on storage failures.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ExceptionsOnNotStorable">Db4objects.Db4o.Config.IConfiguration.ExceptionsOnNotStorable
	/// 	</seealso>
	[System.Serializable]
	public class ObjectNotStorableException : Db4oException
	{
		public ObjectNotStorableException(IReflectClass a_class) : base(Db4objects.Db4o.Internal.Messages
			.Get(a_class.IsPrimitive() ? 59 : 45, a_class.GetName()))
		{
		}

		public ObjectNotStorableException(string message) : base(message)
		{
		}
	}
}
