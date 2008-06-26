/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Bench.Logging.Replay.Commands;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Bench.Logging.Replay.Commands
{
	public class ReadCommand : Db4objects.Db4o.Bench.Logging.Replay.Commands.ReadWriteCommand
		, IIoCommand
	{
		public ReadCommand(int length) : base(length)
		{
		}

		public virtual void Replay(IoAdapter adapter)
		{
			adapter.Read(PrepareBuffer(), _length);
		}
	}
}
