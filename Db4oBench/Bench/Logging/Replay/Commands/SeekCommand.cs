/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Bench.Logging.Replay.Commands;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Bench.Logging.Replay.Commands
{
	public class SeekCommand : IIoCommand
	{
		private readonly int _address;

		public SeekCommand(int address)
		{
			_address = address;
		}

		public virtual void Replay(IoAdapter adapter)
		{
			adapter.Seek(_address);
		}
	}
}
