/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Bench.Logging.Replay.Commands
{
	public class ReadWriteCommand
	{
		protected readonly int _length;

		public ReadWriteCommand(int length)
		{
			_length = length;
		}

		protected virtual byte[] PrepareBuffer()
		{
			return new byte[_length];
		}
	}
}
