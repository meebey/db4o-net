/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Replication;

namespace Db4objects.Db4o.Replication
{
	/// <summary>
	/// will be called by a
	/// <see cref="IReplicationProcess">IReplicationProcess</see>
	/// upon
	/// replication conflicts. Conflicts occur whenever
	/// <see cref="IReplicationProcess.Replicate">IReplicationProcess.Replicate</see>
	/// is called with an object that
	/// was modified in both ObjectContainers since the last replication run between
	/// the two.
	/// </summary>
	[System.ObsoleteAttribute(@"Since db4o-5.2. Use db4o Replication System (dRS) instead.<br><br>"
		)]
	public interface IReplicationConflictHandler
	{
		/// <summary>the callback method to be implemented to resolve a conflict.</summary>
		/// <remarks>
		/// the callback method to be implemented to resolve a conflict. &lt;br&gt;
		/// &lt;br&gt;
		/// </remarks>
		/// <param name="replicationProcess">
		/// the
		/// <see cref="IReplicationProcess">IReplicationProcess</see>
		/// for which this
		/// ReplicationConflictHandler is registered
		/// </param>
		/// <param name="a">the object modified in the peerA ObjectContainer</param>
		/// <param name="b">the object modified in the peerB ObjectContainer</param>
		/// <returns>
		/// the object (a or b) that should prevail in the conflict or null,
		/// if no action is to be taken. If this would violate the direction
		/// set with
		/// <see cref="IReplicationProcess.SetDirection">IReplicationProcess.SetDirection</see>
		/// no action will be taken.
		/// </returns>
		/// <seealso cref="IReplicationProcess.PeerA">IReplicationProcess.PeerA</seealso>
		/// <seealso cref="IReplicationProcess.PeerB">IReplicationProcess.PeerB</seealso>
		object ResolveConflict(IReplicationProcess replicationProcess, object a, object b
			);
	}
}
