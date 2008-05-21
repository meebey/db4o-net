/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Drs;
using Db4objects.Drs.Db4o;
using Db4objects.Drs.Inside;

namespace Db4objects.Drs
{
	/// <summary>Factory to create ReplicationSessions.</summary>
	/// <remarks>Factory to create ReplicationSessions.</remarks>
	/// <author>Albert Kwan</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.2</version>
	/// <seealso cref="com.db4o.drs.hibernate.HibernateReplication">com.db4o.drs.hibernate.HibernateReplication
	/// 	</seealso>
	/// <seealso cref="IReplicationProvider">IReplicationProvider</seealso>
	/// <seealso cref="IReplicationEventListener">IReplicationEventListener</seealso>
	/// <since>dRS 1.0</since>
	public class Replication
	{
		/// <summary>Begins a replication session between two ReplicationProviders without ReplicationEventListener.
		/// 	</summary>
		/// <remarks>Begins a replication session between two ReplicationProviders without ReplicationEventListener.
		/// 	</remarks>
		/// <exception cref="ReplicationConflictException">when conflicts occur</exception>
		/// <seealso cref="IReplicationEventListener">IReplicationEventListener</seealso>
		public static IReplicationSession Begin(IReplicationProvider providerA, IReplicationProvider
			 providerB)
		{
			return Begin(providerA, providerB, null);
		}

		/// <summary>Begins a replication session between db4o and db4o without ReplicationEventListener.
		/// 	</summary>
		/// <remarks>Begins a replication session between db4o and db4o without ReplicationEventListener.
		/// 	</remarks>
		/// <exception cref="ReplicationConflictException">when conflicts occur</exception>
		/// <seealso cref="IReplicationEventListener">IReplicationEventListener</seealso>
		public static IReplicationSession Begin(IObjectContainer oc1, IObjectContainer oc2
			)
		{
			return Begin(oc1, oc2, null);
		}

		/// <summary>Begins a replication session between two ReplicatoinProviders.</summary>
		/// <remarks>Begins a replication session between two ReplicatoinProviders.</remarks>
		public static IReplicationSession Begin(IReplicationProvider providerA, IReplicationProvider
			 providerB, IReplicationEventListener listener)
		{
			if (listener == null)
			{
				listener = new DefaultReplicationEventListener();
			}
			ReplicationReflector reflector = new ReplicationReflector(providerA, providerB);
			providerA.ReplicationReflector(reflector);
			providerB.ReplicationReflector(reflector);
			return new GenericReplicationSession(providerA, providerB, listener);
		}

		/// <summary>Begins a replication session between db4o and db4o.</summary>
		/// <remarks>Begins a replication session between db4o and db4o.</remarks>
		public static IReplicationSession Begin(IObjectContainer oc1, IObjectContainer oc2
			, IReplicationEventListener listener)
		{
			return Begin(Db4oProviderFactory.NewInstance(oc1), Db4oProviderFactory.NewInstance
				(oc2), listener);
		}
	}
}
