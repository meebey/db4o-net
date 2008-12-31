/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>
	/// Base interface for Storage adapters that open a
	/// <see cref="Db4objects.Db4o.IO.IBin">Db4objects.Db4o.IO.IBin</see>
	/// to store db4o database data to.
	/// </summary>
	/// <seealso cref="Db4objects.Db4o.Config.IFileConfiguration.Storage"></seealso>
	public interface IStorage
	{
		/// <summary>
		/// opens a
		/// <see cref="Db4objects.Db4o.IO.IBin">Db4objects.Db4o.IO.IBin</see>
		/// to store db4o database data.
		/// </summary>
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		IBin Open(BinConfiguration config);

		/// <summary>returns true if a Bin (file or memory) exists with the passed name.</summary>
		/// <remarks>returns true if a Bin (file or memory) exists with the passed name.</remarks>
		bool Exists(string uri);
	}
}
