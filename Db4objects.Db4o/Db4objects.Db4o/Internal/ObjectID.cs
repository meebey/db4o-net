/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ObjectID
	{
		public readonly int _id;

		public static readonly Db4objects.Db4o.Internal.ObjectID IsNull = new Db4objects.Db4o.Internal.ObjectID
			(-1);

		public static readonly Db4objects.Db4o.Internal.ObjectID NotPossible = new Db4objects.Db4o.Internal.ObjectID
			(-2);

		public static readonly Db4objects.Db4o.Internal.ObjectID Ignore = new Db4objects.Db4o.Internal.ObjectID
			(-3);

		public ObjectID(int id)
		{
			_id = id;
		}

		public virtual bool IsValid()
		{
			return _id > 0;
		}
	}
}
