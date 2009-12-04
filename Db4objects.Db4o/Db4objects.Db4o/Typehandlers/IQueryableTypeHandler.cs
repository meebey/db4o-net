/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
	public interface IQueryableTypeHandler : ITypeHandler4
	{
		bool DescendsIntoMembers();
	}
}
