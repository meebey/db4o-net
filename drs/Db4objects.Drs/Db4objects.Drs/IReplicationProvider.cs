/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Drs;

namespace Db4objects.Drs
{
	/// <summary>Facade for persistence systems that provide replication support.</summary>
	/// <remarks>
	/// Facade for persistence systems that provide replication support.
	/// Interacts with another ReplicationProvider and a  ReplicationSession
	/// to allows replication of objects between two ReplicationProviders.
	/// &lt;p/&gt;
	/// &lt;p/&gt; To create an instance of this class, use the methods of
	/// <see cref="Db4objects.Drs.Replication">Db4objects.Drs.Replication</see>
	/// .
	/// </remarks>
	/// <author>Albert Kwan</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.2</version>
	/// <seealso cref="IReplicationSession">IReplicationSession</seealso>
	/// <seealso cref="Db4objects.Drs.Replication">Db4objects.Drs.Replication</seealso>
	/// <since>dRS 1.0</since>
	public interface IReplicationProvider
	{
		/// <summary>Returns newly created objects and changed objects since last replication with the opposite provider.
		/// 	</summary>
		/// <remarks>Returns newly created objects and changed objects since last replication with the opposite provider.
		/// 	</remarks>
		/// <returns>newly created objects and changed objects since last replication with the opposite provider.
		/// 	</returns>
		IObjectSet ObjectsChangedSinceLastReplication();

		/// <summary>Returns newly created objects and changed objects since last replication with the opposite provider.
		/// 	</summary>
		/// <remarks>Returns newly created objects and changed objects since last replication with the opposite provider.
		/// 	</remarks>
		/// <param name="clazz">the type of objects interested</param>
		/// <returns>newly created objects and changed objects of the type specified in the clazz parameter since last replication
		/// 	</returns>
		IObjectSet ObjectsChangedSinceLastReplication(Type clazz);
	}
}
