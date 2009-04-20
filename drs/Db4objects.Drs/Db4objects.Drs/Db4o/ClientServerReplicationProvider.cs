/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;

namespace Db4objects.Drs.Db4o
{
	internal class ClientServerReplicationProvider : Db4objects.Drs.Db4o.FileReplicationProvider
	{
		public ClientServerReplicationProvider(IObjectContainer objectContainer) : base(objectContainer
			, "null")
		{
		}

		public ClientServerReplicationProvider(IObjectContainer objectContainer, string name
			) : base(objectContainer, name)
		{
		}

		protected override void Refresh(object obj)
		{
			_stream.Refresh(obj, 1);
		}
	}
}
