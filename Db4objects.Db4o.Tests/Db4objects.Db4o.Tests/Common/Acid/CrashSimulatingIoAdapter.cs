namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingIoAdapter : Db4objects.Db4o.IO.VanillaIoAdapter
	{
		internal Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingBatch batch;

		internal long curPos;

		public CrashSimulatingIoAdapter(Db4objects.Db4o.IO.IoAdapter delegateAdapter) : base
			(delegateAdapter)
		{
			batch = new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingBatch();
		}

		private CrashSimulatingIoAdapter(Db4objects.Db4o.IO.IoAdapter delegateAdapter, string
			 path, bool lockFile, long initialLength, Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingBatch
			 batch) : base(delegateAdapter.Open(path, lockFile, initialLength))
		{
			this.batch = batch;
		}

		public override Db4objects.Db4o.IO.IoAdapter Open(string path, bool lockFile, long
			 initialLength)
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
