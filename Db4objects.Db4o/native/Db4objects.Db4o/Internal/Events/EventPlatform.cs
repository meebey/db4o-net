/* Copyright (C) 2006   Versant Inc.   http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Events
{
	internal class EventPlatform
	{
		public static void TriggerClassEvent(System.EventHandler<ClassEventArgs> e, ClassMetadata klass)
		{
			if (null == e) return;

			WithExceptionHandling(delegate
			{
				e(klass, new ClassEventArgs(klass));
			});
		}

		public static void TriggerQueryEvent(Transaction transaction, System.EventHandler<QueryEventArgs> e, IQuery q)
		{
			if (null == e) return;

			WithExceptionHandling(delegate
			{
				e(q, new QueryEventArgs(transaction, q));
			});
		}

		public static bool TriggerCancellableObjectEventArgs(Transaction transaction, System.EventHandler<CancellableObjectEventArgs> e, IObjectInfo objectInfo, object o)
		{
			if (null == e) return true;
			
			bool ret = false;
			Trigger(delegate
			{
				
				CancellableObjectEventArgs coea = new CancellableObjectEventArgs(transaction, objectInfo, o);
				
				e(o, coea);
				ret = !coea.IsCancelled;
			});

			return ret;
		}

		public static void TriggerObjectInfoEvent(Transaction transaction, System.EventHandler<ObjectInfoEventArgs> e, IObjectInfo o)
		{
			if (null == e) return;
				
			Trigger(delegate
			{
				
				e(o, new ObjectInfoEventArgs(transaction, o));
			});
		}
		
		public static void TriggerCommitEvent(Transaction transaction, System.EventHandler<CommitEventArgs> e, CallbackObjectInfoCollections objectInfoCollections)
		{
			if (null == e) return;

			Trigger(delegate
			{	
				e(null, new CommitEventArgs(transaction, objectInfoCollections));
			});
		
		}
		
		public static bool HasListeners(System.Delegate e)
		{
			return null != e;
		}

		public static void TriggerObjectContainerEvent(IObjectContainer container, System.EventHandler<ObjectContainerEventArgs> e)
		{
			if (null == e) return;

			Trigger(delegate
			{
				e(container, new ObjectContainerEventArgs(container));
			});
		}

		private static void Trigger(RunnableDelegate  runnable)
		{
			WithExceptionHandling(delegate
			{
				InCallbackState._inCallback.With(true, new DelegateRunnable(runnable));
			});
		}

		private static void WithExceptionHandling(RunnableDelegate runnable)
		{
			try
            {
            	runnable();
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
