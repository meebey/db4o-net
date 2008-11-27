/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.IO
{
	public interface IBin
	{
		long Length();

		int Read(long position, byte[] bytes, int bytesToRead);

		void Write(long position, byte[] bytes, int bytesToWrite);

		void Sync();

		void Close();
	}
}
