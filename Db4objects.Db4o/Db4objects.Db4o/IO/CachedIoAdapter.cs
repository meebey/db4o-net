namespace Db4objects.Db4o.IO
{
	/// <summary>CachedIoAdapter is an IOAdapter for random access files, which caches data for IO access.
	/// 	</summary>
	/// <remarks>
	/// CachedIoAdapter is an IOAdapter for random access files, which caches data for IO access.
	/// Its functionality is similar to OS cache.<br />
	/// Example:<br />
	/// <code>delegateAdapter = new RandomAccessFileAdapter();</code><br />
	/// <code>Db4o.configure().io(new CachedIoAdapter(delegateAdapter));</code><br />
	/// </remarks>
	public class CachedIoAdapter : Db4objects.Db4o.IO.IoAdapter
	{
		private Db4objects.Db4o.IO.CachedIoAdapter.Page _head;

		private Db4objects.Db4o.IO.CachedIoAdapter.Page _tail;

		private long _position;

		private int _pageSize;

		private int _pageCount;

		private long _fileLength;

		private long _filePointer;

		private Db4objects.Db4o.IO.IoAdapter _io;

		private static int DEFAULT_PAGE_SIZE = 1024;

		private static int DEFAULT_PAGE_COUNT = 64;

		public CachedIoAdapter(Db4objects.Db4o.IO.IoAdapter ioAdapter) : this(ioAdapter, 
			DEFAULT_PAGE_SIZE, DEFAULT_PAGE_COUNT)
		{
		}

		public CachedIoAdapter(Db4objects.Db4o.IO.IoAdapter ioAdapter, int pageSize, int 
			pageCount)
		{
			_io = ioAdapter;
			_pageSize = pageSize;
			_pageCount = pageCount;
		}

		public CachedIoAdapter(string path, bool lockFile, long initialLength, Db4objects.Db4o.IO.IoAdapter
			 io, int pageSize, int pageCount)
		{
			_io = io;
			_pageSize = pageSize;
			_pageCount = pageCount;
			InitCache();
			InitIOAdaptor(path, lockFile, initialLength);
			_position = initialLength;
			_filePointer = initialLength;
			_fileLength = _io.GetLength();
		}

		/// <summary>Creates and returns a new CachedIoAdapter <br /></summary>
		/// <param name="path">database file path</param>
		/// <param name="lockFile">determines if the file should be locked</param>
		/// <param name="initialLength">initial file length, new writes will start from this point
		/// 	</param>
		public override Db4objects.Db4o.IO.IoAdapter Open(string path, bool lockFile, long
			 initialLength)
		{
			return new Db4objects.Db4o.IO.CachedIoAdapter(path, lockFile, initialLength, _io, 
				_pageSize, _pageCount);
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

		private void InitIOAdaptor(string path, bool lockFile, long initialLength)
		{
			_io = _io.Open(path, lockFile, initialLength);
		}

		private void InitCache()
		{
			_head = new Db4objects.Db4o.IO.CachedIoAdapter.Page(_pageSize);
			_head.prev = null;
			Db4objects.Db4o.IO.CachedIoAdapter.Page page = _head;
			Db4objects.Db4o.IO.CachedIoAdapter.Page next = _head;
			for (int i = 0; i < _pageCount - 1; ++i)
			{
				next = new Db4objects.Db4o.IO.CachedIoAdapter.Page(_pageSize);
				page.next = next;
				next.prev = page;
				page = next;
			}
			_tail = next;
		}

		/// <summary>Reads the file into the buffer using pages from cache.</summary>
		/// <remarks>
		/// Reads the file into the buffer using pages from cache.
		/// If the next page is not cached it will be read from the file.
		/// </remarks>
		/// <param name="buffer">destination buffer</param>
		/// <param name="length">how many bytes to read</param>
		public override int Read(byte[] buffer, int length)
		{
			long startAddress = _position;
			Db4objects.Db4o.IO.CachedIoAdapter.Page page;
			int readBytes;
			int bytesToRead = length;
			int bufferOffset = 0;
			while (bytesToRead > 0)
			{
				page = GetPage(startAddress, true);
				readBytes = page.Read(buffer, bufferOffset, startAddress, bytesToRead);
				MovePageToHead(page);
				if (readBytes == 0)
				{
					break;
				}
				bytesToRead -= readBytes;
				startAddress += readBytes;
				bufferOffset += readBytes;
			}
			_position = startAddress + bufferOffset;
			return bufferOffset;
		}

		/// <summary>Writes the buffer to cache using pages</summary>
		/// <param name="buffer">source buffer</param>
		/// <param name="length">how many bytes to write</param>
		public override void Write(byte[] buffer, int length)
		{
			long startAddress = _position;
			Db4objects.Db4o.IO.CachedIoAdapter.Page page = null;
			int writtenBytes;
			int bytesToWrite = length;
			int bufferOffset = 0;
			while (bytesToWrite > 0)
			{
				bool loadFromDisk = (length < _pageSize) || (startAddress % _pageSize != 0);
				page = GetPage(startAddress, loadFromDisk);
				writtenBytes = page.Write(buffer, bufferOffset, startAddress, bytesToWrite);
				MovePageToHead(page);
				bytesToWrite -= writtenBytes;
				startAddress += writtenBytes;
				bufferOffset += writtenBytes;
			}
			long endAddress = startAddress + length;
			_position = endAddress;
			_fileLength = System.Math.Max(endAddress, _fileLength);
		}

		/// <summary>Flushes cache to a physical storage</summary>
		public override void Sync()
		{
			FlushAllPages();
			_io.Sync();
		}

		/// <summary>Returns the file length</summary>
		public override long GetLength()
		{
			return _fileLength;
		}

		/// <summary>Flushes and closes the file</summary>
		public override void Close()
		{
			FlushAllPages();
			_io.Close();
		}

		public override Db4objects.Db4o.IO.IoAdapter DelegatedIoAdapter()
		{
			return _io.DelegatedIoAdapter();
		}

		private Db4objects.Db4o.IO.CachedIoAdapter.Page GetPage(long startAddress, bool loadFromDisk
			)
		{
			Db4objects.Db4o.IO.CachedIoAdapter.Page page;
			page = GetPageFromCache(startAddress);
			if (page != null)
			{
				return page;
			}
			page = GetFreePageFromCache();
			if (loadFromDisk)
			{
				GetPageFromDisk(page, startAddress);
			}
			else
			{
				ResetPageAddress(page, startAddress);
			}
			return page;
		}

		private void ResetPageAddress(Db4objects.Db4o.IO.CachedIoAdapter.Page page, long 
			startAddress)
		{
			page.StartAddress(startAddress);
			page.EndAddress(startAddress);
		}

		private Db4objects.Db4o.IO.CachedIoAdapter.Page GetFreePageFromCache()
		{
			if (!_tail.IsFree())
			{
				FlushPage(_tail);
			}
			return _tail;
		}

		private Db4objects.Db4o.IO.CachedIoAdapter.Page GetPageFromCache(long pos)
		{
			Db4objects.Db4o.IO.CachedIoAdapter.Page page = _head;
			while (page != null)
			{
				if (page.Contains(pos))
				{
					return page;
				}
				page = page.next;
			}
			return null;
		}

		private void FlushAllPages()
		{
			Db4objects.Db4o.IO.CachedIoAdapter.Page node = _head;
			while (node != null)
			{
				FlushPage(node);
				node = node.next;
			}
		}

		private void FlushPage(Db4objects.Db4o.IO.CachedIoAdapter.Page page)
		{
			if (!page.dirty)
			{
				return;
			}
			IoSeek(page.StartAddress());
			WritePage(page);
			return;
		}

		private void GetPageFromDisk(Db4objects.Db4o.IO.CachedIoAdapter.Page page, long pos
			)
		{
			long startAddress = pos - pos % _pageSize;
			page.StartAddress(startAddress);
			IoSeek(page._startAddress);
			int readCount = _io.Read(page.buffer);
			long endAddress = startAddress + readCount;
			if (readCount >= 0)
			{
				_filePointer = endAddress;
				page.EndAddress(endAddress);
			}
			else
			{
				page.EndAddress(page._startAddress);
			}
		}

		private void MovePageToHead(Db4objects.Db4o.IO.CachedIoAdapter.Page page)
		{
			if (page == _head)
			{
				return;
			}
			if (page == _tail)
			{
				Db4objects.Db4o.IO.CachedIoAdapter.Page tempTail = _tail.prev;
				tempTail.next = null;
				_tail.next = _head;
				_tail.prev = null;
				_head.prev = page;
				_head = _tail;
				_tail = tempTail;
			}
			else
			{
				page.prev.next = page.next;
				page.next.prev = page.prev;
				page.next = _head;
				_head.prev = page;
				page.prev = null;
				_head = page;
			}
		}

		private void WritePage(Db4objects.Db4o.IO.CachedIoAdapter.Page page)
		{
			_io.Write(page.buffer, page.Size());
			_filePointer = page.EndAddress();
			page.dirty = false;
		}

		/// <summary>Moves the pointer to the specified file position</summary>
		/// <param name="pos">position within the file</param>
		public override void Seek(long pos)
		{
			_position = pos;
			_fileLength = System.Math.Max(_fileLength, pos);
		}

		private void IoSeek(long pos)
		{
			if (_filePointer != pos)
			{
				_io.Seek(pos);
				_filePointer = pos;
			}
		}

		private class Page
		{
			internal byte[] buffer;

			internal long _startAddress = -1;

			internal long _endAddress;

			internal int bufferSize;

			internal bool dirty;

			internal Db4objects.Db4o.IO.CachedIoAdapter.Page prev;

			internal Db4objects.Db4o.IO.CachedIoAdapter.Page next;

			public Page(int size)
			{
				bufferSize = size;
				buffer = new byte[bufferSize];
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
				int readBytes = System.Math.Min(pageAvailbeDataSize, length);
				System.Array.Copy(buffer, bufferOffset, @out, outOffset, readBytes);
				return readBytes;
			}

			internal virtual int Write(byte[] data, int dataOffset, long startAddress, int length
				)
			{
				int bufferOffset = (int)(startAddress - _startAddress);
				int pageAvailabeBufferSize = (int)(bufferSize - bufferOffset);
				int writtenBytes = System.Math.Min(pageAvailabeBufferSize, length);
				System.Array.Copy(data, dataOffset, buffer, bufferOffset, writtenBytes);
				long endAddress = startAddress + writtenBytes;
				if (endAddress > _endAddress)
				{
					_endAddress = endAddress;
				}
				dirty = true;
				return writtenBytes;
			}

			internal virtual bool Contains(long address)
			{
				if (_startAddress != -1 && address >= _startAddress && address < _startAddress + 
					bufferSize)
				{
					return true;
				}
				return false;
			}

			internal virtual bool IsFree()
			{
				return _startAddress == -1;
			}
		}
	}
}
