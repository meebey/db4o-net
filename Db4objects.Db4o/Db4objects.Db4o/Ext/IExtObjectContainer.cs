/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Replication;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// extended functionality for the
	/// <see cref="IObjectContainer">IObjectContainer</see>
	/// interface.
	/// &lt;br&gt;&lt;br&gt;Every db4o
	/// <see cref="IObjectContainer">IObjectContainer</see>
	/// always is an &lt;code&gt;ExtObjectContainer&lt;/code&gt; so a cast is possible.&lt;br&gt;&lt;br&gt;
	/// <see cref="IObjectContainer.Ext">IObjectContainer.Ext</see>
	/// is a convenient method to perform the cast.&lt;br&gt;&lt;br&gt;
	/// The ObjectContainer functionality is split to two interfaces to allow newcomers to
	/// focus on the essential methods.
	/// </summary>
	public interface IExtObjectContainer : IObjectContainer
	{
		/// <summary>activates an object with the current activation strategy.</summary>
		/// <remarks>
		/// activates an object with the current activation strategy.
		/// In regular activation mode the object will be activated to the
		/// global activation depth, ( see
		/// <see cref="IConfiguration.ActivationDepth">IConfiguration.ActivationDepth</see>
		/// )
		/// and all configured settings for
		/// <see cref="IObjectClass.MaximumActivationDepth">IObjectClass.MaximumActivationDepth
		/// 	</see>
		/// 
		/// and
		/// <see cref="IObjectClass.MaximumActivationDepth">IObjectClass.MaximumActivationDepth
		/// 	</see>
		/// will be respected.&lt;br&gt;&lt;br&gt;
		/// In Transparent Activation Mode ( see
		/// <see cref="TransparentActivationSupport">TransparentActivationSupport</see>
		/// )
		/// the parameter object will only be activated, if it does not implement
		/// <see cref="IActivatable">IActivatable</see>
		/// . All referenced members that do not implement
		/// <see cref="IActivatable">IActivatable</see>
		/// will also be activated. Any
		/// <see cref="IActivatable">IActivatable</see>
		/// objects
		/// along the referenced graph will break cascading activation.
		/// </remarks>
		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="DatabaseClosedException"></exception>
		void Activate(object obj);

		/// <summary>deactivates an object.</summary>
		/// <remarks>
		/// deactivates an object.
		/// Only the passed object will be deactivated, i.e, no object referenced by this
		/// object will be deactivated.
		/// </remarks>
		/// <param name="obj">the object to be deactivated.</param>
		void Deactivate(object obj);

		/// <summary>backs up a database file of an open ObjectContainer.</summary>
		/// <remarks>
		/// backs up a database file of an open ObjectContainer.
		/// &lt;br&gt;&lt;br&gt;While the backup is running, the ObjectContainer can continue to be
		/// used. Changes that are made while the backup is in progress, will be applied to
		/// the open ObjectContainer and to the backup.&lt;br&gt;&lt;br&gt;
		/// While the backup is running, the ObjectContainer should not be closed.&lt;br&gt;&lt;br&gt;
		/// If a file already exists at the specified path, it will be overwritten.&lt;br&gt;&lt;br&gt;
		/// The backup call may be started in a seperated thread by the application,
		/// if concurrent execution with normal database access is desired.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="path">a fully qualified path</param>
		/// <exception cref="Db4oIOException">I/O operation failed or was unexpectedly interrupted.
		/// 	</exception>
		/// <exception cref="DatabaseClosedException">db4o database file was closed or failed to open.
		/// 	</exception>
		/// <exception cref="NotSupportedException">
		/// is thrown when the operation is not supported in current
		/// configuration/environment
		/// </exception>
		void Backup(string path);

		/// <summary>binds an object to an internal object ID.</summary>
		/// <remarks>
		/// binds an object to an internal object ID.
		/// &lt;br&gt;&lt;br&gt;This method uses the ID parameter to load the
		/// correspondig stored object into memory and replaces this memory
		/// reference with the object parameter. The method may be used to replace
		/// objects or to reassociate an object with it's stored instance
		/// after closing and opening a database file. A subsequent call to
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// is
		/// necessary to update the stored object.&lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;Requirements:&lt;/b&gt;&lt;br&gt;- The ID needs to be a valid internal object ID,
		/// previously retrieved with
		/// <see cref="IExtObjectContainer.GetID">IExtObjectContainer.GetID</see>
		/// .&lt;br&gt;
		/// - The object parameter needs to be of the same class as the stored object.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <seealso cref="IExtObjectContainer.GetID">IExtObjectContainer.GetID</seealso>
		/// <param name="obj">the object that is to be bound</param>
		/// <param name="id">the internal id the object is to be bound to</param>
		/// <exception cref="DatabaseClosedException">db4o database file was closed or failed to open.
		/// 	</exception>
		/// <exception cref="InvalidIDException">
		/// when the provided id is outside the scope of the
		/// database IDs.
		/// </exception>
		void Bind(object obj, long id);

		/// <summary>
		/// returns the
		/// <see cref="IDb4oCollections">IDb4oCollections</see>
		/// interface to create or modify database-aware
		/// collections for this
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <returns>
		/// the
		/// <see cref="IDb4oCollections">IDb4oCollections</see>
		/// interface for this
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// .
		/// </returns>
		[System.ObsoleteAttribute(@"since 7.0. Use of old internal collections is discouraged. Please use  com.db4o.collections.ArrayList4 and com.db4o.collections.ArrayMap4 instead."
			)]
		IDb4oCollections Collections();

		/// <summary>returns the Configuration context for this ObjectContainer.</summary>
		/// <remarks>
		/// returns the Configuration context for this ObjectContainer.
		/// &lt;br&gt;&lt;br&gt;
		/// Upon opening an ObjectContainer with any of the factory methods in the
		/// <see cref="Db4oFactory">Db4oFactory</see>
		/// class, the global
		/// <see cref="IConfiguration">IConfiguration</see>
		/// context
		/// is copied into the ObjectContainer. The
		/// <see cref="IConfiguration">IConfiguration</see>
		/// can be modified individually for
		/// each ObjectContainer without any effects on the global settings.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>
		/// 
		/// <see cref="IConfiguration">IConfiguration</see>
		/// the Configuration
		/// context for this ObjectContainer
		/// </returns>
		/// <seealso cref="Db4oFactory.Configure">Db4oFactory.Configure</seealso>
		IConfiguration Configure();

		/// <summary>returns a member at the specific path without activating intermediate objects.
		/// 	</summary>
		/// <remarks>
		/// returns a member at the specific path without activating intermediate objects.
		/// &lt;br&gt;&lt;br&gt;
		/// This method allows navigating from a persistent object to it's members in a
		/// performant way without activating or instantiating intermediate objects.
		/// </remarks>
		/// <param name="obj">the parent object that is to be used as the starting point.</param>
		/// <param name="path">an array of field names to navigate by</param>
		/// <returns>the object at the specified path or null if no object is found</returns>
		object Descend(object obj, string[] path);

		/// <summary>returns the stored object for an internal ID.</summary>
		/// <remarks>
		/// returns the stored object for an internal ID.
		/// &lt;br&gt;&lt;br&gt;This is the fastest method for direct access to objects. Internal
		/// IDs can be obtained with
		/// <see cref="IExtObjectContainer.GetID">IExtObjectContainer.GetID</see>
		/// .
		/// Objects will not be activated by this method. They will be returned in the
		/// activation state they are currently in, in the local cache.&lt;br&gt;&lt;br&gt;
		/// To activate the returned object with the current activation strategy, call
		/// <see cref="IExtObjectContainer.Activate">IExtObjectContainer.Activate</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="id">the internal ID</param>
		/// <returns>
		/// the object associated with the passed ID or &lt;code&gt;null&lt;/code&gt;,
		/// if no object is associated with this ID in this &lt;code&gt;ObjectContainer&lt;/code&gt;.
		/// </returns>
		/// <seealso cref="IConfiguration.ActivationDepth">Why activation?</seealso>
		/// <exception cref="DatabaseClosedException">db4o database file was closed or failed to open.
		/// 	</exception>
		/// <exception cref="InvalidIDException">when the provided id is not valid.</exception>
		object GetByID(long id);

		/// <summary>
		/// returns a stored object for a
		/// <see cref="Db4oUUID">Db4oUUID</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// This method is intended for replication and for long-term
		/// external references to objects. To get a
		/// <see cref="Db4oUUID">Db4oUUID</see>
		/// for an
		/// object use
		/// <see cref="IExtObjectContainer.GetObjectInfo">IExtObjectContainer.GetObjectInfo</see>
		/// and
		/// <see cref="IObjectInfo.GetUUID">IObjectInfo.GetUUID</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// Objects will not be activated by this method. They will be returned in the
		/// activation state they are currently in, in the local cache.&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <param name="uuid">the UUID</param>
		/// <returns>the object for the UUID</returns>
		/// <seealso cref="IConfiguration.ActivationDepth">Why activation?</seealso>
		/// <exception cref="Db4oIOException">I/O operation failed or was unexpectedly interrupted.
		/// 	</exception>
		/// <exception cref="DatabaseClosedException">db4o database file was closed or failed to open.
		/// 	</exception>
		object GetByUUID(Db4oUUID uuid);

		/// <summary>returns the internal unique object ID.</summary>
		/// <remarks>
		/// returns the internal unique object ID.
		/// &lt;br&gt;&lt;br&gt;db4o assigns an internal ID to every object that is stored. IDs are
		/// guaranteed to be unique within one &lt;code&gt;ObjectContainer&lt;/code&gt;.
		/// An object carries the same ID in every db4o session. Internal IDs can
		/// be used to look up objects with the very fast
		/// <see cref="IExtObjectContainer.GetByID">IExtObjectContainer.GetByID</see>
		/// method.&lt;br&gt;&lt;br&gt;
		/// Internal IDs will change when a database is defragmented. Use
		/// <see cref="IExtObjectContainer.GetObjectInfo">IExtObjectContainer.GetObjectInfo</see>
		/// ,
		/// <see cref="IObjectInfo.GetUUID">IObjectInfo.GetUUID</see>
		/// and
		/// <see cref="IExtObjectContainer.GetByUUID">IExtObjectContainer.GetByUUID</see>
		/// for long-term external references to
		/// objects.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="obj">any object</param>
		/// <returns>
		/// the associated internal ID or &lt;code&gt;0&lt;/code&gt;, if the passed
		/// object is not stored in this &lt;code&gt;ObjectContainer&lt;/code&gt;.
		/// </returns>
		long GetID(object obj);

		/// <summary>
		/// returns the
		/// <see cref="IObjectInfo">IObjectInfo</see>
		/// for a stored object.
		/// &lt;br&gt;&lt;br&gt;This method will return null, if the passed
		/// object is not stored to this &lt;code&gt;ObjectContainer&lt;/code&gt;.&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <param name="obj">the stored object</param>
		/// <returns>
		/// the
		/// <see cref="IObjectInfo">IObjectInfo</see>
		/// 
		/// </returns>
		IObjectInfo GetObjectInfo(object obj);

		/// <summary>
		/// returns the
		/// <see cref="Db4oDatabase">Db4oDatabase</see>
		/// identity object for this ObjectContainer.
		/// </summary>
		/// <returns>the Db4oDatabase identity object for this ObjectContainer.</returns>
		Db4oDatabase Identity();

		/// <summary>tests if an object is activated.</summary>
		/// <remarks>
		/// tests if an object is activated.
		/// &lt;br&gt;&lt;br&gt;&lt;code&gt;isActive&lt;/code&gt; returns &lt;code&gt;false&lt;/code&gt; if an object is not
		/// stored within the &lt;code&gt;ObjectContainer&lt;/code&gt;.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="obj">to be tested&lt;br&gt;&lt;br&gt;</param>
		/// <returns>&lt;code&gt;true&lt;/code&gt; if the passed object is active.</returns>
		bool IsActive(object obj);

		/// <summary>tests if an object with this ID is currently cached.</summary>
		/// <remarks>
		/// tests if an object with this ID is currently cached.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="Id">the internal ID</param>
		bool IsCached(long Id);

		/// <summary>tests if this &lt;code&gt;ObjectContainer&lt;/code&gt; is closed.</summary>
		/// <remarks>
		/// tests if this &lt;code&gt;ObjectContainer&lt;/code&gt; is closed.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>&lt;code&gt;true&lt;/code&gt; if this &lt;code&gt;ObjectContainer&lt;/code&gt; is closed.
		/// 	</returns>
		bool IsClosed();

		/// <summary>tests if an object is stored in this &lt;code&gt;ObjectContainer&lt;/code&gt;.
		/// 	</summary>
		/// <remarks>
		/// tests if an object is stored in this &lt;code&gt;ObjectContainer&lt;/code&gt;.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="obj">to be tested&lt;br&gt;&lt;br&gt;</param>
		/// <returns>&lt;code&gt;true&lt;/code&gt; if the passed object is stored.</returns>
		/// <exception cref="DatabaseClosedException">db4o database file was closed or failed to open.
		/// 	</exception>
		bool IsStored(object obj);

		/// <summary>
		/// returns all class representations that are known to this
		/// ObjectContainer because they have been used or stored.
		/// </summary>
		/// <remarks>
		/// returns all class representations that are known to this
		/// ObjectContainer because they have been used or stored.
		/// </remarks>
		/// <returns>
		/// all class representations that are known to this
		/// ObjectContainer because they have been used or stored.
		/// </returns>
		IReflectClass[] KnownClasses();

		/// <summary>returns the main synchronisation lock.</summary>
		/// <remarks>
		/// returns the main synchronisation lock.
		/// &lt;br&gt;&lt;br&gt;
		/// Synchronize over this object to ensure exclusive access to
		/// the ObjectContainer.&lt;br&gt;&lt;br&gt;
		/// Handle the use of this functionality with extreme care,
		/// since deadlocks can be produced with just two lines of code.
		/// </remarks>
		/// <returns>Object the ObjectContainer lock object</returns>
		[System.ObsoleteAttribute(@"Use is not recommended. Use your own monitor objects instead."
			)]
		object Lock();

		/// <summary>aids migration of objects between ObjectContainers.</summary>
		/// <remarks>
		/// aids migration of objects between ObjectContainers.
		/// &lt;br&gt;&lt;br&gt;When objects are migrated from one ObjectContainer to another, it is
		/// desirable to preserve virtual object attributes such as the object version number
		/// or the UUID. Use this method to signal to an ObjectContainer that it should read
		/// existing version numbers and UUIDs from another ObjectContainer. This method should
		/// also be used during the Defragment. It is included in the default
		/// implementation supplied in Defragment.java/Defragment.cs.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="objectContainer">
		/// the
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// objects are to be migrated
		/// from or &lt;code&gt;null&lt;/code&gt; to denote that migration is completed.
		/// </param>
		void MigrateFrom(IObjectContainer objectContainer);

		/// <summary>
		/// returns a transient copy of a persistent object with all members set
		/// to the values that are currently stored to the database.
		/// </summary>
		/// <remarks>
		/// returns a transient copy of a persistent object with all members set
		/// to the values that are currently stored to the database.
		/// &lt;br&gt;&lt;br&gt;
		/// The returned objects have no connection to the database.&lt;br&gt;&lt;br&gt;
		/// With the &lt;code&gt;committed&lt;/code&gt; parameter it is possible to specify,
		/// whether the desired object should contain the committed values or the
		/// values that were set by the running transaction with
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;A possible usecase for this feature:&lt;br&gt;
		/// An application might want to check all changes applied to an object
		/// by the running transaction.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="@object">the object that is to be cloned</param>
		/// <param name="depth">the member depth to which the object is to be instantiated</param>
		/// <param name="committed">whether committed or set values are to be returned</param>
		/// <returns>the object</returns>
		object PeekPersisted(object @object, int depth, bool committed);

		/// <summary>unloads all clean indices from memory and frees unused objects.</summary>
		/// <remarks>
		/// unloads all clean indices from memory and frees unused objects.
		/// &lt;br&gt;&lt;br&gt;Call commit() and purge() consecutively to achieve the best
		/// result possible. This method can have a negative impact
		/// on performance since indices will have to be reread before further
		/// inserts, updates or queries can take place.
		/// </remarks>
		void Purge();

		/// <summary>unloads a specific object from the db4o reference mechanism.</summary>
		/// <remarks>
		/// unloads a specific object from the db4o reference mechanism.
		/// &lt;br&gt;&lt;br&gt;db4o keeps references to all newly stored and
		/// instantiated objects in memory, to be able to manage object identities.
		/// &lt;br&gt;&lt;br&gt;With calls to this method it is possible to remove an object from the
		/// reference mechanism, to allow it to be garbage collected. You are not required to
		/// call this method in the .NET and JDK 1.2 versions, since objects are
		/// referred to by weak references and garbage collection happens
		/// automatically.&lt;br&gt;&lt;br&gt;An object removed with  &lt;code&gt;purge(Object)&lt;/code&gt; is not
		/// "known" to the &lt;code&gt;ObjectContainer&lt;/code&gt; afterwards, so this method may also be
		/// used to create multiple copies of  objects.&lt;br&gt;&lt;br&gt; &lt;code&gt;purge(Object)&lt;/code&gt; has
		/// no influence on the persistence state of objects. "Purged" objects can be
		/// reretrieved with queries.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="obj">the object to be removed from the reference mechanism.</param>
		void Purge(object obj);

		/// <summary>Return the reflector currently being used by db4objects.</summary>
		/// <remarks>Return the reflector currently being used by db4objects.</remarks>
		/// <returns>the current Reflector.</returns>
		GenericReflector Reflector();

		/// <summary>refreshs all members on a stored object to the specified depth.</summary>
		/// <remarks>
		/// refreshs all members on a stored object to the specified depth.
		/// &lt;br&gt;&lt;br&gt;If a member object is not activated, it will be activated by this method.
		/// &lt;br&gt;&lt;br&gt;The isolation used is READ COMMITTED. This method will read all objects
		/// and values that have been committed by other transactions.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="obj">the object to be refreshed.</param>
		/// <param name="depth">
		/// the member
		/// <see cref="IConfiguration.ActivationDepth">depth</see>
		/// to which refresh is to cascade.
		/// </param>
		void Refresh(object obj, int depth);

		/// <summary>releases a semaphore, if the calling transaction is the owner.</summary>
		/// <remarks>releases a semaphore, if the calling transaction is the owner.</remarks>
		/// <param name="name">the name of the semaphore to be released.</param>
		void ReleaseSemaphore(string name);

		/// <param name="peerB">
		/// the
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// to replicate with.
		/// </param>
		/// <param name="conflictHandler">
		/// the conflict handler for this ReplicationProcess.
		/// Conflicts occur
		/// whenever
		/// <see cref="IReplicationProcess.Replicate">IReplicationProcess.Replicate</see>
		/// is called with an
		/// object that was modified in both ObjectContainers since the last
		/// replication run between the two. Upon a conflict the
		/// <see cref="IReplicationConflictHandler.ResolveConflict">IReplicationConflictHandler.ResolveConflict
		/// 	</see>
		/// method will be called in the conflict handler.
		/// </param>
		/// <returns>
		/// the
		/// <see cref="IReplicationProcess">IReplicationProcess</see>
		/// interface for this replication process.
		/// </returns>
		[System.ObsoleteAttribute(@"Since db4o-5.2. Use db4o Replication System (dRS) instead.<br><br> prepares for replication with another"
			)]
		IReplicationProcess ReplicationBegin(IObjectContainer peerB, IReplicationConflictHandler
			 conflictHandler);

		/// <summary>deep update interface to store or update objects.</summary>
		/// <remarks>
		/// deep update interface to store or update objects.
		/// &lt;br&gt;&lt;br&gt;In addition to the normal storage interface,
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// ,
		/// this method allows a manual specification of the depth, the passed object is to be updated.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="obj">the object to be stored or updated.</param>
		/// <param name="depth">the depth to which the object is to be updated</param>
		/// <seealso cref="IObjectContainer.Store">IObjectContainer.Store</seealso>
		[System.ObsoleteAttribute(@"Use")]
		void Set(object obj, int depth);

		/// <summary>deep update interface to store or update objects.</summary>
		/// <remarks>
		/// deep update interface to store or update objects.
		/// &lt;br&gt;&lt;br&gt;In addition to the normal storage interface,
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// ,
		/// this method allows a manual specification of the depth, the passed object is to be updated.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="obj">the object to be stored or updated.</param>
		/// <param name="depth">the depth to which the object is to be updated</param>
		/// <seealso cref="IObjectContainer.Store">IObjectContainer.Store</seealso>
		void Store(object obj, int depth);

		/// <summary>attempts to set a semaphore.</summary>
		/// <remarks>
		/// attempts to set a semaphore.
		/// &lt;br&gt;&lt;br&gt;
		/// Semaphores are transient multi-purpose named flags for
		/// <see cref="IObjectContainer">ObjectContainers</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// A transaction that successfully sets a semaphore becomes
		/// the owner of the semaphore. Semaphores can only be owned
		/// by a single transaction at one point in time.&lt;br&gt;&lt;br&gt;
		/// This method returns true, if the transaction already owned
		/// the semaphore before the method call or if it successfully
		/// acquires ownership of the semaphore.&lt;br&gt;&lt;br&gt;
		/// The waitForAvailability parameter allows to specify a time
		/// in milliseconds to wait for other transactions to release
		/// the semaphore, in case the semaphore is already owned by
		/// another transaction.&lt;br&gt;&lt;br&gt;
		/// Semaphores are released by the first occurence of one of the
		/// following:&lt;br&gt;
		/// - the transaction releases the semaphore with
		/// <see cref="IExtObjectContainer.ReleaseSemaphore">IExtObjectContainer.ReleaseSemaphore
		/// 	</see>
		/// &lt;br&gt; - the transaction is closed with
		/// <see cref="IObjectContainer.Close">IObjectContainer.Close</see>
		/// &lt;br&gt; - C/S only: the corresponding
		/// <see cref="IObjectServer">IObjectServer</see>
		/// is
		/// closed.&lt;br&gt; - C/S only: the client
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// looses the connection and is timed
		/// out.&lt;br&gt;&lt;br&gt; Semaphores are set immediately. They are independant of calling
		/// <see cref="IObjectContainer.Commit">IObjectContainer.Commit</see>
		/// or
		/// <see cref="IObjectContainer.Rollback">IObjectContainer.Rollback</see>
		/// .&lt;br&gt;&lt;br&gt; &lt;b&gt;Possible usecases
		/// for semaphores:&lt;/b&gt;&lt;br&gt; - prevent other clients from inserting a singleton at the same time.
		/// A suggested name for the semaphore:  "SINGLETON_" + Object#getClass().getName().&lt;br&gt;  - lock
		/// objects. A suggested name:   "LOCK_" +
		/// <see cref="IExtObjectContainer.GetID">getID(Object)</see>
		/// &lt;br&gt; -
		/// generate a unique client ID. A suggested name:  "CLIENT_" +
		/// currentTime.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="name">the name of the semaphore to be set</param>
		/// <param name="waitForAvailability">
		/// the time in milliseconds to wait for other
		/// transactions to release the semaphore. The parameter may be zero, if
		/// the method is to return immediately.
		/// </param>
		/// <returns>
		/// boolean flag
		/// &lt;br&gt;&lt;code&gt;true&lt;/code&gt;, if the semaphore could be set or if the
		/// calling transaction already owned the semaphore.
		/// &lt;br&gt;&lt;code&gt;false&lt;/code&gt;, if the semaphore is owned by another
		/// transaction.
		/// </returns>
		bool SetSemaphore(string name, int waitForAvailability);

		/// <summary>
		/// returns a
		/// <see cref="IStoredClass">IStoredClass</see>
		/// meta information object.
		/// &lt;br&gt;&lt;br&gt;
		/// There are three options how to use this method.&lt;br&gt;
		/// Any of the following parameters are possible:&lt;br&gt;
		/// - a fully qualified classname.&lt;br&gt;
		/// - a Class object.&lt;br&gt;
		/// - any object to be used as a template.&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <param name="clazz">class name, Class object, or example object.&lt;br&gt;&lt;br&gt;
		/// 	</param>
		/// <returns>
		/// an instance of an
		/// <see cref="IStoredClass">IStoredClass</see>
		/// meta information object.
		/// </returns>
		IStoredClass StoredClass(object clazz);

		/// <summary>
		/// returns an array of all
		/// <see cref="IStoredClass">IStoredClass</see>
		/// meta information objects.
		/// </summary>
		IStoredClass[] StoredClasses();

		/// <summary>
		/// returns the
		/// <see cref="ISystemInfo">ISystemInfo</see>
		/// for this ObjectContainer.
		/// &lt;br&gt;&lt;br&gt;The
		/// <see cref="ISystemInfo">ISystemInfo</see>
		/// supplies methods that provide
		/// information about system state and system settings of this
		/// ObjectContainer.
		/// </summary>
		/// <returns>
		/// the
		/// <see cref="ISystemInfo">ISystemInfo</see>
		/// for this ObjectContainer.
		/// </returns>
		ISystemInfo SystemInfo();

		/// <summary>returns the current transaction serial number.</summary>
		/// <remarks>
		/// returns the current transaction serial number.
		/// &lt;br&gt;&lt;br&gt;This serial number can be used to query for modified objects
		/// and for replication purposes.
		/// </remarks>
		/// <returns>the current transaction serial number.</returns>
		long Version();
	}
}
