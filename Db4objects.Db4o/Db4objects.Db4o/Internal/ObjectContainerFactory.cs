/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	public class ObjectContainerFactory
	{
		/// <exception cref="OldFormatException"></exception>
		public static IObjectContainer OpenObjectContainer(IConfiguration config, string 
			databaseFileName)
		{
			IObjectContainer oc = new IoAdaptedObjectContainer(config, databaseFileName);
			Db4objects.Db4o.Internal.Messages.LogMsg(Db4oFactory.Configure(), 5, databaseFileName
				);
			return oc;
		}
	}
}
