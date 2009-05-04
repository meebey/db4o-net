/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
namespace Db4objects.Db4o.Internal.Query
{
	public interface ICachingStrategy
	{
		void Add(object key, object item);
		object Get(object key);
	}
}