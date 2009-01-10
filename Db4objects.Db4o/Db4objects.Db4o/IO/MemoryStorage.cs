/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>
	/// <see cref="Db4objects.Db4o.IO.IStorage">Db4objects.Db4o.IO.IStorage</see>
	/// implementation that produces
	/// <see cref="Db4objects.Db4o.IO.IBin">Db4objects.Db4o.IO.IBin</see>
	/// instances
	/// that operate in memory.
	/// Use this
	/// <see cref="Db4objects.Db4o.IO.IStorage">Db4objects.Db4o.IO.IStorage</see>
	/// to work with db4o as an in-memory database.
	/// </summary>
	public class MemoryStorage : IStorage
	{
		private readonly IDictionary _storages = new Hashtable();

		/// <summary>
		/// returns true if a MemoryBin with the given URI name already exists
		/// in this Storage.
		/// </summary>
		/// <remarks>
		/// returns true if a MemoryBin with the given URI name already exists
		/// in this Storage.
		/// </remarks>
		public virtual bool Exists(string uri)
		{
			return _storages.Contains(uri);
		}

		/// <summary>opens a MemoryBin for the given URI (name can be freely chosen).</summary>
		/// <remarks>opens a MemoryBin for the given URI (name can be freely chosen).</remarks>
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual IBin Open(BinConfiguration config)
		{
			IBin storage = ProduceStorage(config);
			return config.ReadOnly() ? new ReadOnlyBin(storage) : storage;
		}

		/// <summary>Returns the memory bin for the given URI for external use.</summary>
		/// <remarks>Returns the memory bin for the given URI for external use.</remarks>
		public virtual MemoryBin Bin(string uri)
		{
			return ((MemoryBin)_storages[uri]);
		}

		/// <summary>Registers the given bin for this storage with the given URI.</summary>
		/// <remarks>Registers the given bin for this storage with the given URI.</remarks>
		public virtual void Bin(string uri, MemoryBin bin)
		{
			_storages.Add(uri, bin);
		}

		private IBin ProduceStorage(BinConfiguration config)
		{
			IBin storage = Bin(config.Uri());
			if (null != storage)
			{
				return storage;
			}
			MemoryBin newStorage = new MemoryBin(new byte[(int)config.InitialLength()]);
			_storages.Add(config.Uri(), newStorage);
			return newStorage;
		}
	}
}
