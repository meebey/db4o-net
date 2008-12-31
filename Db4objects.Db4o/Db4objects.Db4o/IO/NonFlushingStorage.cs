/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>
	/// Storage adapter that does not pass flush calls
	/// on to its delegate.
	/// </summary>
	/// <remarks>
	/// Storage adapter that does not pass flush calls
	/// on to its delegate.
	/// You can use this
	/// <see cref="Db4objects.Db4o.IO.IStorage">Db4objects.Db4o.IO.IStorage</see>
	/// for improved db4o
	/// speed at the risk of corrupted database files in
	/// case of system failure.
	/// </remarks>
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
