/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs
{
	/// <summary>Factory to create ReplicationSessions.</summary>
	/// <remarks>Factory to create ReplicationSessions.</remarks>
	/// <author>Albert Kwan</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.2</version>
	/// <seealso cref="com.db4o.drs.hibernate.HibernateReplication">com.db4o.drs.hibernate.HibernateReplication
	/// 	</seealso>
	/// <seealso cref="Db4objects.Drs.IReplicationProvider">Db4objects.Drs.IReplicationProvider
	/// 	</seealso>
	/// <seealso cref="Db4objects.Drs.IReplicationEventListener">Db4objects.Drs.IReplicationEventListener
	/// 	</seealso>
	/// <since>dRS 1.0</since>
	public class Replication
	{
		/// <summary>Begins a replication session between two ReplicationProviders without ReplicationEventListener.
		/// 	</summary>
		/// <remarks>Begins a replication session between two ReplicationProviders without ReplicationEventListener.
		/// 	</remarks>
		/// <exception cref="Db4objects.Drs.ReplicationConflictException">when conflicts occur
		/// 	</exception>
		/// <seealso cref="Db4objects.Drs.IReplicationEventListener">Db4objects.Drs.IReplicationEventListener
		/// 	</seealso>
		public static Db4objects.Drs.IReplicationSession Begin(Db4objects.Drs.IReplicationProvider
			 providerA, Db4objects.Drs.IReplicationProvider providerB)
		{
			return Begin(providerA, providerB, null);
		}

		/// <summary>Begins a replication session between db4o and db4o without ReplicationEventListener.
		/// 	</summary>
		/// <remarks>Begins a replication session between db4o and db4o without ReplicationEventListener.
		/// 	</remarks>
		/// <exception cref="Db4objects.Drs.ReplicationConflictException">when conflicts occur
		/// 	</exception>
		/// <seealso cref="Db4objects.Drs.IReplicationEventListener">Db4objects.Drs.IReplicationEventListener
		/// 	</seealso>
		public static Db4objects.Drs.IReplicationSession Begin(Db4objects.Db4o.IObjectContainer
			 oc1, Db4objects.Db4o.IObjectContainer oc2)
		{
			return Begin(oc1, oc2, null);
		}

		/// <summary>Begins a replication session between two ReplicatoinProviders.</summary>
		/// <remarks>Begins a replication session between two ReplicatoinProviders.</remarks>
		public static Db4objects.Drs.IReplicationSession Begin(Db4objects.Drs.IReplicationProvider
			 providerA, Db4objects.Drs.IReplicationProvider providerB, Db4objects.Drs.IReplicationEventListener
			 listener)
		{
			if (listener == null)
			{
				listener = new Db4objects.Drs.Inside.DefaultReplicationEventListener();
			}
			return new Db4objects.Drs.Inside.GenericReplicationSession(providerA, providerB, 
				listener);
		}

		/// <summary>Begins a replication session between db4o and db4o.</summary>
		/// <remarks>Begins a replication session between db4o and db4o.</remarks>
		public static Db4objects.Drs.IReplicationSession Begin(Db4objects.Db4o.IObjectContainer
			 oc1, Db4objects.Db4o.IObjectContainer oc2, Db4objects.Drs.IReplicationEventListener
			 listener)
		{
			return Begin(Db4objects.Drs.Db4o.Db4oProviderFactory.NewInstance(oc1), Db4objects.Drs.Db4o.Db4oProviderFactory
				.NewInstance(oc2), listener);
		}
	}
}
