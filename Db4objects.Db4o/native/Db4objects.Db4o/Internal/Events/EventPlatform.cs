/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal.Events
{
	internal class EventPlatform
	{
		public static void TriggerQueryEvent(QueryEventHandler e, Db4objects.Db4o.Query.IQuery q)
		{
			if (null == e) return;
			e(q, new QueryEventArgs(q));
		}

		public static bool TriggerCancellableObjectEventArgs(CancellableObjectEventHandler e, object o)
		{
			if (null == e) return true;
			CancellableObjectEventArgs coea = new CancellableObjectEventArgs(o);
			e(o, coea);
			return !coea.IsCancelled;
		}

		public static void TriggerObjectEvent(ObjectEventHandler e, object o)
		{
			if (null == e) return;
			e(o, new ObjectEventArgs(o));
		}
		
		public static void TriggerCommitEvent(CommitEventHandler e, IObjectInfoCollection added, IObjectInfoCollection deleted, IObjectInfoCollection updated)
		{
			if (null == e) return;
			e(null, new CommitEventArgs(added, deleted, updated));
		}
		
		public static bool HasListeners(System.Delegate e)
		{
			return null != e;
		}
	}
}
