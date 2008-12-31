/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Sharpen.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>
	/// Storage adapter to store db4o database data to physical
	/// files on hard disc.
	/// </summary>
	/// <remarks>
	/// Storage adapter to store db4o database data to physical
	/// files on hard disc.
	/// </remarks>
	public class FileStorage : IStorage
	{
		/// <summary>
		/// opens a
		/// <see cref="Db4objects.Db4o.IO.IBin">Db4objects.Db4o.IO.IBin</see>
		/// on the specified URI (file system path).
		/// </summary>
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual IBin Open(BinConfiguration config)
		{
			return new FileStorage.FileBin(config);
		}

		/// <summary>returns true if the specified file system path already exists.</summary>
		/// <remarks>returns true if the specified file system path already exists.</remarks>
		public virtual bool Exists(string uri)
		{
			Sharpen.IO.File file = new Sharpen.IO.File(uri);
			return file.Exists() && file.Length() > 0;
		}

		private class FileBin : IBin
		{
			private readonly string _path;

			private readonly RandomAccessFile _file;

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			internal FileBin(BinConfiguration config)
			{
				bool ok = false;
				try
				{
					_path = new Sharpen.IO.File(config.Uri()).GetCanonicalPath();
					_file = new RandomAccessFile(_path, config.ReadOnly() ? "r" : "rw");
					if (config.InitialLength() > 0)
					{
						Write(config.InitialLength() - 1, new byte[] { 0 }, 1);
					}
					if (config.LockFile())
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

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
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

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
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

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
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

			/// <exception cref="System.IO.IOException"></exception>
			private void Seek(long pos)
			{
				if (DTrace.enabled)
				{
					DTrace.RegularSeek.Log(pos);
				}
				_file.Seek(pos);
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
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

			public virtual int SyncRead(long position, byte[] bytes, int bytesToRead)
			{
				return Read(position, bytes, bytesToRead);
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
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
