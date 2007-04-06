using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class CallbackObjectInfoCollections
	{
		public readonly IObjectInfoCollection added;

		public readonly IObjectInfoCollection updated;

		public readonly IObjectInfoCollection deleted;

		public readonly IServerMessageDispatcher serverMessageDispatcher;

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
