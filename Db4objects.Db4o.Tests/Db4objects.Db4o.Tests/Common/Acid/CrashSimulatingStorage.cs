/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Acid;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingStorage : StorageDecorator
	{
		private readonly string _fileName;

		internal CrashSimulatingBatch _batch;

		public CrashSimulatingStorage(IStorage storage, string fileName) : base(storage)
		{
			_batch = new CrashSimulatingBatch();
			_fileName = fileName;
		}

		protected override IBin Decorate(IBin bin)
		{
			return new CrashSimulatingStorage.CrashSimulatingBin(bin, _batch, _fileName);
		}

		internal class CrashSimulatingBin : BinDecorator
		{
			private readonly string _fileName;

			private CrashSimulatingBatch _batch;

			internal long _curPos;

			public CrashSimulatingBin(IBin bin, CrashSimulatingBatch batch, string fileName) : 
				base(bin)
			{
				_batch = batch;
				_fileName = fileName;
			}

			/// <exception cref="Db4oIOException"></exception>
			public override int Read(long pos, byte[] bytes, int length)
			{
				_curPos = pos;
				int readBytes = base.Read(pos, bytes, length);
				if (readBytes > 0)
				{
					_curPos += readBytes;
				}
				return readBytes;
			}

			/// <exception cref="Db4oIOException"></exception>
			public override void Write(long pos, byte[] buffer, int length)
			{
				_curPos = pos;
				base.Write(pos, buffer, length);
				byte[] copy = new byte[buffer.Length];
				System.Array.Copy(buffer, 0, copy, 0, length);
				_batch.Add(_fileName, copy, _curPos, length);
				_curPos += length;
			}

			/// <exception cref="Db4oIOException"></exception>
			public override void Sync()
			{
				base.Sync();
				_batch.Sync();
			}
		}
	}
}
