namespace Db4objects.Db4o.Internal.Callbacks
{
	public interface ICallbacks
	{
		bool ObjectCanNew(object obj);

		bool ObjectCanActivate(object obj);

		bool ObjectCanUpdate(object obj);

		bool ObjectCanDelete(object obj);

		bool ObjectCanDeactivate(object obj);

		void ObjectOnActivate(object obj);

		void ObjectOnNew(object obj);

		void ObjectOnUpdate(object obj);

		void ObjectOnDelete(object obj);

		void ObjectOnDeactivate(object obj);

		void QueryOnStarted(Db4objects.Db4o.Query.IQuery query);

		void QueryOnFinished(Db4objects.Db4o.Query.IQuery query);

		bool CaresAboutCommit();

		void CommitOnStarted(object transaction, Db4objects.Db4o.Ext.IObjectInfoCollection
			 added, Db4objects.Db4o.Ext.IObjectInfoCollection deleted, Db4objects.Db4o.Ext.IObjectInfoCollection
			 updated);
	}
}
