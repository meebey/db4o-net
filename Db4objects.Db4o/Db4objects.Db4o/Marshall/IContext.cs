/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;

namespace Db4objects.Db4o.Marshall
{
	/// <summary>
	/// common functionality for
	/// <see cref="Db4objects.Db4o.Marshall.IReadContext">Db4objects.Db4o.Marshall.IReadContext
	/// 	</see>
	/// and
	/// <see cref="Db4objects.Db4o.Marshall.IWriteContext">Db4objects.Db4o.Marshall.IWriteContext
	/// 	</see>
	/// and
	/// <see cref="Db4objects.Db4o.Internal.Delete.IDeleteContext">Db4objects.Db4o.Internal.Delete.IDeleteContext
	/// 	</see>
	/// 
	/// </summary>
	public interface IContext
	{
		IObjectContainer ObjectContainer();

		Db4objects.Db4o.Internal.Transaction Transaction();
	}
}
