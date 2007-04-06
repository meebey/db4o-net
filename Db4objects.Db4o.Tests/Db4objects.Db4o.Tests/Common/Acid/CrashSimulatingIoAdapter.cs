using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Acid;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingIoAdapter : VanillaIoAdapter
	{
		internal CrashSimulatingBatch batch;

		internal long curPos;

		public CrashSimulatingIoAdapter(IoAdapter delegateAdapter) : base(delegateAdapter
			)
		{
			batch = new CrashSimulatingBatch();
		}

		private CrashSimulatingIoAdapter(IoAdapter delegateAdapter, string path, bool lockFile
			, long initialLength, CrashSimulatingBatch batch) : base(delegateAdapter.Open(path
			, lockFile, initialLength))
		{
			this.batch = batch;
		}

		public override IoAdapter Open(string path, bool lockFile, long initialLength)
		{
			return new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingIoAdapter(_delegate, 
				path, lockFile, initialLength, batch);
		}

		public override void Seek(long pos)
		{
			curPos = pos;
			base.Seek(pos);
		}

		public override void Write(byte[] buffer, int length)
		{
			base.Write(buffer, length);
			byte[] copy = new byte[buffer.Length];
			System.Array.Copy(buffer, 0, copy, 0, buffer.Length);
			batch.Add(copy, curPos, length);
		}

		public override void Sync()
		{
			base.Sync();
			batch.Sync();
		}
	}
}
