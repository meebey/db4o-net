/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Replication;

namespace Db4objects.Db4o.Replication
{
	/// <summary>db4o replication interface.</summary>
	/// <remarks>db4o replication interface.</remarks>
	/// <seealso cref="IExtObjectContainer.ReplicationBegin">IExtObjectContainer.ReplicationBegin
	/// 	</seealso>
	[System.ObsoleteAttribute(@"Since db4o-5.2. Use db4o Replication System (dRS) instead.<br><br>"
		)]
	public interface IReplicationProcess
	{
		/// <summary>
		/// checks if an object has been modified in both ObjectContainers involved
		/// in the replication process since the last time the two ObjectContainers
		/// were replicated.
		/// </summary>
		/// <remarks>
		/// checks if an object has been modified in both ObjectContainers involved
		/// in the replication process since the last time the two ObjectContainers
		/// were replicated.
		/// </remarks>
		/// <param name="obj">- the object to check for a conflict.</param>
		void CheckConflict(object obj);

		/// <summary>commits the replication task to both involved ObjectContainers.</summary>
		/// <remarks>
		/// commits the replication task to both involved ObjectContainers.
		/// &lt;br&gt;&lt;br&gt;Call this method after replication is completed to
		/// write all changes back to the database files. This method
		/// synchronizes both ObjectContainers by setting the transaction
		/// serial number
		/// <see cref="IExtObjectContainer.Version">IExtObjectContainer.Version</see>
		/// on both
		/// ObjectContainers to be equal
		/// to the higher version number among the two. A record with
		/// information about this replication task, including the
		/// synchronized version number is stored to both ObjectContainers
		/// to allow future incremental replication.
		/// </remarks>
		void Commit();

		/// <summary>returns the "peerA" ObjectContainer involved in this ReplicationProcess.
		/// 	</summary>
		/// <remarks>returns the "peerA" ObjectContainer involved in this ReplicationProcess.
		/// 	</remarks>
		IObjectContainer PeerA();

		/// <summary>returns the "peerB" ObjectContainer involved in this ReplicationProcess.
		/// 	</summary>
		/// <remarks>returns the "peerB" ObjectContainer involved in this ReplicationProcess.
		/// 	</remarks>
		IObjectContainer PeerB();

		/// <summary>replicates an object.</summary>
		/// <remarks>
		/// replicates an object.
		/// &lt;br&gt;&lt;br&gt;By default the version number of the object is checked in
		/// both ObjectContainers involved in the replication process. If the
		/// version number has not changed since the last time the two
		/// ObjectContainers were replicated
		/// </remarks>
		/// <param name="obj"></param>
		void Replicate(object obj);

		/// <summary>ends a replication task without committing any changes.</summary>
		/// <remarks>ends a replication task without committing any changes.</remarks>
		void Rollback();

		/// <summary>
		/// modifies the replication policy, what to do on a call to
		/// <see cref="IReplicationProcess.Replicate">IReplicationProcess.Replicate</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;If no direction is set, the replication process will be bidirectional by
		/// default.
		/// </summary>
		/// <param name="relicateFrom">the ObjectContainer to replicate from</param>
		/// <param name="replicateTo">the ObjectContainer to replicate to</param>
		void SetDirection(IObjectContainer relicateFrom, IObjectContainer replicateTo);

		/// <summary>
		/// adds a constraint to the passed Query to query only for objects that
		/// were modified since the last replication process between the two
		/// ObjectContainers involved in this replication process.
		/// </summary>
		/// <remarks>
		/// adds a constraint to the passed Query to query only for objects that
		/// were modified since the last replication process between the two
		/// ObjectContainers involved in this replication process.
		/// </remarks>
		/// <param name="query">the Query to be constrained</param>
		void WhereModified(IQuery query);
	}
}
