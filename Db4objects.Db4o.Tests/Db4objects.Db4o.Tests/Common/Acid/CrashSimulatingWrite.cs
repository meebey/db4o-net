/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Sharpen.IO;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingWrite
	{
		internal byte[] data;

		internal long offset;

		internal int length;

		public CrashSimulatingWrite(byte[] data, long offset, int length)
		{
			this.data = data;
			this.offset = offset;
			this.length = length;
		}

		public virtual void Write(RandomAccessFile raf)
		{
			raf.Seek(offset);
			raf.Write(data, 0, length);
		}

		public override string ToString()
		{
			return "A " + offset + " L " + length;
		}
	}
}
