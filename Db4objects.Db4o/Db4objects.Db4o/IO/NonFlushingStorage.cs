/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public class NonFlushingStorage : Db4objects.Db4o.IO.StorageDecorator
	{
		public NonFlushingStorage(IStorage storage) : base(storage)
		{
		}

		protected override IBin Decorate(IBin storage)
		{
			return new NonFlushingStorage.NonFlushingBin(storage);
		}

		private class NonFlushingBin : Db4objects.Db4o.IO.BinDecorator
		{
			public NonFlushingBin(IBin storage) : base(storage)
			{
			}

			public override void Sync()
			{
			}
		}
	}
}
