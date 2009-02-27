/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Marshall
{
	/// <summary>this interface is passed to first class type handlers (non embedded ones).
	/// 	</summary>
	/// <remarks>this interface is passed to first class type handlers (non embedded ones).
	/// 	</remarks>
	public interface IFirstClassReadContext : IReadContext
	{
		object PersistentObject();
	}
}
