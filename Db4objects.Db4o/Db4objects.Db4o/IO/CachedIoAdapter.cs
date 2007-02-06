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
			long endAddress = startAddress + length;
			Db4objects.Db4o.IO.CachedIoAdapter.Page page;
			int readLength;
			int bufferOffset = 0;
			while (startAddress < endAddress)
			{
				page = GetPage(startAddress, true);
				readLength = (int)(_pageSize - startAddress % _pageSize);
				readLength = System.Math.Min(buffer.Length - bufferOffset, readLength);
				readLength = System.Math.Min(length, readLength);
				page.Read(buffer, bufferOffset, startAddress, readLength);
				MovePageToHead(page);
				startAddress += readLength;
				bufferOffset += readLength;
			}
			_position = endAddress;
			return length;
		}

		/// <summary>Writes the buffer to cache using pages</summary>
		/// <param name="buffer">source buffer</param>
		/// <param name="length">how many bytes to write</param>
		public override void Write(byte[] buffer, int length)
		{
			long startAddress = _position;
			long endAddress = startAddress + length;
			Db4objects.Db4o.IO.CachedIoAdapter.Page page = null;
			int writtenLength;
			int bufferOffset = 0;
			while (startAddress < endAddress)
			{
				writtenLength = (int)(_pageSize - startAddress % _pageSize);
				writtenLength = System.Math.Min(writtenLength, length);
				writtenLength = System.Math.Min(buffer.Length - bufferOffset, writtenLength);
				bool load = writtenLength != _pageSize;
				page = GetPage(startAddress, load);
				page.Write(buffer, bufferOffset, startAddress, writtenLength);
				MovePageToHead(page);
				startAddress += writtenLength;
				bufferOffset += writtenLength;
			}
			_position = endAddress;
			_fileLength = System.Math.Max(page.startPosition + _pageSize, _fileLength);
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

		private Db4objects.Db4o.IO.CachedIoAdapter.Page GetPage(long startAddress, bool load
			)
		{
			Db4objects.Db4o.IO.CachedIoAdapter.Page page;
			page = GetPageFromCache(startAddress);
			if (page == null)
			{
				page = GetFreePage();
				if (load)
				{
					LoadPage(page, startAddress);
				}
				else
				{
					page.startPosition = startAddress;
				}
			}
			return page;
		}

		private Db4objects.Db4o.IO.CachedIoAdapter.Page GetFreePage()
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
			IoSeek(page.startPosition);
			WritePage(page);
			return;
		}

		private void LoadPage(Db4objects.Db4o.IO.CachedIoAdapter.Page page, long pos)
		{
			page.startPosition = pos - pos % _pageSize;
			IoSeek(page.startPosition);
			int readCount = _io.Read(page.buffer);
			if (readCount > 0)
			{
				_filePointer += readCount;
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
			_io.Write(page.buffer);
			_filePointer += _pageSize;
			page.dirty = false;
		}

		/// <summary>Moves the pointer to the specified file position</summary>
		/// <param name="pos">position within the file</param>
		public override void Seek(long pos)
		{
			_position = pos;
			long endAddress = pos - pos % _pageSize + _pageSize;
			_fileLength = System.Math.Max(_fileLength, endAddress);
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
			public byte[] buffer;

			public long startPosition = -1;

			public int size;

			public bool dirty;

			internal Db4objects.Db4o.IO.CachedIoAdapter.Page prev;

			internal Db4objects.Db4o.IO.CachedIoAdapter.Page next;

			public Page(int size)
			{
				buffer = new byte[size];
				this.size = size;
			}

			public virtual void Read(byte[] @out, int outOffset, long startPosition, int length
				)
			{
				int bufferOffset = (int)(startPosition - this.startPosition);
				System.Array.Copy(buffer, bufferOffset, @out, outOffset, length);
			}

			public virtual void Write(byte[] data, int dataOffset, long startPosition, int length
				)
			{
				int bufferOffset = (int)(startPosition - this.startPosition);
				System.Array.Copy(data, dataOffset, buffer, bufferOffset, length);
				dirty = true;
			}

			public virtual bool Contains(long address)
			{
				if (startPosition != -1 && address >= startPosition && address < startPosition + 
					size)
				{
					return true;
				}
				return false;
			}

			public virtual bool IsFree()
			{
				return startPosition == -1;
			}
		}
	}
}
