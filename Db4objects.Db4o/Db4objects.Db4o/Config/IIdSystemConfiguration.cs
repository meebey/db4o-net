/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;

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

		/// <summary>configures db4o to use an in-memory ID system.</summary>
		/// <remarks>
		/// configures db4o to use an in-memory ID system.
		/// All IDs get written to the database file on every commit.
		/// </remarks>
		void UseInMemorySystem();

		/// <summary>configures db4o to use a custom ID system.</summary>
		/// <remarks>
		/// configures db4o to use a custom ID system.
		/// Pass an
		/// <see cref="IIdSystemFactory">IIdSystemFactory</see>
		/// that creates the IdSystem.
		/// Note that this factory has to be configured every time you
		/// open a database that you configured to use a custom IdSystem.
		/// </remarks>
		void UseCustomSystem(IIdSystemFactory factory);
	}
}
