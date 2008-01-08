/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Callbacks
{
	public interface ICallbacks
	{
		bool ObjectCanNew(Transaction transaction, object obj);

		bool ObjectCanActivate(Transaction transaction, object obj);

		bool ObjectCanUpdate(Transaction transaction, object obj);

		bool ObjectCanDelete(Transaction transaction, object obj);

		bool ObjectCanDeactivate(Transaction transaction, object obj);

		void ObjectOnActivate(Transaction transaction, object obj);

		void ObjectOnNew(Transaction transaction, object obj);

		void ObjectOnUpdate(Transaction transaction, object obj);

		void ObjectOnDelete(Transaction transaction, object obj);

		void ObjectOnDeactivate(Transaction transaction, object obj);

		void ObjectOnInstantiate(Transaction transaction, object obj);

		void QueryOnStarted(Transaction transaction, IQuery query);

		void QueryOnFinished(Transaction transaction, IQuery query);

		bool CaresAboutCommitting();

		bool CaresAboutCommitted();

		void ClassOnRegistered(ClassMetadata clazz);

		void CommitOnStarted(Transaction transaction, CallbackObjectInfoCollections objectInfoCollections
			);

		void CommitOnCompleted(Transaction transaction, CallbackObjectInfoCollections objectInfoCollections
			);

		bool CaresAboutDeleting();

		bool CaresAboutDeleted();

		void CloseOnStarted(IObjectContainer container);
	}
}
