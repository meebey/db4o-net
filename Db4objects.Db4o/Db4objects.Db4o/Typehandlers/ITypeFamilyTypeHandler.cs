/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
	/// <exclude></exclude>
	public interface ITypeFamilyTypeHandler : IQueryableTypeHandler
	{
		int LinkLength();
	}
}
