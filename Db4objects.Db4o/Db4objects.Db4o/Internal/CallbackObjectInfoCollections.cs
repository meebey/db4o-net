/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class CallbackObjectInfoCollections
	{
		public IObjectInfoCollection added;

		public IObjectInfoCollection updated;

		public IObjectInfoCollection deleted;

		public static readonly Db4objects.Db4o.Internal.CallbackObjectInfoCollections Emtpy
			 = Empty();

		public CallbackObjectInfoCollections(IObjectInfoCollection added_, IObjectInfoCollection
			 updated_, IObjectInfoCollection deleted_)
		{
			added = added_;
			updated = updated_;
			deleted = deleted_;
		}

		private static Db4objects.Db4o.Internal.CallbackObjectInfoCollections Empty()
		{
			return new Db4objects.Db4o.Internal.CallbackObjectInfoCollections(ObjectInfoCollectionImpl
				.Empty, ObjectInfoCollectionImpl.Empty, ObjectInfoCollectionImpl.Empty);
		}
	}
}
