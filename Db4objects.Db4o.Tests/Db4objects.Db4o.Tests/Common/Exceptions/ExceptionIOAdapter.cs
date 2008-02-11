/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class ExceptionIOAdapter : IoAdapter
	{
		private IoAdapter _delegate = new RandomAccessFileAdapter();

		public static bool exception = false;

		public ExceptionIOAdapter()
		{
		}

		/// <exception cref="Db4oIOException"></exception>
		protected ExceptionIOAdapter(string path, bool lockFile, long initialLength)
		{
			_delegate = _delegate.Open(path, lockFile, initialLength, false);
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Close()
		{
			if (exception)
			{
				throw new Db4oIOException();
			}
			else
			{
				_delegate.Close();
			}
		}

		public override void Delete(string path)
		{
			if (exception)
			{
				return;
			}
			else
			{
				_delegate.Delete(path);
			}
		}

		public override bool Exists(string path)
		{
			if (exception)
			{
				return false;
			}
			else
			{
				return _delegate.Exists(path);
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		public override long GetLength()
		{
			if (exception)
			{
				throw new Db4oIOException();
			}
			else
			{
				return _delegate.GetLength();
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		public override IoAdapter Open(string path, bool lockFile, long initialLength, bool
			 readOnly)
		{
			return new Db4objects.Db4o.Tests.Common.Exceptions.ExceptionIOAdapter(path, lockFile
				, initialLength);
		}

		/// <exception cref="Db4oIOException"></exception>
		public override int Read(byte[] bytes, int length)
		{
			if (exception)
			{
				throw new Db4oIOException();
			}
			else
			{
				return _delegate.Read(bytes, length);
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Seek(long pos)
		{
			if (exception)
			{
				throw new Db4oIOException();
			}
			else
			{
				_delegate.Seek(pos);
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Sync()
		{
			if (exception)
			{
				throw new Db4oIOException();
			}
			else
			{
				_delegate.Sync();
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Write(byte[] buffer, int length)
		{
			if (exception)
			{
				throw new Db4oIOException();
			}
			else
			{
				_delegate.Write(buffer, length);
			}
		}
	}
}
