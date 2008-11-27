/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Events
{
	internal class EventPlatform
	{
		public static void TriggerClassEvent(ClassEventHandler e, ClassMetadata klass)
		{
			Trigger(delegate
			{
				if (null == e) return;
				e(klass, new ClassEventArgs(klass));
			});
		}

		public static void TriggerQueryEvent(Transaction transaction, QueryEventHandler e, IQuery q)
		{
			Trigger(delegate
			{
				if (null == e) return;
			    e(q, new QueryEventArgs(transaction, q));
			});
		}

		public static bool TriggerCancellableObjectEventArgs(Transaction transaction, CancellableObjectEventHandler e, object o)
		{
			bool ret = false;
			Trigger(delegate
			{
				if (null == e)
			    {
					ret = true;
			        return;
				}
				
				CancellableObjectEventArgs coea = new CancellableObjectEventArgs(transaction, o);
				
				e(o, coea);
				ret = !coea.IsCancelled;
			});

			return ret;
		}

		public static void TriggerObjectEvent(Transaction transaction, ObjectEventHandler e, object o)
		{
			Trigger(delegate
			{
				if (null == e) return;
				
				e(o, new ObjectEventArgs(transaction, o));
			});
		}
		
		public static void TriggerCommitEvent(Transaction transaction, CommitEventHandler e, CallbackObjectInfoCollections objectInfoCollections)
		{
			Trigger(delegate
			{
				if (null == e) return;
				e(null, new CommitEventArgs(transaction, objectInfoCollections));
			});
		
		}
		
		public static bool HasListeners(System.Delegate e)
		{
			return null != e;
		}

		public static void TriggerObjectContainerEvent(IObjectContainer container, ObjectContainerEventHandler e)
		{
			Trigger(delegate
			{
				if (null == e) return;
				e(container, new ObjectContainerEventArgs(container));
			});
		}

		private static void Trigger(RunnableDelegate  runnable)
		{
            try
            {
                ObjectReference._inCallback.With(true, new DelegateRunnable(runnable));
            }
            catch (Db4oException)
            {
                throw;
            }
            catch (System.Exception exception)
            {
                throw new EventException(exception);
            }
		}
	}

	internal class DelegateRunnable : IRunnable
	{
		public DelegateRunnable(RunnableDelegate block)
		{
			_delegate = block;
		}

		public void Run()
		{
			_delegate();
		}

		private readonly RunnableDelegate _delegate;
	}

	internal delegate void RunnableDelegate();
}
