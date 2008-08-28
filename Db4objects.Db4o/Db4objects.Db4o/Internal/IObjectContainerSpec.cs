/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal
{
	/// <summary>Workaround to provide the Java 5 version with a hook to add ExtObjectContainer.
	/// 	</summary>
	/// <remarks>
	/// Workaround to provide the Java 5 version with a hook to add ExtObjectContainer.
	/// (Generic method declarations won't match ungenerified YapStreamBase implementations
	/// otherwise and implementing it directly kills .NET conversion.)
	/// </remarks>
	/// <exclude></exclude>
	/// <decaf.ignore.implements>InternalObjectContainer</decaf.ignore.implements>
	public interface IObjectContainerSpec
	{
	}
}
