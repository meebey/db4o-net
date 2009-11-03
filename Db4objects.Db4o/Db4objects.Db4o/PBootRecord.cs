/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <summary>Old database boot record class.</summary>
	/// <remarks>
	/// Old database boot record class.
	/// This class was responsible for storing the last timestamp id,
	/// for holding a reference to the Db4oDatabase object of the
	/// ObjectContainer and for holding on to the UUID index.
	/// This class is no longer needed with the change to the new
	/// fileheader. It still has to stay here to be able to read
	/// old databases.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class PBootRecord : P1Object, IInternal4
	{
		public Db4oDatabase i_db;

		public long i_versionGenerator;

		public MetaIndex i_uuidMetaIndex;

		public virtual MetaIndex GetUUIDMetaIndex()
		{
			return i_uuidMetaIndex;
		}

		public virtual void Write(LocalObjectContainer file)
		{
		}
		// write is still called when storing objects to old
		// database files (CLI1.ObjectInfoMigration52TestCase
		// and CLI1.ObjectInfoMigration57)
	}
}
