using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;

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

		void QueryOnStarted(IQuery query);

		void QueryOnFinished(IQuery query);

		bool CaresAboutCommitting();

		bool CaresAboutCommitted();

		void CommitOnStarted(object transaction, CallbackObjectInfoCollections objectInfoCollections
			);

		void CommitOnCompleted(object transaction, CallbackObjectInfoCollections objectInfoCollections
			);
	}
}
