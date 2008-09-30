/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

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

		void ReplicationReflector(Db4objects.Drs.Inside.ReplicationReflector replicationReflector
			);
	}
}
