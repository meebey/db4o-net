/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Bench.Delaying;
using Db4objects.Db4o.Bench.Timing;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Bench.Delaying
{
	public class DelayingIoAdapter : Db4objects.Db4o.IO.VanillaIoAdapter
	{
		private static Delays _delays = new Delays(0, 0, 0, 0);

		private TicksTiming _timing;

		public DelayingIoAdapter(IoAdapter delegateAdapter) : this(delegateAdapter, _delays
			)
		{
		}

		public DelayingIoAdapter(IoAdapter delegateAdapter, Delays delays) : base(delegateAdapter
			)
		{
			_delays = delays;
			_timing = new TicksTiming();
		}

		/// <exception cref="Db4oIOException"></exception>
		public DelayingIoAdapter(IoAdapter delegateAdapter, string path, bool lockFile, long
			 initialLength) : this(delegateAdapter, path, lockFile, initialLength, _delays)
		{
		}

		/// <exception cref="Db4oIOException"></exception>
		public DelayingIoAdapter(IoAdapter delegateAdapter, string path, bool lockFile, long
			 initialLength, Delays delays) : this(delegateAdapter.Open(path, lockFile, initialLength
			, false), delays)
		{
		}

		/// <exception cref="Db4oIOException"></exception>
		public override IoAdapter Open(string path, bool lockFile, long initialLength, bool
			 readOnly)
		{
			return new Db4objects.Db4o.Bench.Delaying.DelayingIoAdapter(_delegate, path, lockFile
				, initialLength);
		}

		/// <exception cref="Db4oIOException"></exception>
		public override int Read(byte[] bytes, int length)
		{
			Delay(_delays.values[Delays.Read]);
			return _delegate.Read(bytes, length);
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Seek(long pos)
		{
			Delay(_delays.values[Delays.Seek]);
			_delegate.Seek(pos);
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Sync()
		{
			Delay(_delays.values[Delays.Sync]);
			_delegate.Sync();
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Write(byte[] buffer, int length)
		{
			Delay(_delays.values[Delays.Write]);
			_delegate.Write(buffer, length);
		}

		private void Delay(long time)
		{
			_timing.WaitTicks(time);
		}
	}
}
