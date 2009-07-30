/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Events
{
	/// <summary>Argument for object related events which can be cancelled.</summary>
	/// <remarks>Argument for object related events which can be cancelled.</remarks>
	/// <seealso cref="Db4objects.Db4o.Events.IEventRegistry">Db4objects.Db4o.Events.IEventRegistry
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Events.ICancellableEventArgs">Db4objects.Db4o.Events.ICancellableEventArgs
	/// 	</seealso>
	public class CancellableObjectEventArgs : ObjectInfoEventArgs, ICancellableEventArgs
	{
		private bool _cancelled;

		private object _object;

		/// <summary>Creates a new instance for the specified object.</summary>
		/// <remarks>Creates a new instance for the specified object.</remarks>
		public CancellableObjectEventArgs(Transaction transaction, IObjectInfo objectInfo
			, object obj) : base(transaction, objectInfo)
		{
			_object = obj;
		}

		/// <seealso cref="Db4objects.Db4o.Events.ICancellableEventArgs.Cancel">Db4objects.Db4o.Events.ICancellableEventArgs.Cancel
		/// 	</seealso>
		public virtual void Cancel()
		{
			_cancelled = true;
		}

		/// <seealso cref="Db4objects.Db4o.Events.ICancellableEventArgs.IsCancelled">Db4objects.Db4o.Events.ICancellableEventArgs.IsCancelled
		/// 	</seealso>
		public virtual bool IsCancelled
		{
			get
			{
				return _cancelled;
			}
		}

		public override object Object
		{
			get
			{
				return _object;
			}
		}

		public override IObjectInfo Info
		{
			get
			{
				IObjectInfo info = base.Info;
				if (null == info)
				{
					throw new InvalidOperationException();
				}
				return info;
			}
		}
	}
}
