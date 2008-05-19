/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Events
{
	internal class EventPlatform
	{
		public static void TriggerClassEvent(ClassEventHandler e, Db4objects.Db4o.Internal.ClassMetadata klass)
		{
			if (null == e) return;
			e(klass, new ClassEventArgs(klass));
		}
		
		public static void TriggerQueryEvent(Transaction transaction, QueryEventHandler e, Db4objects.Db4o.Query.IQuery q)
		{
			if (null == e) return;
			e(q, new QueryEventArgs(transaction, q));
		}

		public static bool TriggerCancellableObjectEventArgs(Transaction transaction, CancellableObjectEventHandler e, object o)
		{
			if (null == e) return true;
			CancellableObjectEventArgs coea = new CancellableObjectEventArgs(transaction, o);
            try
            {
                e(o, coea);
            }
            catch (Db4oException)
            {
                throw;
            }
            catch (System.Exception exception)
            {
                throw new EventException(exception);
            }
			return !coea.IsCancelled;
		}

		public static void TriggerObjectEvent(Transaction transaction, ObjectEventHandler e, object o)
		{
			if (null == e) return;
			e(o, new ObjectEventArgs(transaction, o));
		}
		
		public static void TriggerCommitEvent(Transaction transaction, CommitEventHandler e, CallbackObjectInfoCollections objectInfoCollections)
		{
			if (null == e) return;
            e(null, new CommitEventArgs(transaction, objectInfoCollections));
		}
		
		public static bool HasListeners(System.Delegate e)
		{
			return null != e;
		}

		public static void TriggerObjectContainerEvent(IObjectContainer container, ObjectContainerEventHandler e)
		{
			if (null == e) return;
			e(container, new ObjectContainerEventArgs(container));
		}
	}
}
