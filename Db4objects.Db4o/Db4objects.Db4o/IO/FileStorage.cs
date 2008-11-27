/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Sharpen.IO;

namespace Db4objects.Db4o.IO
{
	public class FileStorage : IStorage
	{
		/// <exception cref="Db4oIOException"></exception>
		public virtual IBin Open(string uri, bool lockFile, long initialLength, bool readOnly
			)
		{
			return new FileStorage.FileBin(uri, lockFile, initialLength, readOnly);
		}

		public virtual bool Exists(string uri)
		{
			Sharpen.IO.File file = new Sharpen.IO.File(uri);
			return file.Exists() && file.Length() > 0;
		}

		internal class FileBin : IBin
		{
			private readonly string _path;

			private readonly RandomAccessFile _file;

			/// <exception cref="Db4oIOException"></exception>
			internal FileBin(string path, bool lockFile, long initialLength, bool readOnly)
			{
				bool ok = false;
				try
				{
					_path = new Sharpen.IO.File(path).GetCanonicalPath();
					_file = new RandomAccessFile(_path, readOnly ? "r" : "rw");
					if (initialLength > 0)
					{
						Write(initialLength - 1, new byte[] { 0 }, 1);
					}
					if (lockFile)
					{
						Platform4.LockFile(_path, _file);
					}
					ok = true;
				}
				catch (IOException e)
				{
					throw new Db4oIOException(e);
				}
				finally
				{
					if (!ok)
					{
						Close();
					}
				}
			}

			/// <exception cref="Db4oIOException"></exception>
			public virtual void Close()
			{
				// TODO: use separate subclass for Android with the fix
				// 
				// FIXME: This is a temporary quickfix for a bug in Android.
				//        Remove after Android has been fixed.
				try
				{
					if (_file != null)
					{
						_file.Seek(0);
					}
				}
				catch (IOException)
				{
				}
				// ignore
				Platform4.UnlockFile(_path, _file);
				try
				{
					if (_file != null)
					{
						_file.Close();
					}
				}
				catch (IOException e)
				{
					throw new Db4oIOException(e);
				}
			}

			/// <exception cref="Db4oIOException"></exception>
			public virtual long Length()
			{
				try
				{
					return _file.Length();
				}
				catch (IOException e)
				{
					throw new Db4oIOException(e);
				}
			}

			/// <exception cref="Db4oIOException"></exception>
			public virtual int Read(long pos, byte[] bytes, int length)
			{
				try
				{
					Seek(pos);
					return _file.Read(bytes, 0, length);
				}
				catch (IOException e)
				{
					throw new Db4oIOException(e);
				}
			}

			/// <exception cref="IOException"></exception>
			private void Seek(long pos)
			{
				if (DTrace.enabled)
				{
					DTrace.RegularSeek.Log(pos);
				}
				_file.Seek(pos);
			}

			/// <exception cref="Db4oIOException"></exception>
			public virtual void Sync()
			{
				try
				{
					_file.GetFD().Sync();
				}
				catch (IOException e)
				{
					throw new Db4oIOException(e);
				}
			}

			/// <exception cref="Db4oIOException"></exception>
			public virtual void Write(long pos, byte[] buffer, int length)
			{
				try
				{
					Seek(pos);
					_file.Write(buffer, 0, length);
				}
				catch (IOException e)
				{
					throw new Db4oIOException(e);
				}
			}
		}
	}
}
