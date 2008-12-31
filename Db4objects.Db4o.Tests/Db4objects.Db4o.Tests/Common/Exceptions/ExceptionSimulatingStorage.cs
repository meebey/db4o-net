/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class ExceptionSimulatingStorage : Db4objects.Db4o.IO.StorageDecorator
	{
		private IoAdapter _delegate = new RandomAccessFileAdapter();

		public static bool exception = false;

		public ExceptionSimulatingStorage(IStorage storage) : base(storage)
		{
		}

		public virtual void Delete(string path)
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

		protected override IBin Decorate(IBin bin)
		{
			return new ExceptionSimulatingStorage.ExceptionSimulatingBin(bin);
		}

		internal class ExceptionSimulatingBin : Db4objects.Db4o.IO.BinDecorator
		{
			public ExceptionSimulatingBin(IBin bin) : base(bin)
			{
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override int Read(long pos, byte[] bytes, int length)
			{
				if (exception)
				{
					throw new Db4oIOException();
				}
				else
				{
					return _bin.Read(pos, bytes, length);
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override void Sync()
			{
				if (exception)
				{
					throw new Db4oIOException();
				}
				else
				{
					_bin.Sync();
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override void Write(long pos, byte[] buffer, int length)
			{
				if (exception)
				{
					throw new Db4oIOException();
				}
				else
				{
					_bin.Write(pos, buffer, length);
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override void Close()
			{
				if (exception)
				{
					throw new Db4oIOException();
				}
				else
				{
					_bin.Close();
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override long Length()
			{
				if (exception)
				{
					throw new Db4oIOException();
				}
				else
				{
					return _bin.Length();
				}
			}
		}
	}
}
