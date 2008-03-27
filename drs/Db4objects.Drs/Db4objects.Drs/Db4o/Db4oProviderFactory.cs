/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Db4o
{
	/// <exclude></exclude>
	public class Db4oProviderFactory
	{
		public static Db4objects.Drs.Db4o.IDb4oReplicationProvider NewInstance(Db4objects.Db4o.IObjectContainer
			 oc, string name)
		{
			if (oc is Db4objects.Db4o.Internal.CS.ClientObjectContainer)
			{
				return new Db4objects.Drs.Db4o.ClientServerReplicationProvider(oc, name);
			}
			else
			{
				return new Db4objects.Drs.Db4o.FileReplicationProvider(oc, name);
			}
		}

		public static Db4objects.Drs.Db4o.IDb4oReplicationProvider NewInstance(Db4objects.Db4o.IObjectContainer
			 oc)
		{
			return NewInstance(oc, null);
		}
	}
}
