/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <exclude></exclude>
	public class DebugIoAdapter : Db4objects.Db4o.IO.VanillaIoAdapter
	{
		internal static int counter;

		private static readonly int[] RangeOfInterest = new int[] { 0, 20 };

		public DebugIoAdapter(IoAdapter delegateAdapter) : base(delegateAdapter)
		{
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		protected DebugIoAdapter(IoAdapter delegateAdapter, string path, bool lockFile, long
			 initialLength, bool readOnly) : base(delegateAdapter.Open(path, lockFile, initialLength
			, readOnly))
		{
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override IoAdapter Open(string path, bool lockFile, long initialLength, bool
			 readOnly)
		{
			return new DebugIoAdapter(new RandomAccessFileAdapter(), path, lockFile, initialLength
				, readOnly);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Seek(long pos)
		{
			if (pos >= RangeOfInterest[0] && pos <= RangeOfInterest[1])
			{
				counter++;
				Sharpen.Runtime.Out.WriteLine("seek: " + pos + "  counter: " + counter);
			}
			base.Seek(pos);
		}
	}
}
