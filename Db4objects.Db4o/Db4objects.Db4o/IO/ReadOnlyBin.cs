/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public class ReadOnlyBin : Db4objects.Db4o.IO.BinDecorator
	{
		public ReadOnlyBin(IBin storage) : base(storage)
		{
		}

		public override void Write(long position, byte[] bytes, int bytesToWrite)
		{
			throw new Db4oIOException();
		}
	}
}
