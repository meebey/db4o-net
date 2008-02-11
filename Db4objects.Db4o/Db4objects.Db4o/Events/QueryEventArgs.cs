/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Events
{
	/// <summary>
	/// Arguments for
	/// <see cref="IQuery">IQuery</see>
	/// related events.
	/// </summary>
	/// <seealso cref="IEventRegistry">IEventRegistry</seealso>
	public class QueryEventArgs : ObjectEventArgs
	{
		/// <summary>
		/// Creates a new instance for the specified
		/// <see cref="IQuery">IQuery</see>
		/// instance.
		/// </summary>
		public QueryEventArgs(Transaction transaction, IQuery q) : base(transaction, q)
		{
		}

		/// <summary>
		/// The
		/// <see cref="IQuery">IQuery</see>
		/// which triggered the event.
		/// </summary>
		public virtual IQuery Query
		{
			get
			{
				return (IQuery)Object;
			}
		}
	}
}
