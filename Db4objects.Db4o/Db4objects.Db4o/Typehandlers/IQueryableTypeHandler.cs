/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
	public interface IQueryableTypeHandler : ITypeHandler4
	{
		/// <summary>Returns true if the types handled by this type handler can not refer to other objects.
		/// 	</summary>
		/// <remarks>Returns true if the types handled by this type handler can not refer to other objects.
		/// 	</remarks>
		bool IsSimple();
	}
}
