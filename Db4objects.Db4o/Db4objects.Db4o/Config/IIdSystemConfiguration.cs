/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Config
{
	/// <summary>Interface to configure the IdSystem.</summary>
	/// <remarks>Interface to configure the IdSystem.</remarks>
	public interface IIdSystemConfiguration
	{
		/// <summary>configures db4o to store IDs as pointers.</summary>
		/// <remarks>configures db4o to store IDs as pointers.</remarks>
		void UsePointerBasedSystem();

		/// <summary>configures db4o to use a BTree based ID system.</summary>
		/// <remarks>configures db4o to use a BTree based ID system.</remarks>
		void UseBTreeSystem();
	}
}
