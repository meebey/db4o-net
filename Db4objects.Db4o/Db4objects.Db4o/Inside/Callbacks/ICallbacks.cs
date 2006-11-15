namespace Db4objects.Db4o.Inside.Callbacks
{
	public interface ICallbacks
	{
		void OnQueryStarted(Db4objects.Db4o.Query.IQuery query);

		void OnQueryFinished(Db4objects.Db4o.Query.IQuery query);

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
	}
}
