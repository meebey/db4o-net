/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public class BinDecorator : IBin
	{
		protected readonly IBin _bin;

		public BinDecorator(IBin bin)
		{
			_bin = bin;
		}

		public virtual void Close()
		{
			_bin.Close();
		}

		public virtual long Length()
		{
			return _bin.Length();
		}

		public virtual int Read(long position, byte[] buffer, int bytesToRead)
		{
			return _bin.Read(position, buffer, bytesToRead);
		}

		public virtual void Sync()
		{
			_bin.Sync();
		}

		public virtual void Write(long position, byte[] bytes, int bytesToWrite)
		{
			_bin.Write(position, bytes, bytesToWrite);
		}
	}
}
