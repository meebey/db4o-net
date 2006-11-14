namespace Db4objects.Db4o.Replication
{
	/// <summary>
	/// will be called by a
	/// <see cref="Db4objects.Db4o.Replication.IReplicationProcess">Db4objects.Db4o.Replication.IReplicationProcess
	/// 	</see>
	/// upon
	/// replication conflicts. Conflicts occur whenever
	/// <see cref="Db4objects.Db4o.Replication.IReplicationProcess.Replicate">Db4objects.Db4o.Replication.IReplicationProcess.Replicate
	/// 	</see>
	/// is called with an object that
	/// was modified in both ObjectContainers since the last replication run between
	/// the two.
	/// </summary>
	public interface IReplicationConflictHandler
	{
		/// <summary>the callback method to be implemented to resolve a conflict.</summary>
		/// <remarks>
		/// the callback method to be implemented to resolve a conflict. <br />
		/// <br />
		/// </remarks>
		/// <param name="replicationProcess">
		/// the
		/// <see cref="Db4objects.Db4o.Replication.IReplicationProcess">Db4objects.Db4o.Replication.IReplicationProcess
		/// 	</see>
		/// for which this
		/// ReplicationConflictHandler is registered
		/// </param>
		/// <param name="a">the object modified in the peerA ObjectContainer</param>
		/// <param name="b">the object modified in the peerB ObjectContainer</param>
		/// <returns>
		/// the object (a or b) that should prevail in the conflict or null,
		/// if no action is to be taken. If this would violate the direction
		/// set with
		/// <see cref="Db4objects.Db4o.Replication.IReplicationProcess.SetDirection">Db4objects.Db4o.Replication.IReplicationProcess.SetDirection
		/// 	</see>
		/// no action will be taken.
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Replication.IReplicationProcess.PeerA">Db4objects.Db4o.Replication.IReplicationProcess.PeerA
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Replication.IReplicationProcess.PeerB">Db4objects.Db4o.Replication.IReplicationProcess.PeerB
		/// 	</seealso>
		object ResolveConflict(Db4objects.Db4o.Replication.IReplicationProcess replicationProcess
			, object a, object b);
	}
}
