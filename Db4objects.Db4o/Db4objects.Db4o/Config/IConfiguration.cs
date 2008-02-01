/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Config
{
	/// <member name="ActivationDepth(int)">
	/// <doc>
	/// <summary>sets the activation depth to the specified value.</summary>
	/// <remarks>
	/// sets the activation depth to the specified value.
	/// <br/><br/><b>Why activation?</b><br/>
	/// When objects are instantiated from the database, the instantiation of member
	/// objects needs to be limited to a certain depth. Otherwise a single object
	/// could lead to loading the complete database into memory, if all objects where
	/// reachable from a single root object.<br/><br/>
	/// db4o uses the concept "depth", the number of field-to-field hops an object
	/// is away from another object. <b>
	/// The preconfigured "activation depth" db4o uses
	/// in the default setting is 5.
	/// </b>
	/// <br/><br/>Whenever an application iterates through the
	/// <see cref="IObjectSet">IObjectSet</see>
	/// of a query result, the result objects
	/// will be activated to the configured activation depth.<br/><br/>
	/// A concrete example with the preconfigured activation depth of 5:<br/>
	/// <pre>
	/// Object foo is the result of a query, it is delivered by the ObjectSet
	/// object foo = objectSet.Next();
	/// </pre>
	/// foo.member1.member2.member3.member4.member5 will be a valid object<br/>
	/// foo, member1, member2, member3 and member4 will be activated<br/>
	/// member5 will be deactivated, all of it's members will be null<br/>
	/// member5 can be activated at any time by calling
	/// <see cref="IObjectContainer.Activate">ObjectContainer#activate(member5, depth)</see>
	/// .
	/// <br/><br/>
	/// Note that raising the global activation depth will consume more memory and
	/// have negative effects on the performance of first-time retrievals. Lowering
	/// the global activation depth needs more individual activation work but can
	/// increase performance of queries.<br/><br/>
	/// <see cref="IObjectContainer.Deactivate">
	/// ObjectContainer#deactivate(Object, depth)
	/// </see>
	/// can be used to manually free memory by deactivating objects.<br/><br/>
	/// </remarks>
	/// <param name="depth">the desired global activation depth.</param>
	/// <seealso cref="IObjectClass.MaximumActivationDepth">
	/// configuring classes individually
	/// </seealso>
	/// 
	/// </doc>
	/// </member>
	/// <member name="AddAlias(IAlias)">
	/// <doc>
	/// <summary>adds a new Alias for a class, namespace or package.</summary>
	/// <remarks>
	/// adds a new Alias for a class, namespace or package.
	/// <br/><br/>Aliases can be used to persist classes in the running
	/// application to different persistent classes in a database file
	/// or on a db4o server.
	/// <br/><br/>Two simple Alias implementations are supplied along with
	/// db4o:<br/>
	/// -
	/// <see cref="TypeAlias">TypeAlias</see>
	/// provides an #equals() resolver to match
	/// names directly.<br/>
	/// -
	/// <see cref="WildcardAlias">WildcardAlias</see>
	/// allows simple pattern matching
	/// with one single '*' wildcard character.<br/>
	/// <br/>
	/// It is possible to create
	/// own complex
	/// <see cref="IAlias">IAlias</see>
	/// constructs by creating own resolvers
	/// that implement the
	/// <see cref="IAlias">IAlias</see>
	/// interface.
	/// <br/><br/>
	/// Four examples of concrete usecases:
	/// <br/><br/>
	/// <code>
	/// <b>// Creating an Alias for a single class</b><br/>
	/// Db4oFactory.Configure().AddAlias(<br/>
	///   new TypeAlias("Tutorial.F1.Pilot", "Tutorial.F1.Driver"));<br/>
	/// <br/><br/>
	/// <b>// Accessing a Java package from a .NET assembly</b><br/>
	/// Db4o.configure().addAlias(<br/>
	///   new WildcardAlias(<br/>
	///     "com.f1.*",<br/>
	///     "Tutorial.F1.*, Tutorial"));<br/>
	/// <br/><br/>
	/// <b>// Using a different local .NET assembly</b><br/>
	/// Db4o.configure().addAlias(<br/>
	///   new WildcardAlias(<br/>
	///     "Tutorial.F1.*, Tutorial",<br/>
	///     "Tutorial.F1.*, RaceClient"));<br/>
	/// </code>
	/// <br/><br/>Aliases that translate the persistent name of a class to
	/// a name that already exists as a persistent name in the database
	/// (or on the server) are not permitted and will throw an exception
	/// when the database file is opened.
	/// <br/><br/>Aliases should be configured before opening a database file
	/// or connecting to a server.
	/// </remarks>
	/// 
	/// </doc>
	/// </member>
	/// 
	/// <member name="AutomaticShutDown(boo)">
	/// <doc>
	/// <summary>turns automatic shutdown of the engine on and off.</summary>
	/// <remarks>
	/// turns automatic shutdown of the engine on and off.
	/// </remarks>
	/// <param name="flag">whether db4o should shut down automatically.</param>
	/// </doc>
	/// </member>
	/// 
	/// <member name="LockDatabaseFile(bool)">
	/// <doc>
	/// <summary>can be used to turn the database file locking thread off.</summary>
	/// <param name="flag">
	/// <code>false</code> to turn database file locking off.
	/// </param>
	/// 
	/// </doc>
	/// </member>
	/// 
	/// <member name="ReflectWith(IReflector)">
	/// <doc>
	/// <summary>configures the use of a specially designed reflection implementation.</summary>
	/// <remarks>
	/// configures the use of a specially designed reflection implementation.
	/// <br/><br/>
	/// db4o internally uses System.Reflection by default. On platforms that
	/// do not support this package, customized implementations may be written
	/// to supply all the functionality of the interfaces in System.Reflection
	/// namespace. This method can be used to install a custom reflection
	/// implementation.
	/// 
	/// </remarks>
	/// 
	/// </doc>
	/// </member>
	/// 
	/// <member name="WeakReferenceCollectionInterval(int)">
	/// <doc>
	/// <summary>configures the timer for WeakReference collection.</summary>
	/// <remarks>
	/// configures the timer for WeakReference collection.
	/// <br/><br/>The default setting is 1000 milliseconds.
	/// <br/><br/>Configure this setting to zero to turn WeakReference
	/// collection off.
	/// 
	/// </remarks>
	/// <param name="milliseconds">the time in milliseconds</param>
	/// </doc>
	/// </member>
	/// 
	/// <member name="WeakReferences(bool)">
	/// <doc>
	/// <summary>turns weak reference management on or off.</summary>
	/// <remarks>
	/// turns weak reference management on or off.
	/// <br/><br/>
	/// This method must be called before opening a database.
	/// <br/><br/>
	/// Performance may be improved by running db4o without using weak
	/// references durring memory management at the cost of higher
	/// memory consumption or by alternatively implementing a manual
	/// memory management scheme using
	/// <see cref="IExtObjectContainer.Purge">IExtObjectContainer.Purge</see>
	/// <br/><br/>Setting the value to <code>false</code> causes db4o to use hard
	/// references to objects, preventing the garbage collection process
	/// from disposing of unused objects.
	/// <br/><br/>The default setting is <code>true</code>.
	/// </remarks>
	/// </doc>
	/// </member>
	public interface IConfiguration
	{
		/// <summary>sets the activation depth to the specified value.</summary>
		/// <remarks>
		/// sets the activation depth to the specified value.
		/// &lt;br&gt;&lt;br&gt;&lt;b&gt;Why activation?&lt;/b&gt;&lt;br&gt;
		/// When objects are instantiated from the database, the instantiation of member
		/// objects needs to be limited to a certain depth. Otherwise a single object
		/// could lead to loading the complete database into memory, if all objects where
		/// reachable from a single root object.&lt;br&gt;&lt;br&gt;
		/// db4o uses the concept "depth", the number of field-to-field hops an object
		/// is away from another object. &lt;b&gt;The preconfigured "activation depth" db4o uses
		/// in the default setting is 5.&lt;/b&gt;
		/// &lt;br&gt;&lt;br&gt;Whenever an application iterates through the
		/// <see cref="IObjectSet">IObjectSet</see>
		/// of a query result, the result objects
		/// will be activated to the configured activation depth.&lt;br&gt;&lt;br&gt;
		/// A concrete example with the preconfigured activation depth of 5:&lt;br&gt;
		/// &lt;pre&gt;
		/// // Object foo is the result of a query, it is delivered by the ObjectSet
		/// Object foo = objectSet.next();&lt;/pre&gt;
		/// foo.member1.member2.member3.member4.member5 will be a valid object&lt;br&gt;
		/// foo, member1, member2, member3 and member4 will be activated&lt;br&gt;
		/// member5 will be deactivated, all of it's members will be null&lt;br&gt;
		/// member5 can be activated at any time by calling
		/// <see cref="IObjectContainer.Activate">ObjectContainer#activate(member5, depth)</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// Note that raising the global activation depth will consume more memory and
		/// have negative effects on the performance of first-time retrievals. Lowering
		/// the global activation depth needs more individual activation work but can
		/// increase performance of queries.&lt;br&gt;&lt;br&gt;
		/// <see cref="IObjectContainer.Deactivate">ObjectContainer#deactivate(Object, depth)
		/// 	</see>
		/// can be used to manually free memory by deactivating objects.&lt;br&gt;&lt;br&gt;
		/// In client/server environment the same setting should be used on both
		/// client and server&lt;br&gt;&lt;br&gt;.
		/// </remarks>
		/// <param name="depth">the desired global activation depth.</param>
		/// <seealso cref="IObjectClass.MaximumActivationDepth">configuring classes individually
		/// 	</seealso>
		void ActivationDepth(int depth);

		/// <summary>gets the configured activation depth.</summary>
		/// <remarks>gets the configured activation depth.</remarks>
		/// <returns>the configured activation depth.</returns>
		int ActivationDepth();

		/// <summary>
		/// adds ConfigurationItems to be applied when
		/// an ObjectContainer or ObjectServer is opened.
		/// </summary>
		/// <remarks>
		/// adds ConfigurationItems to be applied when
		/// an ObjectContainer or ObjectServer is opened.
		/// </remarks>
		/// <param name="configurationItem">the ConfigurationItem</param>
		void Add(IConfigurationItem configurationItem);

		/// <summary>adds a new Alias for a class, namespace or package.</summary>
		/// <remarks>
		/// adds a new Alias for a class, namespace or package.
		/// &lt;br&gt;&lt;br&gt;Aliases can be used to persist classes in the running
		/// application to different persistent classes in a database file
		/// or on a db4o server.
		/// &lt;br&gt;&lt;br&gt;Two simple Alias implementations are supplied along with
		/// db4o:&lt;br&gt;
		/// -
		/// <see cref="TypeAlias">TypeAlias</see>
		/// provides an #equals() resolver to match
		/// names directly.&lt;br&gt;
		/// -
		/// <see cref="WildcardAlias">WildcardAlias</see>
		/// allows simple pattern matching
		/// with one single '*' wildcard character.&lt;br&gt;
		/// &lt;br&gt;
		/// It is possible to create
		/// own complex
		/// <see cref="IAlias">IAlias</see>
		/// constructs by creating own resolvers
		/// that implement the
		/// <see cref="IAlias">IAlias</see>
		/// interface.
		/// &lt;br&gt;&lt;br&gt;
		/// Examples of concrete usecases:
		/// &lt;br&gt;&lt;br&gt;
		/// &lt;code&gt;
		/// &lt;b&gt;// Creating an Alias for a single class&lt;/b&gt;&lt;br&gt;
		/// Db4o.configure().addAlias(&lt;br&gt;
		/// &#160;&#160;new TypeAlias("com.f1.Pilot", "com.f1.Driver"));&lt;br&gt;
		/// &lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;// Accessing a .NET assembly from a Java package&lt;/b&gt;&lt;br&gt;
		/// Db4o.configure().addAlias(&lt;br&gt;
		/// &#160;&#160;new WildcardAlias(&lt;br&gt;
		/// &#160;&#160;&#160;&#160;"Tutorial.F1.*, Tutorial",&lt;br&gt;
		/// &#160;&#160;&#160;&#160;"com.f1.*"));&lt;br&gt;
		/// &lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;// Mapping a Java package onto another&lt;/b&gt;&lt;br&gt;
		/// Db4o.configure().addAlias(&lt;br&gt;
		/// &#160;&#160;new WildcardAlias(&lt;br&gt;
		/// &#160;&#160;&#160;&#160;"com.f1.*",&lt;br&gt;
		/// &#160;&#160;&#160;&#160;"com.f1.client*"));&lt;br&gt;&lt;/code&gt;
		/// &lt;br&gt;&lt;br&gt;Aliases that translate the persistent name of a class to
		/// a name that already exists as a persistent name in the database
		/// (or on the server) are not permitted and will throw an exception
		/// when the database file is opened.
		/// &lt;br&gt;&lt;br&gt;Aliases should be configured before opening a database file
		/// or connecting to a server.&lt;br&gt;&lt;br&gt;
		/// In client/server environment this setting should be used on the server side.
		/// </remarks>
		void AddAlias(IAlias alias);

		/// <summary>
		/// Removes an alias previously added with
		/// <see cref="IConfiguration.AddAlias">IConfiguration.AddAlias</see>
		/// .
		/// </summary>
		/// <param name="alias">the alias to remove</param>
		void RemoveAlias(IAlias alias);

		/// <summary>turns automatic database file format version updates on.</summary>
		/// <remarks>
		/// turns automatic database file format version updates on.
		/// &lt;br&gt;&lt;br&gt;Upon db4o database file format version changes,
		/// db4o can automatically update database files to the
		/// current version. db4objects does not provide functionality
		/// to reverse this process. It is not ensured that updated
		/// database files can be read with older db4o versions.
		/// In some cases (Example: using ObjectManager) it may not be
		/// desirable to update database files automatically therefore
		/// automatic updating is turned off by default for
		/// security reasons.
		/// &lt;br&gt;&lt;br&gt;Call this method to turn automatic database file
		/// version updating on.
		/// &lt;br&gt;&lt;br&gt;If automatic updating is turned off, db4o will refuse
		/// to open database files that use an older database file format.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both client
		/// and server.
		/// </remarks>
		void AllowVersionUpdates(bool flag);

		/// <summary>turns automatic shutdown of the engine on and off.</summary>
		/// <remarks>
		/// turns automatic shutdown of the engine on and off.
		/// &lt;br&gt;&lt;br&gt;Depending on the JDK, db4o uses one of the following
		/// two methods to shut down, if no more references to the ObjectContainer
		/// are being held or the JVM terminates:&lt;br&gt;
		/// - JDK 1.3 and above: &lt;code&gt;Runtime.addShutdownHook()&lt;/code&gt;&lt;br&gt;
		/// - JDK 1.2 and below: &lt;code&gt;System.runFinalizersOnExit(true)&lt;/code&gt; and code
		/// in the finalizer.&lt;br&gt;&lt;br&gt;
		/// Some JVMs have severe problems with both methods. For these rare cases the
		/// autoShutDown feature may be turned off.&lt;br&gt;&lt;br&gt;
		/// The default and recommended setting is &lt;code&gt;true&lt;/code&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both client
		/// and server.
		/// </remarks>
		/// <param name="flag">whether db4o should shut down automatically.</param>
		void AutomaticShutDown(bool flag);

		/// <summary>sets the storage data blocksize for new ObjectContainers.</summary>
		/// <remarks>
		/// sets the storage data blocksize for new ObjectContainers.
		/// &lt;br&gt;&lt;br&gt;The standard setting is 1 allowing for a maximum
		/// database file size of 2GB. This value can be increased
		/// to allow larger database files, although some space will
		/// be lost to padding because the size of some stored objects
		/// will not be an exact multiple of the block size. A
		/// recommended setting for large database files is 8, since
		/// internal pointers have this length.&lt;br&gt;&lt;br&gt;
		/// This setting is only effective when the database is first created, in
		/// client-server environment in most cases it means that the setting
		/// should be used on the server side.
		/// </remarks>
		/// <param name="bytes">the size in bytes from 1 to 127</param>
		/// <exception cref="GlobalOnlyConfigException"></exception>
		void BlockSize(int bytes);

		/// <summary>configures the size of BTree nodes in indexes.</summary>
		/// <remarks>
		/// configures the size of BTree nodes in indexes.
		/// &lt;br&gt;&lt;br&gt;Default setting: 100
		/// &lt;br&gt;Lower values will allow a lower memory footprint
		/// and more efficient reading and writing of small slots.
		/// &lt;br&gt;Higher values will reduce the overall number of
		/// read and write operations and allow better performance
		/// at the cost of more RAM use.&lt;br&gt;&lt;br&gt;
		/// This setting should be used on both client and server in
		/// client-server environment.
		/// </remarks>
		/// <param name="size">the number of elements held in one BTree node.</param>
		void BTreeNodeSize(int size);

		/// <summary>configures caching of BTree nodes.</summary>
		/// <remarks>
		/// configures caching of BTree nodes.
		/// &lt;br&gt;&lt;br&gt;Clean BTree nodes will be unloaded on #commit and
		/// #rollback unless they are configured as cached here.
		/// &lt;br&gt;&lt;br&gt;Default setting: 0
		/// &lt;br&gt;Possible settings: 1, 2 or 3
		/// &lt;br&gt;&lt;br&gt; The potential number of cached BTree nodes can be
		/// calculated with the following forumula:&lt;br&gt;
		/// maxCachedNodes = bTreeNodeSize ^ bTreeCacheHeight&lt;br&gt;&lt;br&gt;
		/// This setting should be used on both client and server in
		/// client-server environment.
		/// </remarks>
		/// <param name="height">the height of the cache from the root</param>
		void BTreeCacheHeight(int height);

		/// <summary>turns callback methods on and off.</summary>
		/// <remarks>
		/// turns callback methods on and off.
		/// &lt;br&gt;&lt;br&gt;Callbacks are turned on by default.&lt;br&gt;&lt;br&gt;
		/// A tuning hint: If callbacks are not used, you can turn this feature off, to
		/// prevent db4o from looking for callback methods in persistent classes. This will
		/// increase the performance on system startup.&lt;br&gt;&lt;br&gt;
		/// In client/server environment this setting should be used on both
		/// client and server.
		/// </remarks>
		/// <param name="flag">false to turn callback methods off</param>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void Callbacks(bool flag);

		/// <summary>
		/// advises db4o to try instantiating objects with/without calling
		/// constructors.
		/// </summary>
		/// <remarks>
		/// advises db4o to try instantiating objects with/without calling
		/// constructors.
		/// &lt;br&gt;&lt;br&gt;
		/// Not all JDKs / .NET-environments support this feature. db4o will
		/// attempt, to follow the setting as good as the enviroment supports.
		/// In doing so, it may call implementation-specific features like
		/// sun.reflect.ReflectionFactory#newConstructorForSerialization on the
		/// Sun Java 1.4.x/5 VM (not available on other VMs) and
		/// FormatterServices.GetUninitializedObject() on
		/// the .NET framework (not available on CompactFramework).
		/// This setting may also be overridden for individual classes in
		/// <see cref="IObjectClass.CallConstructor">IObjectClass.CallConstructor</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;The default setting depends on the features supported by your current environment.&lt;br&gt;&lt;br&gt;
		/// In client/server environment this setting should be used on both
		/// client and server.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">
		/// - specify true, to request calling constructors, specify
		/// false to request &lt;b&gt;not&lt;/b&gt; calling constructors.
		/// </param>
		/// <seealso cref="IObjectClass.CallConstructor">IObjectClass.CallConstructor</seealso>
		void CallConstructors(bool flag);

		/// <summary>
		/// turns
		/// <see cref="IObjectClass.MaximumActivationDepth">individual class activation depth configuration
		/// 	</see>
		/// on
		/// and off.
		/// &lt;br&gt;&lt;br&gt;This feature is turned on by default.&lt;br&gt;&lt;br&gt;
		/// In client/server environment this setting should be used on both
		/// client and server.&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <param name="flag">
		/// false to turn the possibility to individually configure class
		/// activation depths off
		/// </param>
		/// <seealso cref="IConfiguration.ActivationDepth">Why activation?</seealso>
		void ClassActivationDepthConfigurable(bool flag);

		/// <summary>returns client/server configuration interface.</summary>
		/// <remarks>returns client/server configuration interface.</remarks>
		IClientServerConfiguration ClientServer();

		/// <summary>
		/// configures the size database files should grow in bytes, when no
		/// free slot is found within.
		/// </summary>
		/// <remarks>
		/// configures the size database files should grow in bytes, when no
		/// free slot is found within.
		/// &lt;br&gt;&lt;br&gt;Tuning setting.
		/// &lt;br&gt;&lt;br&gt;Whenever no free slot of sufficient length can be found
		/// within the current database file, the database file's length
		/// is extended. This configuration setting configures by how much
		/// it should be extended, in bytes.&lt;br&gt;&lt;br&gt;
		/// This configuration setting is intended to reduce fragmentation.
		/// Higher values will produce bigger database files and less
		/// fragmentation.&lt;br&gt;&lt;br&gt;
		/// To extend the database file, a single byte array is created
		/// and written to the end of the file in one write operation. Be
		/// aware that a high setting will require allocating memory for
		/// this byte array.
		/// </remarks>
		/// <param name="bytes">amount of bytes</param>
		void DatabaseGrowthSize(int bytes);

		/// <summary>
		/// tuning feature: configures whether db4o checks all persistent classes upon system
		/// startup, for added or removed fields.
		/// </summary>
		/// <remarks>
		/// tuning feature: configures whether db4o checks all persistent classes upon system
		/// startup, for added or removed fields.
		/// &lt;br&gt;&lt;br&gt;If this configuration setting is set to false while a database is
		/// being created, members of classes will not be detected and stored.
		/// &lt;br&gt;&lt;br&gt;This setting can be set to false in a production environment after
		/// all persistent classes have been stored at least once and classes will not
		/// be modified any further in the future.&lt;br&gt;&lt;br&gt;
		/// In a client/server environment this setting should be configured both on the
		/// client and and on the server.
		/// &lt;br&gt;&lt;br&gt;Default value:&lt;br&gt;
		/// &lt;code&gt;true&lt;/code&gt;
		/// </remarks>
		/// <param name="flag">the desired setting</param>
		void DetectSchemaChanges(bool flag);

		/// <summary>returns the configuration interface for diagnostics.</summary>
		/// <remarks>returns the configuration interface for diagnostics.</remarks>
		/// <returns>the configuration interface for diagnostics.</returns>
		IDiagnosticConfiguration Diagnostic();

		/// <summary>turns commit recovery off.</summary>
		/// <remarks>
		/// turns commit recovery off.
		/// &lt;br&gt;&lt;br&gt;db4o uses a two-phase commit algorithm. In a first step all intended
		/// changes are written to a free place in the database file, the "transaction commit
		/// record". In a second step the
		/// actual changes are performed. If the system breaks down during commit, the
		/// commit process is restarted when the database file is opened the next time.
		/// On very rare occasions (possibilities: hardware failure or editing the database
		/// file with an external tool) the transaction commit record may be broken. In this
		/// case, this method can be used to try to open the database file without commit
		/// recovery. The method should only be used in emergency situations after consulting
		/// db4o support.
		/// </remarks>
		void DisableCommitRecovery();

		/// <summary>
		/// tuning feature: configures the minimum size of free space slots in the database file
		/// that are to be reused.
		/// </summary>
		/// <remarks>
		/// tuning feature: configures the minimum size of free space slots in the database file
		/// that are to be reused.
		/// &lt;br&gt;&lt;br&gt;When objects are updated or deleted, the space previously occupied in the
		/// database file is marked as "free", so it can be reused. db4o maintains two lists
		/// in RAM, sorted by address and by size. Adjacent entries are merged. After a large
		/// number of updates or deletes have been executed, the lists can become large, causing
		/// RAM consumption and performance loss for maintenance. With this method you can
		/// specify an upper bound for the byte slot size to discard.
		/// &lt;br&gt;&lt;br&gt;Pass &lt;code&gt;Integer.MAX_VALUE&lt;/code&gt; to this method to discard all free slots for
		/// the best possible startup time.&lt;br&gt;&lt;br&gt;
		/// The downside of setting this value: Database files will necessarily grow faster.
		/// &lt;br&gt;&lt;br&gt;Default value:&lt;br&gt;
		/// &lt;code&gt;0&lt;/code&gt; all space is reused
		/// </remarks>
		/// <param name="byteCount">Slots with this size or smaller will be lost.</param>
		[System.ObsoleteAttribute(@"please call Db4o.configure().freespace().discardSmallerThan()"
			)]
		void DiscardFreeSpace(int byteCount);

		/// <summary>configures the use of encryption.</summary>
		/// <remarks>
		/// configures the use of encryption.
		/// &lt;br&gt;&lt;br&gt;This method needs to be called &lt;b&gt;before&lt;/b&gt; a database file
		/// is created with the first
		/// <see cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;If encryption is set to true,
		/// you need to supply a password to seed the encryption mechanism.&lt;br&gt;&lt;br&gt;
		/// db4o database files keep their encryption format after creation.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">
		/// true for turning encryption on, false for turning encryption
		/// off.
		/// </param>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		/// <exception cref="GlobalOnlyConfigException"></exception>
		[System.ObsoleteAttribute(@"use a custom encrypting")]
		void Encrypt(bool flag);

		/// <summary>configures whether Exceptions are to be thrown, if objects can not be stored.
		/// 	</summary>
		/// <remarks>
		/// configures whether Exceptions are to be thrown, if objects can not be stored.
		/// &lt;br&gt;&lt;br&gt;db4o requires the presence of a constructor that can be used to
		/// instantiate objects. If no default public constructor is present, all
		/// available constructors are tested, whether an instance of the class can
		/// be instantiated. Null is passed to all constructor parameters.
		/// The first constructor that is successfully tested will
		/// be used throughout the running db4o session. If an instance of the class
		/// can not be instantiated, the object will not be stored. By default,
		/// execution will continue without any message or error. This method can
		/// be used to configure db4o to throw an
		/// <see cref="ObjectNotStorableException">ObjectNotStorableException</see>
		/// if an object can not be stored.
		/// &lt;br&gt;&lt;br&gt;
		/// The default for this setting is &lt;b&gt;false&lt;/b&gt;.&lt;br&gt;&lt;br&gt;
		/// In client/server environment this setting should be used on both
		/// client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">true to throw Exceptions if objects can not be stored.</param>
		void ExceptionsOnNotStorable(bool flag);

		/// <summary>configures file buffers to be flushed during transaction commits.</summary>
		/// <remarks>
		/// configures file buffers to be flushed during transaction commits.
		/// &lt;br&gt;&lt;br&gt;
		/// db4o uses a resume-commit-on-crash strategy to ensure ACID transactions.
		/// When a transaction commits,&lt;br&gt;
		/// - (1) a list "pointers that are to be modified" is written to the database file,&lt;br&gt;
		/// - (2) the database file is switched into "in-commit" mode, &lt;br&gt;
		/// - (3) the pointers are actually modified in the database file,&lt;br&gt;
		/// - (4) the database file is switched to "not-in-commit" mode.&lt;br&gt;
		/// If the system is halted by a hardware or power failure &lt;br&gt;
		/// - before (2)&lt;br&gt;
		/// all objects will be available as before the commit&lt;br&gt;
		/// - between (2) and (4)
		/// the commit is restarted when the database file is opened the next time, all pointers
		/// will be read from the "pointers to be modified" list and all of them will be modified
		/// to the state they are intended to have after commit&lt;br&gt;
		/// - after (4)
		/// no work is necessary, the transaction is committed.
		/// &lt;br&gt;&lt;br&gt;
		/// In order for the above to be 100% failsafe, the order of writes to the
		/// storage medium has to be obeyed. On operating systems that use in-memory
		/// file caching, the OS cache may revert the order of writes to optimize
		/// file performance.&lt;br&gt;&lt;br&gt;
		/// db4o enforces the correct write order by flushing file
		/// buffers after every single one of the above steps during transaction
		/// commit. Flush calls have a strong impact on performance. It is possible to
		/// tune an application by turning flushing off, at the risc of less security
		/// for hardware-, power- or operating system failures.&lt;br&gt;&lt;br&gt;
		/// In client/server environment this setting should be used on both
		/// client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">true for flushing file buffers</param>
		void FlushFileBuffers(bool flag);

		/// <summary>returns the freespace configuration interface.</summary>
		/// <remarks>returns the freespace configuration interface.</remarks>
		IFreespaceConfiguration Freespace();

		/// <summary>configures db4o to generate UUIDs for stored objects.</summary>
		/// <remarks>configures db4o to generate UUIDs for stored objects.</remarks>
		/// <param name="setting">
		/// one of the following values:&lt;br&gt;
		/// -1 - off&lt;br&gt;
		/// 1 - configure classes individually&lt;br&gt;
		/// Integer.MAX_Value - on for all classes&lt;br&gt;&lt;br&gt;
		/// This setting should be used when the database is first created.
		/// </param>
		[System.ObsoleteAttribute(@"Use")]
		void GenerateUUIDs(int setting);

		/// <summary>configures db4o to generate UUIDs for stored objects.</summary>
		/// <remarks>
		/// configures db4o to generate UUIDs for stored objects.
		/// This setting should be used when the database is first created.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="setting">the scope for UUID generation: disabled, generate for all classes, or configure individually
		/// 	</param>
		void GenerateUUIDs(ConfigScope setting);

		/// <summary>configures db4o to generate version numbers for stored objects.</summary>
		/// <remarks>configures db4o to generate version numbers for stored objects.</remarks>
		/// <param name="setting">
		/// one of the following values:&lt;br&gt;
		/// -1 - off&lt;br&gt;
		/// 1 - configure classes individually&lt;br&gt;
		/// Integer.MAX_Value - on for all classes&lt;br&gt;&lt;br&gt;
		/// This setting should be used when the database is first created.
		/// </param>
		[System.ObsoleteAttribute(@"Use")]
		void GenerateVersionNumbers(int setting);

		/// <summary>configures db4o to generate version numbers for stored objects.</summary>
		/// <remarks>
		/// configures db4o to generate version numbers for stored objects.
		/// This setting should be used when the database is first created.
		/// </remarks>
		/// <param name="setting">the scope for version number generation: disabled, generate for all classes, or configure individually
		/// 	</param>
		void GenerateVersionNumbers(ConfigScope setting);

		/// <summary>configures db4o to call #intern() on strings upon retrieval.</summary>
		/// <remarks>
		/// configures db4o to call #intern() on strings upon retrieval.
		/// In client/server environment the setting should be used on both
		/// client and server.
		/// </remarks>
		/// <param name="flag">true to intern strings</param>
		void InternStrings(bool flag);

		/// <summary>returns true if strings will be interned.</summary>
		/// <remarks>returns true if strings will be interned.</remarks>
		bool InternStrings();

		/// <summary>allows to configure db4o to use a customized byte IO adapter.</summary>
		/// <remarks>
		/// allows to configure db4o to use a customized byte IO adapter.
		/// &lt;br&gt;&lt;br&gt;Derive from the abstract class
		/// <see cref="IoAdapter">IoAdapter</see>
		/// to
		/// write your own. Possible usecases could be improved performance
		/// with a native library, mirrored write to two files, encryption or
		/// read-on-write fail-safety control.&lt;br&gt;&lt;br&gt;An example of a custom
		/// io adapter can be found in xtea_db4o community project:&lt;br&gt;
		/// http://developer.db4o.com/ProjectSpaces/view.aspx/XTEA&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on the server
		/// (adapter class must be available)&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="adapter">- the IoAdapter</param>
		/// <exception cref="GlobalOnlyConfigException"></exception>
		void Io(IoAdapter adapter);

		/// <summary>allows to mark fields as transient with custom attributes.</summary>
		/// <remarks>
		/// allows to mark fields as transient with custom attributes.
		/// &lt;br&gt;&lt;br&gt;.NET only: Call this method with the attribute name that you
		/// wish to use to mark fields as transient. Multiple transient attributes
		/// are possible by calling this method multiple times with different
		/// attribute names.&lt;br&gt;&lt;br&gt;
		/// In client/server environment the setting should be used on both
		/// client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="attributeName">
		/// - the fully qualified name of the attribute, including
		/// it's namespace
		/// </param>
		void MarkTransient(string attributeName);

		/// <summary>sets the detail level of db4o messages.</summary>
		/// <remarks>
		/// sets the detail level of db4o messages. Messages will be output to the
		/// configured output
		/// <see cref="TextWriter">TextWriter</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// Level 0 - no messages&lt;br&gt;
		/// Level 1 - open and close messages&lt;br&gt;
		/// Level 2 - messages for new, update and delete&lt;br&gt;
		/// Level 3 - messages for activate and deactivate&lt;br&gt;&lt;br&gt;
		/// When using client-server and the level is set to 0, the server will override this and set it to 1.  To get around this you can set the level to -1.  This has the effect of not returning any messages.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting can be used on client or on server
		/// depending on which information do you want to track (server side provides more
		/// detailed information).&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="level">integer from 0 to 3</param>
		/// <seealso cref="IConfiguration.SetOut">IConfiguration.SetOut</seealso>
		void MessageLevel(int level);

		/// <summary>can be used to turn the database file locking thread off.</summary>
		/// <remarks>
		/// can be used to turn the database file locking thread off.
		/// &lt;br&gt;&lt;br&gt;Since Java does not support file locking up to JDK 1.4,
		/// db4o uses an additional thread per open database file to prohibit
		/// concurrent access to the same database file by different db4o
		/// sessions in different VMs.&lt;br&gt;&lt;br&gt;
		/// To improve performance and to lower ressource consumption, this
		/// method provides the possibility to prevent the locking thread
		/// from being started.&lt;br&gt;&lt;br&gt;&lt;b&gt;Caution!&lt;/b&gt;&lt;br&gt;If database file
		/// locking is turned off, concurrent write access to the same
		/// database file from different JVM sessions will &lt;b&gt;corrupt&lt;/b&gt; the
		/// database file immediately.&lt;br&gt;&lt;br&gt; This method
		/// has no effect on open ObjectContainers. It will only affect how
		/// ObjectContainers are opened.&lt;br&gt;&lt;br&gt;
		/// The default setting is &lt;code&gt;true&lt;/code&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">&lt;code&gt;false&lt;/code&gt; to turn database file locking off.
		/// 	</param>
		void LockDatabaseFile(bool flag);

		/// <summary>
		/// returns an
		/// <see cref="IObjectClass">IObjectClass</see>
		/// object
		/// to configure the specified class.
		/// &lt;br&gt;&lt;br&gt;
		/// The clazz parameter can be any of the following:&lt;br&gt;
		/// - a fully qualified classname as a String.&lt;br&gt;
		/// - a Class object.&lt;br&gt;
		/// - any other object to be used as a template.&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <param name="clazz">class name, Class object, or example object.&lt;br&gt;&lt;br&gt;
		/// 	</param>
		/// <returns>
		/// an instance of an
		/// <see cref="IObjectClass">IObjectClass</see>
		/// object for configuration.
		/// </returns>
		IObjectClass ObjectClass(object clazz);

		/// <summary>
		/// If set to true, db4o will try to optimize native queries
		/// dynamically at query execution time, otherwise it will
		/// run native queries in unoptimized mode as SODA evaluations.
		/// </summary>
		/// <remarks>
		/// If set to true, db4o will try to optimize native queries
		/// dynamically at query execution time, otherwise it will
		/// run native queries in unoptimized mode as SODA evaluations.
		/// On the Java platform the jars needed for native query
		/// optimization (db4o-X.x-nqopt.jar, bloat-X.x.jar) have to be
		/// on the classpath at runtime for this
		/// switch to have effect.
		/// &lt;br&gt;&lt;br&gt;The default setting is &lt;code&gt;true&lt;/code&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="optimizeNQ">
		/// true, if db4o should try to optimize
		/// native queries at query execution time, false otherwise
		/// </param>
		void OptimizeNativeQueries(bool optimizeNQ);

		/// <summary>indicates whether Native Queries will be optimized dynamically.</summary>
		/// <remarks>indicates whether Native Queries will be optimized dynamically.</remarks>
		/// <returns>
		/// boolean true if Native Queries will be optimized
		/// dynamically.
		/// </returns>
		/// <seealso cref="IConfiguration.OptimizeNativeQueries">IConfiguration.OptimizeNativeQueries
		/// 	</seealso>
		bool OptimizeNativeQueries();

		/// <summary>protects the database file with a password.</summary>
		/// <remarks>
		/// protects the database file with a password.
		/// &lt;br&gt;&lt;br&gt;To set a password for a database file, this method needs to be
		/// called &lt;b&gt;before&lt;/b&gt; a database file is created with the first
		/// <see cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;All further attempts to open
		/// the file, are required to set the same password.&lt;br&gt;&lt;br&gt;The password
		/// is used to seed the encryption mechanism, which makes it impossible
		/// to read the database file without knowing the password.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="pass">the password to be used.</param>
		/// <exception cref="GlobalOnlyConfigException"></exception>
		[System.ObsoleteAttribute(@"use a custom encrypting")]
		void Password(string pass);

		/// <summary>returns the Query configuration interface.</summary>
		/// <remarks>returns the Query configuration interface.</remarks>
		IQueryConfiguration Queries();

		/// <summary>turns readOnly mode on and off.</summary>
		/// <remarks>
		/// turns readOnly mode on and off.
		/// &lt;br&gt;&lt;br&gt;This method configures the mode in which subsequent calls to
		/// <see cref="Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// will open files.
		/// &lt;br&gt;&lt;br&gt;Readonly mode allows to open an unlimited number of reading
		/// processes on one database file. It is also convenient
		/// for deploying db4o database files on CD-ROM.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on the server side
		/// in embedded mode and ONLY on client side in networked mode.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">
		/// &lt;code&gt;true&lt;/code&gt; for configuring readOnly mode for subsequent
		/// calls to
		/// <see cref="Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// .
		/// </param>
		void ReadOnly(bool flag);

		/// <summary>configures the use of a specially designed reflection implementation.</summary>
		/// <remarks>
		/// configures the use of a specially designed reflection implementation.
		/// &lt;br&gt;&lt;br&gt;
		/// db4o internally uses java.lang.reflect.* by default. On platforms that
		/// do not support this package, customized implementations may be written
		/// to supply all the functionality of the interfaces in the com.db4o.reflect
		/// package. This method can be used to install a custom reflection
		/// implementation.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on the server side
		/// (reflector class must be available)&lt;br&gt;&lt;br&gt;
		/// </remarks>
		void ReflectWith(IReflector reflector);

		/// <summary>forces analysis of all Classes during a running session.</summary>
		/// <remarks>
		/// forces analysis of all Classes during a running session.
		/// &lt;br&gt;&lt;br&gt;
		/// This method may be useful in combination with a modified ClassLoader and
		/// allows exchanging classes during a running db4o session.&lt;br&gt;&lt;br&gt;
		/// Calling this method on the global Configuration context will refresh
		/// the classes in all db4o sessions in the running VM. Calling this method
		/// in an ObjectContainer Configuration context, only the classes of the
		/// respective ObjectContainer will be refreshed.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <seealso cref="IConfiguration.SetClassLoader">IConfiguration.SetClassLoader</seealso>
		void RefreshClasses();

		/// <summary>tuning feature only: reserves a number of bytes in database files.</summary>
		/// <remarks>
		/// tuning feature only: reserves a number of bytes in database files.
		/// &lt;br&gt;&lt;br&gt;The global setting is used for the creation of new database
		/// files. Continous calls on an ObjectContainer Configuration context
		/// (see
		/// <see cref="IExtObjectContainer.Configure">IExtObjectContainer.Configure</see>
		/// ) will
		/// continually allocate space.
		/// &lt;br&gt;&lt;br&gt;The allocation of a fixed number of bytes at one time
		/// makes it more likely that the database will be stored in one
		/// chunk on the mass storage. Less read/write head movevement can result
		/// in improved performance.&lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;Note:&lt;/b&gt;&lt;br&gt; Allocated space will be lost on abnormal termination
		/// of the database engine (hardware crash, VM crash). A Defragment run
		/// will recover the lost space. For the best possible performance, this
		/// method should be called before the Defragment run to configure the
		/// allocation of storage space to be slightly greater than the anticipated
		/// database file size.
		/// &lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on the server side. &lt;br&gt;&lt;br&gt;
		/// Default configuration: 0&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="byteCount">the number of bytes to reserve</param>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		void ReserveStorageSpace(long byteCount);

		/// <summary>
		/// configures the path to be used to store and read
		/// Blob data.
		/// </summary>
		/// <remarks>
		/// configures the path to be used to store and read
		/// Blob data.
		/// &lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on the
		/// server side. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="path">the path to be used</param>
		/// <exception cref="IOException"></exception>
		void SetBlobPath(string path);

		/// <summary>configures db4o to use a custom ClassLoader.</summary>
		/// <remarks>
		/// configures db4o to use a custom ClassLoader.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="classLoader">the ClassLoader to be used</param>
		[System.ObsoleteAttribute(@"use reflectWith(new JdkReflector(classLoader)) instead"
			)]
		void SetClassLoader(object classLoader);

		/// <summary>
		/// Assigns a
		/// <see cref="TextWriter">TextWriter</see>
		/// where db4o is to print its event messages.
		/// &lt;br&gt;&lt;br&gt;Messages are useful for debugging purposes and for learning
		/// to understand, how db4o works. The message level can be raised with
		/// <see cref="IConfiguration.MessageLevel">IConfiguration.MessageLevel</see>
		/// to produce more detailed messages.
		/// &lt;br&gt;&lt;br&gt;Use &lt;code&gt;setOut(System.out)&lt;/code&gt; to print messages to the
		/// console.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on the same side
		/// where
		/// <see cref="IConfiguration.MessageLevel">IConfiguration.MessageLevel</see>
		/// is used.&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <param name="outStream">the new &lt;code&gt;PrintStream&lt;/code&gt; for messages.
		/// 	</param>
		/// <seealso cref="IConfiguration.MessageLevel">IConfiguration.MessageLevel</seealso>
		void SetOut(TextWriter outStream);

		/// <summary>
		/// tuning feature: configures whether db4o should try to instantiate one instance
		/// of each persistent class on system startup.
		/// </summary>
		/// <remarks>
		/// tuning feature: configures whether db4o should try to instantiate one instance
		/// of each persistent class on system startup.
		/// &lt;br&gt;&lt;br&gt;In a production environment this setting can be set to &lt;code&gt;false&lt;/code&gt;,
		/// if all persistent classes have public default constructors.
		/// &lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both client and server
		/// side. &lt;br&gt;&lt;br&gt;
		/// Default value:&lt;br&gt;
		/// &lt;code&gt;true&lt;/code&gt;
		/// </remarks>
		/// <param name="flag">the desired setting</param>
		void TestConstructors(bool flag);

		/// <summary>configures the storage format of Strings.</summary>
		/// <remarks>
		/// configures the storage format of Strings.
		/// &lt;br&gt;&lt;br&gt;This method needs to be called &lt;b&gt;before&lt;/b&gt; a database file
		/// is created with the first
		/// <see cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</see>
		/// or
		/// <see cref="Db4oFactory.OpenServer">Db4oFactory.OpenServer</see>
		/// .
		/// db4o database files keep their string format after creation.&lt;br&gt;&lt;br&gt;
		/// Turning Unicode support off reduces the file storage space for strings
		/// by factor 2 and improves performance.&lt;br&gt;&lt;br&gt;
		/// Default setting: &lt;b&gt;true&lt;/b&gt;&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">
		/// &lt;code&gt;true&lt;/code&gt; for turning Unicode support on, &lt;code&gt;false&lt;/code&gt; for turning
		/// Unicode support off.
		/// </param>
		void Unicode(bool flag);

		/// <summary>specifies the global updateDepth.</summary>
		/// <remarks>
		/// specifies the global updateDepth.
		/// &lt;br&gt;&lt;br&gt;see the documentation of
		/// <see cref="IObjectContainer.Set"></see>
		/// for further details.&lt;br&gt;&lt;br&gt;
		/// The value be may be overridden for individual classes.&lt;br&gt;&lt;br&gt;
		/// The default setting is 1: Only the object passed to
		/// <see cref="IObjectContainer.Set">IObjectContainer.Set</see>
		/// will be updated.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both client and
		/// server sides.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="depth">the depth of the desired update.</param>
		/// <seealso cref="IObjectClass.UpdateDepth">IObjectClass.UpdateDepth</seealso>
		/// <seealso cref="IObjectClass.CascadeOnUpdate">IObjectClass.CascadeOnUpdate</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void UpdateDepth(int depth);

		/// <summary>turns weak reference management on or off.</summary>
		/// <remarks>
		/// turns weak reference management on or off.
		/// &lt;br&gt;&lt;br&gt;
		/// This method must be called before opening a database.
		/// &lt;br&gt;&lt;br&gt;
		/// Performance may be improved by running db4o without using weak
		/// references durring memory management at the cost of higher
		/// memory consumption or by alternatively implementing a manual
		/// memory management scheme using
		/// <see cref="IExtObjectContainer.Purge">IExtObjectContainer.Purge</see>
		/// &lt;br&gt;&lt;br&gt;Setting the value to &lt;code&gt;false&lt;/code&gt; causes db4o to use hard
		/// references to objects, preventing the garbage collection process
		/// from disposing of unused objects.
		/// &lt;br&gt;&lt;br&gt;The default setting is &lt;code&gt;true&lt;/code&gt;.
		/// &lt;br&gt;&lt;br&gt;Ignored on JDKs before 1.2.
		/// </remarks>
		void WeakReferences(bool flag);

		/// <summary>configures the timer for WeakReference collection.</summary>
		/// <remarks>
		/// configures the timer for WeakReference collection.
		/// &lt;br&gt;&lt;br&gt;The default setting is 1000 milliseconds.
		/// &lt;br&gt;&lt;br&gt;Configure this setting to zero to turn WeakReference
		/// collection off.
		/// &lt;br&gt;&lt;br&gt;Ignored on JDKs before 1.2.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="milliseconds">the time in milliseconds</param>
		void WeakReferenceCollectionInterval(int milliseconds);

		/// <summary>
		/// allows registering special TypeHandlers for customized marshalling
		/// and customized comparisons.
		/// </summary>
		/// <remarks>
		/// allows registering special TypeHandlers for customized marshalling
		/// and customized comparisons.
		/// </remarks>
		/// <param name="predicate">
		/// to specify for which classes and versions the
		/// TypeHandler is to be used.
		/// </param>
		/// <param name="typeHandler">to be used for the classes that match the predicate.</param>
		void RegisterTypeHandler(ITypeHandlerPredicate predicate, ITypeHandler4 typeHandler
			);
	}
}
