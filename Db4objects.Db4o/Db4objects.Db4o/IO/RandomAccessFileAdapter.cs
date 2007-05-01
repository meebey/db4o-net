using System;
using Db4objects.Db4o;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Sharpen.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>IO adapter for random access files.</summary>
	/// <remarks>IO adapter for random access files.</remarks>
	public class RandomAccessFileAdapter : IoAdapter
	{
		private string _path;

		private RandomAccessFile _delegate;

		public RandomAccessFileAdapter()
		{
		}

		protected RandomAccessFileAdapter(string path, bool lockFile, long initialLength)
		{
			_path = new Sharpen.IO.File(path).GetCanonicalPath();
			_delegate = new RandomAccessFile(_path, "rw");
			if (initialLength > 0)
			{
				_delegate.Seek(initialLength - 1);
				_delegate.Write(new byte[] { 0 });
			}
			if (lockFile)
			{
				try
				{
					Platform4.LockFile(_path, _delegate);
				}
				catch (DatabaseFileLockedException e)
				{
					_delegate.Close();
					throw;
				}
			}
		}

		public override void Close()
		{
			try
			{
				Platform4.UnlockFile(_path, _delegate);
			}
			catch (Exception)
			{
			}
			_delegate.Close();
		}

		public override void Delete(string path)
		{
			new Sharpen.IO.File(path).Delete();
		}

		public override bool Exists(string path)
		{
			Sharpen.IO.File existingFile = new Sharpen.IO.File(path);
			return existingFile.Exists() && existingFile.Length() > 0;
		}

		public override long GetLength()
		{
			return _delegate.Length();
		}

		public override IoAdapter Open(string path, bool lockFile, long initialLength)
		{
			return new Db4objects.Db4o.IO.RandomAccessFileAdapter(path, lockFile, initialLength
				);
		}

		public override int Read(byte[] bytes, int length)
		{
			return _delegate.Read(bytes, 0, length);
		}

		public override void Seek(long pos)
		{
			_delegate.Seek(pos);
		}

		public override void Sync()
		{
			_delegate.GetFD().Sync();
		}

		public override void Write(byte[] buffer, int length)
		{
			_delegate.Write(buffer, 0, length);
		}
	}
}
