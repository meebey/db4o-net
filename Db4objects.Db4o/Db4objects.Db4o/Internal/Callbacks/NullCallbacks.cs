using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Callbacks
{
	public class NullCallbacks : ICallbacks
	{
		public virtual void QueryOnFinished(IQuery query)
		{
		}

		public virtual void QueryOnStarted(IQuery query)
		{
		}

		public virtual bool ObjectCanNew(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanActivate(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanUpdate(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanDelete(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanDeactivate(object obj)
		{
			return true;
		}

		public virtual void ObjectOnNew(object obj)
		{
		}

		public virtual void ObjectOnActivate(object obj)
		{
		}

		public virtual void ObjectOnUpdate(object obj)
		{
		}

		public virtual void ObjectOnDelete(object obj)
		{
		}

		public virtual void ObjectOnDeactivate(object obj)
		{
		}

		public virtual void ObjectOnInstantiate(object obj)
		{
		}

		public virtual void CommitOnStarted(object transaction, CallbackObjectInfoCollections
			 objectInfoCollections)
		{
		}

		public virtual void CommitOnCompleted(object transaction, CallbackObjectInfoCollections
			 objectInfoCollections)
		{
		}

		public virtual bool CaresAboutCommitting()
		{
			return false;
		}

		public virtual bool CaresAboutCommitted()
		{
			return false;
		}

		public virtual void ClassOnRegistered(ClassMetadata clazz)
		{
		}
	}
}
