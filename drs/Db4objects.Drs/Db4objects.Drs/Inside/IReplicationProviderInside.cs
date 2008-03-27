/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside
{
	public interface IReplicationProviderInside : Db4objects.Drs.IReplicationProvider
	{
		void Activate(object @object);

		/// <summary>Activates the fields, e.g.</summary>
		/// <remarks>
		/// Activates the fields, e.g. Collections, arrays, of an object
		/// &lt;p/&gt;
		/// /** Clear the  ReplicationReference cache
		/// </remarks>
		void ClearAllReferences();

		void CommitReplicationTransaction(long raisedDatabaseVersion);

		/// <summary>Destroys this provider and frees up resources.</summary>
		/// <remarks>Destroys this provider and frees up resources.</remarks>
		void Destroy();

		Db4objects.Db4o.IObjectSet GetStoredObjects(System.Type type);

		/// <summary>Returns the current transaction serial number.</summary>
		/// <remarks>Returns the current transaction serial number.</remarks>
		/// <returns>the current transaction serial number</returns>
		long GetCurrentVersion();

		long GetLastReplicationVersion();

		object GetMonitor();

		string GetName();

		Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature GetSignature();

		/// <summary>Returns the ReplicationReference of an object</summary>
		/// <param name="obj">object queried</param>
		/// <param name="referencingObj"></param>
		/// <param name="fieldName"></param>
		/// <returns>null if the object is not owned by this ReplicationProvider.</returns>
		Db4objects.Drs.Inside.IReplicationReference ProduceReference(object obj, object referencingObj
			, string fieldName);

		/// <summary>Returns the ReplicationReference of an object by specifying the uuid of the object.
		/// 	</summary>
		/// <remarks>Returns the ReplicationReference of an object by specifying the uuid of the object.
		/// 	</remarks>
		/// <param name="uuid">the uuid of the object</param>
		/// <param name="hint">the type of the object</param>
		/// <returns>the ReplicationReference or null if the reference cannot be found</returns>
		Db4objects.Drs.Inside.IReplicationReference ProduceReferenceByUUID(Db4objects.Db4o.Ext.Db4oUUID
			 uuid, System.Type hint);

		Db4objects.Drs.Inside.IReplicationReference ReferenceNewObject(object obj, Db4objects.Drs.Inside.IReplicationReference
			 counterpartReference, Db4objects.Drs.Inside.IReplicationReference referencingObjRef
			, string fieldName);

		/// <summary>Rollbacks all changes done during the replication session  and terminates the Transaction.
		/// 	</summary>
		/// <remarks>
		/// Rollbacks all changes done during the replication session  and terminates the Transaction.
		/// Guarantees the changes will not be applied to the underlying databases.
		/// </remarks>
		void RollbackReplication();

		/// <summary>Start a Replication Transaction with another ReplicationProvider</summary>
		/// <param name="peerSignature">the signature of another ReplicationProvider.</param>
		void StartReplicationTransaction(Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature
			 peerSignature);

		/// <summary>Stores the new replicated state of obj.</summary>
		/// <remarks>
		/// Stores the new replicated state of obj. It can also be a new object to this
		/// provider.
		/// </remarks>
		/// <param name="obj">Object with updated state or a clone of new object in the peer.
		/// 	</param>
		void StoreReplica(object obj);

		void SyncVersionWithPeer(long maxVersion);

		/// <summary>Visits the object of each cached ReplicationReference.</summary>
		/// <remarks>Visits the object of each cached ReplicationReference.</remarks>
		/// <param name="visitor">implements the visit functions, including copying of object states, and storing of changed objects
		/// 	</param>
		void VisitCachedReferences(Db4objects.Db4o.Foundation.IVisitor4 visitor);

		bool WasModifiedSinceLastReplication(Db4objects.Drs.Inside.IReplicationReference 
			reference);

		void ReplicateDeletion(Db4objects.Db4o.Ext.Db4oUUID uuid);
	}
}
