/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Events
{
	/// <summary>Arguments for commit time related events.</summary>
	/// <remarks>Arguments for commit time related events.</remarks>
	/// <seealso cref="IEventRegistry">IEventRegistry</seealso>
	public class CommitEventArgs : TransactionalEventArgs
	{
		private readonly CallbackObjectInfoCollections _collections;

		public CommitEventArgs(Transaction transaction, CallbackObjectInfoCollections collections
			) : base(transaction)
		{
			_collections = collections;
		}

		/// <summary>Returns a iteration</summary>
		public virtual IObjectInfoCollection Added
		{
			get
			{
				return _collections.added;
			}
		}

		public virtual IObjectInfoCollection Deleted
		{
			get
			{
				return _collections.deleted;
			}
		}

		public virtual IObjectInfoCollection Updated
		{
			get
			{
				return _collections.updated;
			}
		}
	}
}
