/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Sharpen;

namespace Db4objects.Db4o.IO
{
	public class MemoryStorage : IStorage
	{
		private readonly IDictionary _storages = new Hashtable();

		public virtual bool Exists(string uri)
		{
			return _storages.Contains(uri);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual IBin Open(string uri, bool lockFile, long initialLength, bool readOnly
			)
		{
			IBin storage = ProduceStorage(uri, initialLength);
			return readOnly ? new ReadOnlyBin(storage) : storage;
		}

		private IBin ProduceStorage(string uri, long initialLength)
		{
			IBin storage = ((IBin)_storages[uri]);
			if (null != storage)
			{
				return storage;
			}
			MemoryStorage.MemoryBin newStorage = new MemoryStorage.MemoryBin(new byte[(int)initialLength
				]);
			_storages.Add(uri, newStorage);
			return newStorage;
		}

		private class MemoryBin : IBin
		{
			private const int GrowBy = 10000;

			private byte[] _bytes;

			private int _length;

			public MemoryBin(byte[] bytes)
			{
				_bytes = bytes;
				_length = bytes.Length;
			}

			public virtual long Length()
			{
				return _length;
			}

			/// <exception cref="Db4oIOException"></exception>
			public virtual int Read(long pos, byte[] bytes, int length)
			{
				long avail = _length - pos;
				if (avail <= 0)
				{
					return -1;
				}
				int read = Math.Min((int)avail, length);
				System.Array.Copy(_bytes, (int)pos, bytes, 0, read);
				return read;
			}

			/// <exception cref="Db4oIOException"></exception>
			public virtual void Sync()
			{
			}

			public virtual void Close()
			{
			}

			/// <summary>for internal processing only.</summary>
			/// <remarks>for internal processing only.</remarks>
			/// <exception cref="Db4oIOException"></exception>
			public virtual void Write(long pos, byte[] buffer, int length)
			{
				if (pos + length > _bytes.Length)
				{
					long growBy = GrowBy;
					if (pos + length > growBy)
					{
						growBy = pos + length;
					}
					byte[] temp = new byte[(int)(_bytes.Length + growBy)];
					System.Array.Copy(_bytes, 0, temp, 0, _length);
					_bytes = temp;
				}
				System.Array.Copy(buffer, 0, _bytes, (int)pos, length);
				pos += length;
				if (pos > _length)
				{
					_length = (int)pos;
				}
			}
		}
	}
}
