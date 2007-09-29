/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
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
