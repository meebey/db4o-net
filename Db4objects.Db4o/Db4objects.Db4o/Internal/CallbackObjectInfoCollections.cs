/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class CallbackObjectInfoCollections
	{
		public IObjectInfoCollection added;

		public IObjectInfoCollection updated;

		public IObjectInfoCollection deleted;

		[System.NonSerialized]
		public IServerMessageDispatcher serverMessageDispatcher;

		public static readonly Db4objects.Db4o.Internal.CallbackObjectInfoCollections EMTPY
			 = Empty();

		public CallbackObjectInfoCollections(IServerMessageDispatcher serverMessageDispatcher_
			, IObjectInfoCollection added_, IObjectInfoCollection updated_, IObjectInfoCollection
			 deleted_)
		{
			added = added_;
			updated = updated_;
			deleted = deleted_;
			serverMessageDispatcher = serverMessageDispatcher_;
		}

		private static Db4objects.Db4o.Internal.CallbackObjectInfoCollections Empty()
		{
			return new Db4objects.Db4o.Internal.CallbackObjectInfoCollections(null, ObjectInfoCollectionImpl
				.EMPTY, ObjectInfoCollectionImpl.EMPTY, ObjectInfoCollectionImpl.EMPTY);
		}
	}
}
