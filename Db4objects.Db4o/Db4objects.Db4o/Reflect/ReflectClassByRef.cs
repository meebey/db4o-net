/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	/// <summary>Useful as "out" or "by ref" function parameter.</summary>
	/// <remarks>Useful as "out" or "by ref" function parameter.</remarks>
	public sealed class ReflectClassByRef
	{
		/// <summary>Useful whenever an "out" parameter is to be ignored.</summary>
		/// <remarks>Useful whenever an "out" parameter is to be ignored.</remarks>
		public static readonly Db4objects.Db4o.Reflect.ReflectClassByRef Ignored = new Db4objects.Db4o.Reflect.ReflectClassByRef
			();

		public IReflectClass value;

		public ReflectClassByRef(IReflectClass initialValue)
		{
			value = initialValue;
		}

		public ReflectClassByRef()
		{
		}
	}
}
