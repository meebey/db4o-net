/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal.Caching;
using Sharpen;

namespace Db4objects.Db4o.IO
{
	public abstract class IoAdapterWithCache : IoAdapter
	{
		private long _position;

		private int _pageSize;

		private int _pageCount;

		private long _fileLength;

		private IoAdapter _io;

		private bool _readOnly;

		private ICache4 _cache;

		private IObjectPool _pagePool;

		private static int DefaultPageSize = 1024;

		private static int DefaultPageCount = 64;

		private sealed class _IProcedure4_33 : IProcedure4
		{
			public _IProcedure4_33(IoAdapterWithCache _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object discardedPage)
			{
				this._enclosing.FlushPage(((IoAdapterWithCache.Page)discardedPage));
				this._enclosing._pagePool.ReturnObject(((IoAdapterWithCache.Page)discardedPage));
			}

			private readonly IoAdapterWithCache _enclosing;
		}

		private IProcedure4 _onDiscardPage;

		/// <summary>
		/// Creates an instance of CachedIoAdapter with the default page size and
		/// page count.
		/// </summary>
		/// <remarks>
		/// Creates an instance of CachedIoAdapter with the default page size and
		/// page count.
		/// </remarks>
		/// <param name="ioAdapter">delegate IO adapter (RandomAccessFileAdapter by default)</param>
		public IoAdapterWithCache(IoAdapter ioAdapter) : this(ioAdapter, DefaultPageSize, 
			DefaultPageCount)
		{
			_onDiscardPage = new _IProcedure4_33(this);
			_producerFromDisk = new _IFunction4_239(this);
			_producerFromCache = new _IFunction4_248(this);
		}

		/// <summary>
		/// Creates an instance of CachedIoAdapter with a custom page size and page
		/// count.<br />
		/// </summary>
		/// <param name="ioAdapter">delegate IO adapter (RandomAccessFileAdapter by default)</param>
		/// <param name="pageSize">cache page size</param>
		/// <param name="pageCount">allocated amount of pages</param>
		public IoAdapterWithCache(IoAdapter ioAdapter, int pageSize, int pageCount)
		{
			_onDiscardPage = new _IProcedure4_33(this);
			_producerFromDisk = new _IFunction4_239(this);
			_producerFromCache = new _IFunction4_248(this);
			_io = ioAdapter;
			_pageSize = pageSize;
			_pageCount = pageCount;
		}

		/// <summary>Creates an instance of CachedIoAdapter with extended parameters.<br /></summary>
		/// <param name="path">database file path</param>
		/// <param name="lockFile">determines if the file should be locked</param>
		/// <param name="initialLength">initial file length, new writes will start from this point
		/// 	</param>
		/// <param name="readOnly">if the file should be used in read-onlyt mode.</param>
		/// <param name="io">delegate IO adapter (RandomAccessFileAdapter by default)</param>
		/// <param name="pageSize">cache page size</param>
		/// <param name="pageCount">allocated amount of pages</param>
		/// <exception cref="Db4oIOException"></exception>
		private IoAdapterWithCache(string path, bool lockFile, long initialLength, bool readOnly
			, IoAdapter io, ICache4 cache, int pageCount, int pageSize)
		{
			_onDiscardPage = new _IProcedure4_33(this);
			_producerFromDisk = new _IFunction4_239(this);
			_producerFromCache = new _IFunction4_248(this);
			_readOnly = readOnly;
			_pageSize = pageSize;
			_io = io.Open(path, lockFile, initialLength, readOnly);
			_pagePool = new SimpleObjectPool(NewPagePool(pageCount));
			_cache = cache;
			_position = initialLength;
			_fileLength = _io.GetLength();
		}

		private IoAdapterWithCache.Page[] NewPagePool(int pageCount)
		{
			IoAdapterWithCache.Page[] pages = new IoAdapterWithCache.Page[pageCount];
			for (int i = 0; i < pages.Length; ++i)
			{
				pages[i] = new IoAdapterWithCache.Page(_pageSize);
			}
			return pages;
		}

		/// <summary>Creates and returns a new CachedIoAdapter <br /></summary>
		/// <param name="path">database file path</param>
		/// <param name="lockFile">determines if the file should be locked</param>
		/// <param name="initialLength">initial file length, new writes will start from this point
		/// 	</param>
		/// <exception cref="Db4oIOException"></exception>
		public override IoAdapter Open(string path, bool lockFile, long initialLength, bool
			 readOnly)
		{
			return new _IoAdapterWithCache_118(path, lockFile, initialLength, readOnly, _io, 
				NewCache(_pageCount), _pageCount, _pageSize);
		}

		private sealed class _IoAdapterWithCache_118 : Db4objects.Db4o.IO.IoAdapterWithCache
		{
			public _IoAdapterWithCache_118(string baseArg1, bool baseArg2, long baseArg3, bool
				 baseArg4, IoAdapter baseArg5, ICache4 baseArg6, int baseArg7, int baseArg8) : base
				(baseArg1, baseArg2, baseArg3, baseArg4, baseArg5, baseArg6, baseArg7, baseArg8)
			{
			}

			protected override ICache4 NewCache(int pageCount)
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>Deletes the database file</summary>
		/// <param name="path">file path</param>
		public override void Delete(string path)
		{
			_io.Delete(path);
		}

		/// <summary>Checks if the file exists</summary>
		/// <param name="path">file path</param>
		public override bool Exists(string path)
		{
			return _io.Exists(path);
		}

		/// <summary>Reads the file into the buffer using pages from cache.</summary>
		/// <remarks>
		/// Reads the file into the buffer using pages from cache. If the next page
		/// is not cached it will be read from the file.
		/// </remarks>
		/// <param name="buffer">destination buffer</param>
		/// <param name="length">how many bytes to read</param>
		/// <exception cref="Db4oIOException"></exception>
		public override int Read(byte[] buffer, int length)
		{
			long startAddress = _position;
			int bytesToRead = length;
			int totalRead = 0;
			while (bytesToRead > 0)
			{
				IoAdapterWithCache.Page page = GetPage(startAddress, _producerFromDisk);
				int readBytes = page.Read(buffer, totalRead, startAddress, bytesToRead);
				if (readBytes <= 0)
				{
					break;
				}
				bytesToRead -= readBytes;
				startAddress += readBytes;
				totalRead += readBytes;
			}
			_position = startAddress;
			return totalRead == 0 ? -1 : totalRead;
		}

		/// <summary>Writes the buffer to cache using pages</summary>
		/// <param name="buffer">source buffer</param>
		/// <param name="length">how many bytes to write</param>
		/// <exception cref="Db4oIOException"></exception>
		public override void Write(byte[] buffer, int length)
		{
			ValidateReadOnly();
			long startAddress = _position;
			int bytesToWrite = length;
			int bufferOffset = 0;
			while (bytesToWrite > 0)
			{
				// page doesn't need to loadFromDisk if the whole page is dirty
				bool loadFromDisk = (bytesToWrite < _pageSize) || (startAddress % _pageSize != 0);
				IoAdapterWithCache.Page page = GetPage(startAddress, loadFromDisk);
				int writtenBytes = page.Write(buffer, bufferOffset, startAddress, bytesToWrite);
				bytesToWrite -= writtenBytes;
				startAddress += writtenBytes;
				bufferOffset += writtenBytes;
			}
			long endAddress = startAddress;
			_position = endAddress;
			_fileLength = Math.Max(endAddress, _fileLength);
		}

		private void ValidateReadOnly()
		{
			if (_readOnly)
			{
				throw new Db4oIOException();
			}
		}

		/// <summary>Flushes cache to a physical storage</summary>
		/// <exception cref="Db4oIOException"></exception>
		public override void Sync()
		{
			ValidateReadOnly();
			FlushAllPages();
			_io.Sync();
		}

		/// <summary>Returns the file length</summary>
		/// <exception cref="Db4oIOException"></exception>
		public override long GetLength()
		{
			return _fileLength;
		}

		/// <summary>Flushes and closes the file</summary>
		/// <exception cref="Db4oIOException"></exception>
		public override void Close()
		{
			try
			{
				FlushAllPages();
			}
			finally
			{
				_io.Close();
			}
		}

		public override IoAdapter DelegatedIoAdapter()
		{
			return _io.DelegatedIoAdapter();
		}

		private sealed class _IFunction4_239 : IFunction4
		{
			public _IFunction4_239(IoAdapterWithCache _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object pageAddress)
			{
				// in case that page is not found in the cache
				IoAdapterWithCache.Page newPage = ((IoAdapterWithCache.Page)this._enclosing._pagePool
					.BorrowObject());
				this._enclosing.LoadPage(newPage, ((long)pageAddress));
				return newPage;
			}

			private readonly IoAdapterWithCache _enclosing;
		}

		internal readonly IFunction4 _producerFromDisk;

		private sealed class _IFunction4_248 : IFunction4
		{
			public _IFunction4_248(IoAdapterWithCache _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object pageAddress)
			{
				// in case that page is not found in the cache
				IoAdapterWithCache.Page newPage = ((IoAdapterWithCache.Page)this._enclosing._pagePool
					.BorrowObject());
				this._enclosing.ResetPageAddress(newPage, ((long)pageAddress));
				return newPage;
			}

			private readonly IoAdapterWithCache _enclosing;
		}

		internal readonly IFunction4 _producerFromCache;

		/// <exception cref="Db4oIOException"></exception>
		private IoAdapterWithCache.Page GetPage(long startAddress, bool loadFromDisk)
		{
			IFunction4 producer = loadFromDisk ? _producerFromDisk : _producerFromCache;
			return GetPage(startAddress, producer);
		}

		private IoAdapterWithCache.Page GetPage(long startAddress, IFunction4 producer)
		{
			IoAdapterWithCache.Page page = ((IoAdapterWithCache.Page)_cache.Produce(PageAddressFor
				(startAddress), producer, _onDiscardPage));
			page.EnsureEndAddress(_fileLength);
			return page;
		}

		private long PageAddressFor(long startAddress)
		{
			return (startAddress / _pageSize) * _pageSize;
		}

		private void ResetPageAddress(IoAdapterWithCache.Page page, long startAddress)
		{
			page.StartAddress(startAddress);
			page.EndAddress(startAddress + _pageSize);
		}

		/// <exception cref="Db4oIOException"></exception>
		private void FlushAllPages()
		{
			for (IEnumerator pIter = _cache.GetEnumerator(); pIter.MoveNext(); )
			{
				IoAdapterWithCache.Page p = ((IoAdapterWithCache.Page)pIter.Current);
				FlushPage(p);
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		private void FlushPage(IoAdapterWithCache.Page page)
		{
			if (!page._dirty)
			{
				return;
			}
			IoSeek(page.StartAddress());
			WritePageToDisk(page);
		}

		/// <exception cref="Db4oIOException"></exception>
		private void LoadPage(IoAdapterWithCache.Page page, long pos)
		{
			long startAddress = pos - pos % _pageSize;
			page.StartAddress(startAddress);
			IoSeek(page._startAddress);
			int count = IoRead(page);
			if (count > 0)
			{
				page.EndAddress(startAddress + count);
			}
			else
			{
				page.EndAddress(startAddress);
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		private int IoRead(IoAdapterWithCache.Page page)
		{
			return _io.Read(page._buffer);
		}

		/// <exception cref="Db4oIOException"></exception>
		private void WritePageToDisk(IoAdapterWithCache.Page page)
		{
			try
			{
				_io.Write(page._buffer, page.Size());
				page._dirty = false;
			}
			catch (Db4oIOException e)
			{
				_readOnly = true;
				throw;
			}
		}

		/// <summary>Moves the pointer to the specified file position</summary>
		/// <param name="pos">position within the file</param>
		/// <exception cref="Db4oIOException"></exception>
		public override void Seek(long pos)
		{
			_position = pos;
		}

		/// <exception cref="Db4oIOException"></exception>
		private void IoSeek(long pos)
		{
			_io.Seek(pos);
		}

		private class Page
		{
			internal byte[] _buffer;

			internal long _startAddress = -1;

			internal long _endAddress;

			private readonly int _bufferSize;

			internal bool _dirty;

			private byte[] zeroBytes;

			public Page(int size)
			{
				_bufferSize = size;
				_buffer = new byte[_bufferSize];
			}

			internal virtual void EnsureEndAddress(long fileLength)
			{
				long bufferEndAddress = _startAddress + _bufferSize;
				if (_endAddress < bufferEndAddress && fileLength > _endAddress)
				{
					long newEndAddress = Math.Min(fileLength, bufferEndAddress);
					if (zeroBytes == null)
					{
						zeroBytes = new byte[_bufferSize];
					}
					System.Array.Copy(zeroBytes, 0, _buffer, (int)(_endAddress - _startAddress), (int
						)(newEndAddress - _endAddress));
					_endAddress = newEndAddress;
				}
			}

			internal virtual long EndAddress()
			{
				return _endAddress;
			}

			internal virtual void StartAddress(long address)
			{
				_startAddress = address;
			}

			internal virtual long StartAddress()
			{
				return _startAddress;
			}

			internal virtual void EndAddress(long address)
			{
				_endAddress = address;
			}

			internal virtual int Size()
			{
				return (int)(_endAddress - _startAddress);
			}

			internal virtual int Read(byte[] @out, int outOffset, long startAddress, int length
				)
			{
				int bufferOffset = (int)(startAddress - _startAddress);
				int pageAvailbeDataSize = (int)(_endAddress - startAddress);
				int readBytes = Math.Min(pageAvailbeDataSize, length);
				if (readBytes <= 0)
				{
					// meaning reach EOF
					return -1;
				}
				System.Array.Copy(_buffer, bufferOffset, @out, outOffset, readBytes);
				return readBytes;
			}

			internal virtual int Write(byte[] data, int dataOffset, long startAddress, int length
				)
			{
				int bufferOffset = (int)(startAddress - _startAddress);
				int pageAvailabeBufferSize = _bufferSize - bufferOffset;
				int writtenBytes = Math.Min(pageAvailabeBufferSize, length);
				System.Array.Copy(data, dataOffset, _buffer, bufferOffset, writtenBytes);
				long endAddress = startAddress + writtenBytes;
				if (endAddress > _endAddress)
				{
					_endAddress = endAddress;
				}
				_dirty = true;
				return writtenBytes;
			}
		}

		protected abstract ICache4 NewCache(int pageCount);
	}
}
