namespace Db4objects.Db4o.Config
{
	/// <summary>configuration interface.</summary>
	/// <remarks>
	/// configuration interface.
	/// <br /><br />This interface contains methods to configure db4o.<br /><br />
	/// The global Configuration context is available with
	/// <see cref="Db4objects.Db4o.Db4oFactory.Configure">Db4objects.Db4o.Db4oFactory.Configure
	/// 	</see>
	/// .
	/// When an ObjectContainer or ObjectServer is opened, the global Configuration
	/// context is cloned and copied into the ObjectContainer/ObjectServer.
	/// That means every ObjectContainer/ObjectServer gets it's own copy of
	/// configuration settings.<br /><br />
	/// <b>Most configuration settings should be set before opening an
	/// ObjectContainer/ObjectServer</b>.
	/// <br /><br />Some configuration settings can be modified on an open
	/// ObjectContainer/ObjectServer. The local Configuration context is
	/// available with
	/// <see cref="Db4objects.Db4o.Ext.IExtObjectContainer.Configure">Db4objects.Db4o.Ext.IExtObjectContainer.Configure
	/// 	</see>
	/// and
	/// <see cref="Db4objects.Db4o.Ext.IExtObjectServer.Configure">Db4objects.Db4o.Ext.IExtObjectServer.Configure
	/// 	</see>
	/// .
	/// </remarks>
	public interface IConfiguration
	{
		/// <summary>sets the activation depth to the specified value.</summary>
		/// <remarks>
		/// sets the activation depth to the specified value.
		/// <br /><br /><b>Why activation?</b><br />
		/// When objects are instantiated from the database, the instantiation of member
		/// objects needs to be limited to a certain depth. Otherwise a single object
		/// could lead to loading the complete database into memory, if all objects where
		/// reachable from a single root object.<br /><br />
		/// db4o uses the concept "depth", the number of field-to-field hops an object
		/// is away from another object. <b>The preconfigured "activation depth" db4o uses
		/// in the default setting is 5.</b>
		/// <br /><br />Whenever an application iterates through the
		/// <see cref="Db4objects.Db4o.IObjectSet">ObjectSet</see>
		/// of a query result, the result objects
		/// will be activated to the configured activation depth.<br /><br />
		/// A concrete example with the preconfigured activation depth of 5:<br />
		/// <pre>
		/// // Object foo is the result of a query, it is delivered by the ObjectSet
		/// Object foo = objectSet.next();</pre>
		/// foo.member1.member2.member3.member4.member5 will be a valid object<br />
		/// foo, member1, member2, member3 and member4 will be activated<br />
		/// member5 will be deactivated, all of it's members will be null<br />
		/// member5 can be activated at any time by calling
		/// <see cref="Db4objects.Db4o.IObjectContainer.Activate">ObjectContainer#activate(member5, depth)
		/// 	</see>
		/// .
		/// <br /><br />
		/// Note that raising the global activation depth will consume more memory and
		/// have negative effects on the performance of first-time retrievals. Lowering
		/// the global activation depth needs more individual activation work but can
		/// increase performance of queries.<br /><br />
		/// <see cref="Db4objects.Db4o.IObjectContainer.Deactivate">ObjectContainer#deactivate(Object, depth)
		/// 	</see>
		/// can be used to manually free memory by deactivating objects.<br /><br />
		/// </remarks>
		/// <param name="depth">the desired global activation depth.</param>
		/// <seealso cref="Db4objects.Db4o.Config.IObjectClass.MaximumActivationDepth">configuring classes individually
		/// 	</seealso>
		void ActivationDepth(int depth);

		/// <summary>adds a new Alias for a class, namespace or package.</summary>
		/// <remarks>
		/// adds a new Alias for a class, namespace or package.
		/// <br /><br />Aliases can be used to persist classes in the running
		/// application to different persistent classes in a database file
		/// or on a db4o server.
		/// <br /><br />Two simple Alias implementations are supplied along with
		/// db4o:<br />
		/// -
		/// <see cref="Db4objects.Db4o.Config.TypeAlias">Db4objects.Db4o.Config.TypeAlias</see>
		/// provides an #equals() resolver to match
		/// names directly.<br />
		/// -
		/// <see cref="Db4objects.Db4o.Config.WildcardAlias">Db4objects.Db4o.Config.WildcardAlias
		/// 	</see>
		/// allows simple pattern matching
		/// with one single '*' wildcard character.<br />
		/// <br />
		/// It is possible to create
		/// own complex
		/// <see cref="Db4objects.Db4o.Config.IAlias">Db4objects.Db4o.Config.IAlias</see>
		/// constructs by creating own resolvers
		/// that implement the
		/// <see cref="Db4objects.Db4o.Config.IAlias">Db4objects.Db4o.Config.IAlias</see>
		/// interface.
		/// <br /><br />
		/// Four examples of concrete usecases:
		/// <br /><br />
		/// <code>
		/// <b>// Creating an Alias for a single class</b><br />
		/// Db4o.configure().addAlias(<br />
		/// &#160;&#160;new TypeAlias("com.f1.Pilot", "com.f1.Driver"));<br />
		/// <br /><br />
		/// <b>// Accessing a .NET assembly from a Java package</b><br />
		/// Db4o.configure().addAlias(<br />
		/// &#160;&#160;new WildcardAlias(<br />
		/// &#160;&#160;&#160;&#160;"com.f1.*, F1RaceAssembly",<br />
		/// &#160;&#160;&#160;&#160;"com.f1.*"));<br />
		/// <br /><br />
		/// <b>// Using a different local .NET assembly</b><br />
		/// Db4o.configure().addAlias(<br />
		/// &#160;&#160;new WildcardAlias(<br />
		/// &#160;&#160;&#160;&#160;"com.f1.*, F1RaceAssembly",<br />
		/// &#160;&#160;&#160;&#160;"com.f1.*, RaceClient"));<br />
		/// <br /><br />
		/// <b>// Mapping a Java package onto another</b><br />
		/// Db4o.configure().addAlias(<br />
		/// &#160;&#160;new WildcardAlias(<br />
		/// &#160;&#160;&#160;&#160;"com.f1.*",<br />
		/// &#160;&#160;&#160;&#160;"com.f1.client*"));<br /></code>
		/// <br /><br />Aliases that translate the persistent name of a class to
		/// a name that already exists as a persistent name in the database
		/// (or on the server) are not permitted and will throw an exception
		/// when the database file is opened.
		/// <br /><br />Aliases should be configured before opening a database file
		/// or connecting to a server.
		/// </remarks>
		void AddAlias(Db4objects.Db4o.Config.IAlias alias);

		/// <summary>
		/// Removes an alias previously added with
		/// <see cref="addAlias">addAlias</see>
		/// .
		/// </summary>
		/// <param name="alias">the alias to remove</param>
		void RemoveAlias(Db4objects.Db4o.Config.IAlias alias);

		/// <summary>turns automatic database file format version updates on.</summary>
		/// <remarks>
		/// turns automatic database file format version updates on.
		/// <br /><br />Upon db4o database file format version changes,
		/// db4o can automatically update database files to the
		/// current version. db4objects does not provide functionality
		/// to reverse this process. It is not ensured that updated
		/// database files can be read with older db4o versions.
		/// In some cases (Example: using ObjectManager) it may not be
		/// desirable to update database files automatically therefore
		/// automatic updating is turned off by default for
		/// security reasons.
		/// <br /><br />Call this method to turn automatic database file
		/// version updating on.
		/// <br /><br />If automatic updating is turned off, db4o will refuse
		/// to open database files that use an older database file format.
		/// </remarks>
		void AllowVersionUpdates(bool flag);

		/// <summary>turns automatic shutdown of the engine on and off.</summary>
		/// <remarks>
		/// turns automatic shutdown of the engine on and off.
		/// <br /><br />Depending on the JDK, db4o uses one of the following
		/// two methods to shut down, if no more references to the ObjectContainer
		/// are being held or the JVM terminates:<br />
		/// - JDK 1.3 and above: <code>Runtime.addShutdownHook()</code><br />
		/// - JDK 1.2 and below: <code>System.runFinalizersOnExit(true)</code> and code
		/// in the finalizer.<br /><br />
		/// Some JVMs have severe problems with both methods. For these rare cases the
		/// autoShutDown feature may be turned off.<br /><br />
		/// The default and recommended setting is <code>true</code>.<br /><br />
		/// </remarks>
		/// <param name="flag">whether db4o should shut down automatically.</param>
		void AutomaticShutDown(bool flag);

		/// <summary>sets the storage data blocksize for new ObjectContainers.</summary>
		/// <remarks>
		/// sets the storage data blocksize for new ObjectContainers.
		/// <br /><br />The standard setting is 1 allowing for a maximum
		/// database file size of 2GB. This value can be increased
		/// to allow larger database files, although some space will
		/// be lost to padding because the size of some stored objects
		/// will not be an exact multiple of the block size. A
		/// recommended setting for large database files is 8, since
		/// internal pointers have this length.
		/// </remarks>
		/// <param name="bytes">the size in bytes from 1 to 127</param>
		void BlockSize(int bytes);

		/// <summary>configures the size of BTree nodes in indexes.</summary>
		/// <remarks>
		/// configures the size of BTree nodes in indexes.
		/// <br /><br />Default setting: 100
		/// <br />Lower values will allow a lower memory footprint
		/// and more efficient reading and writing of small slots.
		/// <br />Higher values will reduce the overall number of
		/// read and write operations and allow better performance
		/// at the cost of more RAM use.
		/// </remarks>
		/// <param name="size">the number of elements held in one BTree node.</param>
		void BTreeNodeSize(int size);

		/// <summary>configures caching of BTree nodes.</summary>
		/// <remarks>
		/// configures caching of BTree nodes.
		/// <br /><br />Clean BTree nodes will be unloaded on #commit and
		/// #rollback unless they are configured as cached here.
		/// <br /><br />Default setting: 0
		/// <br />Possible settings: 1, 2 or 3
		/// <br /><br /> The potential number of cached BTree nodes can be
		/// calculated with the following forumula:<br />
		/// maxCachedNodes = bTreeNodeSize ^ bTreeCacheHeight
		/// </remarks>
		/// <param name="height">the height of the cache from the root</param>
		void BTreeCacheHeight(int height);

		/// <summary>turns callback methods on and off.</summary>
		/// <remarks>
		/// turns callback methods on and off.
		/// <br /><br />Callbacks are turned on by default.<br /><br />
		/// A tuning hint: If callbacks are not used, you can turn this feature off, to
		/// prevent db4o from looking for callback methods in persistent classes. This will
		/// increase the performance on system startup.<br /><br />
		/// </remarks>
		/// <param name="flag">false to turn callback methods off</param>
		/// <seealso cref="Db4objects.Db4o.Ext.IObjectCallbacks">Using callbacks</seealso>
		void Callbacks(bool flag);

		/// <summary>
		/// advises db4o to try instantiating objects with/without calling
		/// constructors.
		/// </summary>
		/// <remarks>
		/// advises db4o to try instantiating objects with/without calling
		/// constructors.
		/// <br /><br />
		/// Not all JDKs / .NET-environments support this feature. db4o will
		/// attempt, to follow the setting as good as the enviroment supports.
		/// In doing so, it may call implementation-specific features like
		/// sun.reflect.ReflectionFactory#newConstructorForSerialization on the
		/// Sun Java 1.4.x/5 VM (not available on other VMs) and
		/// FormatterServices.GetUninitializedObject() on
		/// the .NET framework (not available on CompactFramework).
		/// This setting may also be overridden for individual classes in
		/// <see cref="Db4objects.Db4o.Config.IObjectClass.CallConstructor">Db4objects.Db4o.Config.IObjectClass.CallConstructor
		/// 	</see>
		/// .
		/// <br /><br />The default setting depends on the features supported by your current environment.
		/// <br /><br />
		/// </remarks>
		/// <param name="flag">
		/// - specify true, to request calling constructors, specify
		/// false to request <b>not</b> calling constructors.
		/// </param>
		/// <seealso cref="Db4objects.Db4o.Config.IObjectClass.CallConstructor">Db4objects.Db4o.Config.IObjectClass.CallConstructor
		/// 	</seealso>
		void CallConstructors(bool flag);

		/// <summary>
		/// turns
		/// <see cref="Db4objects.Db4o.Config.IObjectClass.MaximumActivationDepth">individual class activation depth configuration
		/// 	</see>
		/// on
		/// and off.
		/// <br /><br />This feature is turned on by default.<br /><br />
		/// </summary>
		/// <param name="flag">
		/// false to turn the possibility to individually configure class
		/// activation depths off
		/// </param>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ActivationDepth">Why activation?
		/// 	</seealso>
		void ClassActivationDepthConfigurable(bool flag);

		/// <summary>
		/// tuning feature: configures whether db4o checks all persistent classes upon system
		/// startup, for added or removed fields.
		/// </summary>
		/// <remarks>
		/// tuning feature: configures whether db4o checks all persistent classes upon system
		/// startup, for added or removed fields.
		/// <br /><br />In a production environment this setting can be set to <code>false</code>,
		/// if all necessary classes have been stored to the database file and none of them
		/// have been modified since the last use.
		/// <br /><br />Default value:<br />
		/// <code>true</code>
		/// </remarks>
		/// <param name="flag">the desired setting</param>
		void DetectSchemaChanges(bool flag);

		/// <summary>returns the configuration interface for diagnostics.</summary>
		/// <remarks>returns the configuration interface for diagnostics.</remarks>
		/// <returns>the configuration interface for diagnostics.</returns>
		Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration Diagnostic();

		/// <summary>turns commit recovery off.</summary>
		/// <remarks>
		/// turns commit recovery off.
		/// <br /><br />db4o uses a two-phase commit algorithm. In a first step all intended
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
		/// <br /><br />When objects are updated or deleted, the space previously occupied in the
		/// database file is marked as "free", so it can be reused. db4o maintains two lists
		/// in RAM, sorted by address and by size. Adjacent entries are merged. After a large
		/// number of updates or deletes have been executed, the lists can become large, causing
		/// RAM consumption and performance loss for maintenance. With this method you can
		/// specify an upper bound for the byte slot size to discard.
		/// <br /><br />Pass <code>Integer.MAX_VALUE</code> to this method to discard all free slots for
		/// the best possible startup time.<br /><br />
		/// The downside of setting this value: Database files will necessarily grow faster.
		/// <br /><br />Default value:<br />
		/// <code>0</code> all space is reused
		/// </remarks>
		/// <param name="byteCount">Slots with this size or smaller will be lost.</param>
		/// <deprecated>please call Db4o.configure().freespace().discardSmallerThan()</deprecated>
		void DiscardFreeSpace(int byteCount);

		/// <summary>configures the use of encryption.</summary>
		/// <remarks>
		/// configures the use of encryption.
		/// <br /><br />This method needs to be called <b>before</b> a database file
		/// is created with the first
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// .
		/// <br /><br />If encryption is set to true,
		/// you need to supply a password to seed the encryption mechanism.<br /><br />
		/// db4o database files keep their encryption format after creation.<br /><br />
		/// </remarks>
		/// <param name="flag">
		/// true for turning encryption on, false for turning encryption
		/// off.
		/// </param>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Password">Db4objects.Db4o.Config.IConfiguration.Password
		/// 	</seealso>
		/// <deprecated>
		/// Please use:
		/// Db4o.configure().io(new XTeaEncryptionFileAdapter(password)) or any of the
		/// other XTeaEncryptionFileAdapter constructors.
		/// </deprecated>
		void Encrypt(bool flag);

		/// <summary>configures whether Exceptions are to be thrown, if objects can not be stored.
		/// 	</summary>
		/// <remarks>
		/// configures whether Exceptions are to be thrown, if objects can not be stored.
		/// <br /><br />db4o requires the presence of a constructor that can be used to
		/// instantiate objects. If no default public constructor is present, all
		/// available constructors are tested, whether an instance of the class can
		/// be instantiated. Null is passed to all constructor parameters.
		/// The first constructor that is successfully tested will
		/// be used throughout the running db4o session. If an instance of the class
		/// can not be instantiated, the object will not be stored. By default,
		/// execution will continue without any message or error. This method can
		/// be used to configure db4o to throw an
		/// <see cref="Db4objects.Db4o.Ext.ObjectNotStorableException">ObjectNotStorableException
		/// 	</see>
		/// if an object can not be stored.
		/// <br /><br />
		/// The default for this setting is <b>false</b>.<br /><br />
		/// </remarks>
		/// <param name="flag">true to throw Exceptions if objects can not be stored.</param>
		void ExceptionsOnNotStorable(bool flag);

		/// <summary>configures file buffers to be flushed during transaction commits.</summary>
		/// <remarks>
		/// configures file buffers to be flushed during transaction commits.
		/// <br /><br />
		/// db4o uses a resume-commit-on-crash strategy to ensure ACID transactions.
		/// When a transaction commits,<br />
		/// - (1) a list "pointers that are to be modified" is written to the database file,<br />
		/// - (2) the database file is switched into "in-commit" mode, <br />
		/// - (3) the pointers are actually modified in the database file,<br />
		/// - (4) the database file is switched to "not-in-commit" mode.<br />
		/// If the system is halted by a hardware or power failure <br />
		/// - before (2)<br />
		/// all objects will be available as before the commit<br />
		/// - between (2) and (4)
		/// the commit is restarted when the database file is opened the next time, all pointers
		/// will be read from the "pointers to be modified" list and all of them will be modified
		/// to the state they are intended to have after commit<br />
		/// - after (4)
		/// no work is necessary, the transaction is committed.
		/// <br /><br />
		/// In order for the above to be 100% failsafe, the order of writes to the
		/// storage medium has to be obeyed. On operating systems that use in-memory
		/// file caching, the OS cache may revert the order of writes to optimize
		/// file performance.<br /><br />
		/// db4o enforces the correct write order by flushing file
		/// buffers after every single one of the above steps during transaction
		/// commit. Flush calls have a strong impact on performance. It is possible to
		/// tune an application by turning flushing off, at the risc of less security
		/// for hardware-, power- or operating system failures.
		/// </remarks>
		/// <param name="flag">true for flushing file buffers</param>
		void FlushFileBuffers(bool flag);

		/// <summary>returns the freespace configuration interface.</summary>
		/// <remarks>returns the freespace configuration interface.</remarks>
		Db4objects.Db4o.Config.IFreespaceConfiguration Freespace();

		/// <summary>configures db4o to generate UUIDs for stored objects.</summary>
		/// <remarks>configures db4o to generate UUIDs for stored objects.</remarks>
		/// <param name="setting">
		/// one of the following values:<br />
		/// -1 - off<br />
		/// 1 - configure classes individually<br />
		/// Integer.MAX_Value - on for all classes
		/// </param>
		/// <deprecated>
		/// Use
		/// <see cref="Db4objects.Db4o.Config.IConfiguration.GenerateUUIDs">Db4objects.Db4o.Config.IConfiguration.GenerateUUIDs
		/// 	</see>
		/// instead.
		/// </deprecated>
		void GenerateUUIDs(int setting);

		/// <summary>configures db4o to generate UUIDs for stored objects.</summary>
		/// <remarks>configures db4o to generate UUIDs for stored objects.</remarks>
		/// <param name="the">scope for UUID generation: disabled, generate for all classes, or configure individually
		/// 	</param>
		void GenerateUUIDs(Db4objects.Db4o.Config.ConfigScope setting);

		/// <summary>configures db4o to generate version numbers for stored objects.</summary>
		/// <remarks>configures db4o to generate version numbers for stored objects.</remarks>
		/// <param name="setting">
		/// one of the following values:<br />
		/// -1 - off<br />
		/// 1 - configure classes individually<br />
		/// Integer.MAX_Value - on for all classes
		/// </param>
		/// <deprecated>
		/// Use
		/// <see cref="Db4objects.Db4o.Config.IConfiguration.GenerateVersionNumbers">Db4objects.Db4o.Config.IConfiguration.GenerateVersionNumbers
		/// 	</see>
		/// instead.
		/// </deprecated>
		void GenerateVersionNumbers(int setting);

		/// <summary>configures db4o to generate version numbers for stored objects.</summary>
		/// <remarks>configures db4o to generate version numbers for stored objects.</remarks>
		/// <param name="the">scope for version number generation: disabled, generate for all classes, or configure individually
		/// 	</param>
		void GenerateVersionNumbers(Db4objects.Db4o.Config.ConfigScope setting);

		/// <summary>Configures db4o to call intern() on strings upon retrieval.</summary>
		/// <remarks>Configures db4o to call intern() on strings upon retrieval.</remarks>
		/// <param name="doIntern">intern strings on retrieval if true, don't otherwise</param>
		void InternStrings(bool doIntern);

		/// <summary>allows to configure db4o to use a customized byte IO adapter.</summary>
		/// <remarks>
		/// allows to configure db4o to use a customized byte IO adapter.
		/// <br /><br />Derive from the abstract class
		/// <see cref="Db4objects.Db4o.IO.IoAdapter">Db4objects.Db4o.IO.IoAdapter</see>
		/// to
		/// write your own. Possible usecases could be improved performance
		/// with a native library, mirrored write to two files or
		/// read-on-write fail-safety control.<br /><br />Sample IoAdapters
		/// are supplied with the distribution as source code.
		/// </remarks>
		/// <param name="adapter">- the IoAdapter</param>
		void Io(Db4objects.Db4o.IO.IoAdapter adapter);

		/// <summary>allows to mark fields as transient with custom attributes.</summary>
		/// <remarks>
		/// allows to mark fields as transient with custom attributes.
		/// <br /><br />.NET only: Call this method with the attribute name that you
		/// wish to use to mark fields as transient. Multiple transient attributes
		/// are possible by calling this method multiple times with different
		/// attribute names.<br /><br />
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
		/// <see cref="System.IO.TextWriter">PrintStream</see>
		/// .
		/// <br /><br />
		/// Level 0 - no messages<br />
		/// Level 1 - open and close messages<br />
		/// Level 2 - messages for new, update and delete<br />
		/// Level 3 - messages for activate and deactivate<br /><br />
		/// When using client-server and the level is set to 0, the server will override this and set it to 1.  To get around this you can set the level to -1.  This has the effect of not returning any messages.<br /><br />
		/// </remarks>
		/// <param name="level">integer from 0 to 3</param>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.SetOut">Db4objects.Db4o.Config.IConfiguration.SetOut
		/// 	</seealso>
		void MessageLevel(int level);

		/// <summary>can be used to turn the database file locking thread off.</summary>
		/// <remarks>
		/// can be used to turn the database file locking thread off.
		/// <br /><br />Since Java does not support file locking up to JDK 1.4,
		/// db4o uses an additional thread per open database file to prohibit
		/// concurrent access to the same database file by different db4o
		/// sessions in different VMs.<br /><br />
		/// To improve performance and to lower ressource consumption, this
		/// method provides the possibility to prevent the locking thread
		/// from being started.<br /><br /><b>Caution!</b><br />If database file
		/// locking is turned off, concurrent write access to the same
		/// database file from different JVM sessions will <b>corrupt</b> the
		/// database file immediately.<br /><br /> This method
		/// has no effect on open ObjectContainers. It will only affect how
		/// ObjectContainers are opened.<br /><br />
		/// The default setting is <code>true</code>.<br /><br />
		/// </remarks>
		/// <param name="flag"><code>false</code> to turn database file locking off.</param>
		void LockDatabaseFile(bool flag);

		/// <summary>
		/// returns an
		/// <see cref="Db4objects.Db4o.Config.IObjectClass">ObjectClass</see>
		/// object
		/// to configure the specified class.
		/// <br /><br />
		/// The clazz parameter can be any of the following:<br />
		/// - a fully qualified classname as a String.<br />
		/// - a Class object.<br />
		/// - any other object to be used as a template.<br /><br />
		/// </summary>
		/// <param name="clazz">class name, Class object, or example object.<br /><br /></param>
		/// <returns>
		/// an instance of an
		/// <see cref="Db4objects.Db4o.Config.IObjectClass">ObjectClass</see>
		/// object for configuration.
		/// </returns>
		Db4objects.Db4o.Config.IObjectClass ObjectClass(object clazz);

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
		/// <br /><br />The default setting is <code>true</code>.
		/// </remarks>
		/// <param name="optimizeNQ">
		/// true, if db4o should try to optimize
		/// native queries at query execution time, false otherwise
		/// </param>
		void OptimizeNativeQueries(bool optimizeNQ);

		/// <summary>
		/// indicates whether Native Queries will be optimized
		/// dynamically.
		/// </summary>
		/// <remarks>
		/// indicates whether Native Queries will be optimized
		/// dynamically.
		/// </remarks>
		/// <returns>
		/// boolean indicates whether Native Queries will be optimized
		/// dynamically.
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.OptimizeNativeQueries">Db4objects.Db4o.Config.IConfiguration.OptimizeNativeQueries
		/// 	</seealso>
		bool OptimizeNativeQueries();

		/// <summary>protects the database file with a password.</summary>
		/// <remarks>
		/// protects the database file with a password.
		/// <br /><br />To set a password for a database file, this method needs to be
		/// called <b>before</b> a database file is created with the first
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// .
		/// <br /><br />All further attempts to open
		/// the file, are required to set the same password.<br /><br />The password
		/// is used to seed the encryption mechanism, which makes it impossible
		/// to read the database file without knowing the password.<br /><br />
		/// </remarks>
		/// <param name="pass">the password to be used.</param>
		/// <deprecated>
		/// Please use:
		/// Db4o.configure().io(new XTeaEncryptionFileAdapter(password)) or any of the
		/// other XTeaEncryptionFileAdapter constructors.
		/// </deprecated>
		void Password(string pass);

		/// <summary>returns the Query configuration interface.</summary>
		/// <remarks>returns the Query configuration interface.</remarks>
		Db4objects.Db4o.Config.IQueryConfiguration Queries();

		/// <summary>turns readOnly mode on and off.</summary>
		/// <remarks>
		/// turns readOnly mode on and off.
		/// <br /><br />This method configures the mode in which subsequent calls to
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// will open files.
		/// <br /><br />Readonly mode allows to open an unlimited number of reading
		/// processes on one database file. It is also convenient
		/// for deploying db4o database files on CD-ROM.<br />
		/// </remarks>
		/// <param name="flag">
		/// <code>true</code> for configuring readOnly mode for subsequent
		/// calls to
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// .
		/// </param>
		void ReadOnly(bool flag);

		/// <summary>configures the use of a specially designed reflection implementation.</summary>
		/// <remarks>
		/// configures the use of a specially designed reflection implementation.
		/// <br /><br />
		/// db4o internally uses java.lang.reflect.* by default. On platforms that
		/// do not support this package, customized implementations may be written
		/// to supply all the functionality of the interfaces in the com.db4o.reflect
		/// package. This method can be used to install a custom reflection
		/// implementation.
		/// </remarks>
		void ReflectWith(Db4objects.Db4o.Reflect.IReflector reflector);

		/// <summary>forces analysis of all Classes during a running session.</summary>
		/// <remarks>
		/// forces analysis of all Classes during a running session.
		/// <br /><br />
		/// This method may be useful in combination with a modified ClassLoader and
		/// allows exchanging classes during a running db4o session.<br /><br />
		/// Calling this method on the global Configuration context will refresh
		/// the classes in all db4o sessions in the running VM. Calling this method
		/// in an ObjectContainer Configuration context, only the classes of the
		/// respective ObjectContainer will be refreshed.<br /><br />
		/// </remarks>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.SetClassLoader">Db4objects.Db4o.Config.IConfiguration.SetClassLoader
		/// 	</seealso>
		void RefreshClasses();

		/// <summary>tuning feature only: reserves a number of bytes in database files.</summary>
		/// <remarks>
		/// tuning feature only: reserves a number of bytes in database files.
		/// <br /><br />The global setting is used for the creation of new database
		/// files. Continous calls on an ObjectContainer Configuration context
		/// (see
		/// <see cref="Db4objects.Db4o.Ext.IExtObjectContainer.Configure">Db4objects.Db4o.Ext.IExtObjectContainer.Configure
		/// 	</see>
		/// ) will
		/// continually allocate space.
		/// <br /><br />The allocation of a fixed number of bytes at one time
		/// makes it more likely that the database will be stored in one
		/// chunk on the mass storage. Less read/write head movevement can result
		/// in improved performance.<br /><br />
		/// <b>Note:</b><br /> Allocated space will be lost on abnormal termination
		/// of the database engine (hardware crash, VM crash). A Defragment run
		/// will recover the lost space. For the best possible performance, this
		/// method should be called before the Defragment run to configure the
		/// allocation of storage space to be slightly greater than the anticipated
		/// database file size.
		/// <br /><br /> Default configuration: 0<br /><br />
		/// </remarks>
		/// <param name="byteCount">the number of bytes to reserve</param>
		void ReserveStorageSpace(long byteCount);

		/// <summary>
		/// configures the path to be used to store and read
		/// Blob data.
		/// </summary>
		/// <remarks>
		/// configures the path to be used to store and read
		/// Blob data.
		/// <br /><br />
		/// </remarks>
		/// <param name="path">the path to be used</param>
		void SetBlobPath(string path);

		/// <summary>configures db4o to use a custom ClassLoader.</summary>
		/// <remarks>
		/// configures db4o to use a custom ClassLoader.
		/// <br /><br />
		/// </remarks>
		/// <param name="classLoader">the ClassLoader to be used</param>
		/// <deprecated>use reflectWith(new JdkReflector(classLoader)) instead</deprecated>
		void SetClassLoader(object classLoader);

		/// <summary>
		/// Assigns a
		/// <see cref="System.IO.TextWriter">PrintStream</see>
		/// where db4o is to print its event messages.
		/// <br /><br />Messages are useful for debugging purposes and for learning
		/// to understand, how db4o works. The message level can be raised with
		/// <see cref="Db4objects.Db4o.Config.IConfiguration.MessageLevel">Db4o.configure().messageLevel()
		/// 	</see>
		/// to produce more detailed messages.
		/// <br /><br />Use <code>setOut(System.out)</code> to print messages to the
		/// console.<br /><br />
		/// </summary>
		/// <param name="outStream">the new <code>PrintStream</code> for messages.</param>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.MessageLevel">Db4objects.Db4o.Config.IConfiguration.MessageLevel
		/// 	</seealso>
		void SetOut(System.IO.TextWriter outStream);

		/// <summary>
		/// tuning feature: configures whether db4o should try to instantiate one instance
		/// of each persistent class on system startup.
		/// </summary>
		/// <remarks>
		/// tuning feature: configures whether db4o should try to instantiate one instance
		/// of each persistent class on system startup.
		/// <br /><br />In a production environment this setting can be set to <code>false</code>,
		/// if all persistent classes have public default constructors.
		/// <br /><br />Default value:<br />
		/// <code>true</code>
		/// </remarks>
		/// <param name="flag">the desired setting</param>
		void TestConstructors(bool flag);

		/// <summary>configures the storage format of Strings.</summary>
		/// <remarks>
		/// configures the storage format of Strings.
		/// <br /><br />This method needs to be called <b>before</b> a database file
		/// is created with the first
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// .
		/// db4o database files keep their string format after creation.<br /><br />
		/// Turning Unicode support off reduces the file storage space for strings
		/// by factor 2 and improves performance.<br /><br />
		/// Default setting: <b>true</b><br /><br />
		/// </remarks>
		/// <param name="flag">
		/// <code>true</code> for turning Unicode support on, <code>false</code> for turning
		/// Unicode support off.
		/// </param>
		void Unicode(bool flag);

		/// <summary>specifies the global updateDepth.</summary>
		/// <remarks>
		/// specifies the global updateDepth.
		/// <br /><br />see the documentation of
		/// <see cref="Db4objects.Db4o.IObjectContainer.Set">ObjectContainer.set()</see>
		/// for further details.<br /><br />
		/// The value be may be overridden for individual classes.<br /><br />
		/// The default setting is 1: Only the object passed to
		/// <see cref="Db4objects.Db4o.IObjectContainer.Set">ObjectContainer.set()</see>
		/// will be updated.<br /><br />
		/// </remarks>
		/// <param name="depth">the depth of the desired update.</param>
		/// <seealso cref="Db4objects.Db4o.Config.IObjectClass.UpdateDepth">Db4objects.Db4o.Config.IObjectClass.UpdateDepth
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IObjectClass.CascadeOnUpdate">Db4objects.Db4o.Config.IObjectClass.CascadeOnUpdate
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Ext.IObjectCallbacks">Using callbacks</seealso>
		void UpdateDepth(int depth);

		/// <summary>turns weak reference management on or off.</summary>
		/// <remarks>
		/// turns weak reference management on or off.
		/// <br /><br />
		/// This method must be called before opening a database.
		/// <br /><br />
		/// Performance may be improved by running db4o without using weak
		/// references durring memory management at the cost of higher
		/// memory consumption or by alternatively implementing a manual
		/// memory management scheme using
		/// <see cref="Db4objects.Db4o.Ext.IExtObjectContainer.Purge">Db4objects.Db4o.Ext.IExtObjectContainer.Purge
		/// 	</see>
		/// <br /><br />Setting the value to <code>false</code> causes db4o to use hard
		/// references to objects, preventing the garbage collection process
		/// from disposing of unused objects.
		/// <br /><br />The default setting is <code>true</code>.
		/// <br /><br />Ignored on JDKs before 1.2.
		/// </remarks>
		void WeakReferences(bool flag);

		/// <summary>configures the timer for WeakReference collection.</summary>
		/// <remarks>
		/// configures the timer for WeakReference collection.
		/// <br /><br />The default setting is 1000 milliseconds.
		/// <br /><br />Configure this setting to zero to turn WeakReference
		/// collection off.
		/// <br /><br />Ignored on JDKs before 1.2.<br /><br />
		/// </remarks>
		/// <param name="milliseconds">the time in milliseconds</param>
		void WeakReferenceCollectionInterval(int milliseconds);

		/// <summary>returns client/server configuration interface.</summary>
		/// <remarks>returns client/server configuration interface.</remarks>
		Db4objects.Db4o.Config.IClientServerConfiguration ClientServer();
	}
}
