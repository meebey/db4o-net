/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public class SynchronizedBin : Db4objects.Db4o.IO.BinDecorator
	{
		public SynchronizedBin(IBin bin) : base(bin)
		{
		}

		public override void Close()
		{
			lock (_bin)
			{
				base.Close();
			}
		}

		public override long Length()
		{
			lock (_bin)
			{
				return base.Length();
			}
		}

		public override int Read(long position, byte[] buffer, int bytesToRead)
		{
			lock (_bin)
			{
				return base.Read(position, buffer, bytesToRead);
			}
		}

		public override void Write(long position, byte[] bytes, int bytesToWrite)
		{
			lock (_bin)
			{
				base.Write(position, bytes, bytesToWrite);
			}
		}

		public override void Sync()
		{
			lock (_bin)
			{
				base.Sync();
			}
		}
	}
}
