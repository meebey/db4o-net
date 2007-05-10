using Db4objects.Db4o;
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

		protected ExceptionIOAdapter(string path, bool lockFile, long initialLength)
		{
			_delegate = _delegate.Open(path, lockFile, initialLength);
		}

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

		public override IoAdapter Open(string path, bool lockFile, long initialLength)
		{
			return new Db4objects.Db4o.Tests.Common.Exceptions.ExceptionIOAdapter(path, lockFile
				, initialLength);
		}

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
