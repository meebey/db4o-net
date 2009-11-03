/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	public class ObjectContainerFactory
	{
		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		public static IEmbeddedObjectContainer OpenObjectContainer(IConfiguration config, 
			string databaseFileName)
		{
			Config4Impl.AssertIsNotTainted(config);
			EmitDebugInfo();
			IEmbeddedObjectContainer oc = new IoAdaptedObjectContainer(config, databaseFileName
				);
			Db4objects.Db4o.Internal.Messages.LogMsg(config, 5, databaseFileName);
			return oc;
		}

		private static void EmitDebugInfo()
		{
		}
	}
}
