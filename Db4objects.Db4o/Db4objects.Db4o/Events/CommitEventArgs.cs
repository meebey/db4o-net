using System;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Events
{
	/// <summary>Arguments for commit time related events.</summary>
	/// <remarks>Arguments for commit time related events.</remarks>
	/// <seealso cref="IEventRegistry">IEventRegistry</seealso>
	public class CommitEventArgs : EventArgs
	{
		private readonly CallbackObjectInfoCollections _collections;

		private readonly object _transaction;

		public CommitEventArgs(object transaction, CallbackObjectInfoCollections collections
			)
		{
			_transaction = transaction;
			_collections = collections;
		}

		/// <summary>The transaction being committed.</summary>
		/// <remarks>The transaction being committed.</remarks>
		public virtual object Transaction
		{
			get
			{
				return _transaction;
			}
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
