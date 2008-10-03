/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Config
{
	/// <since>7.5</since>
	public interface ILocalConfiguration
	{
		/// <summary>adds a new Alias for a class, namespace or package.</summary>
		/// <remarks>
		/// adds a new Alias for a class, namespace or package.
		/// <br /><br />Aliases can be used to persist classes in the running
		/// application to different persistent classes in a database file
		/// or on a db4o server.
		/// <br /><br />Two simple Alias implementations are supplied along with
		/// db4o:<br />
		/// -
		/// <see cref="TypeAlias">TypeAlias</see>
		/// provides an #equals() resolver to match
		/// names directly.<br />
		/// -
		/// <see cref="WildcardAlias">WildcardAlias</see>
		/// allows simple pattern matching
		/// with one single '*' wildcard character.<br />
		/// <br />
		/// It is possible to create
		/// own complex
		/// <see cref="IAlias">IAlias</see>
		/// constructs by creating own resolvers
		/// that implement the
		/// <see cref="IAlias">IAlias</see>
		/// interface.
		/// <br /><br />
		/// Examples of concrete usecases:
		/// <br /><br />
		/// <code>
		/// <b>// Creating an Alias for a single class</b><br />
		/// Db4o.configure().addAlias(<br />
		/// &#160;&#160;new TypeAlias("com.f1.Pilot", "com.f1.Driver"));<br />
		/// <br /><br />
		/// <b>// Accessing a .NET assembly from a Java package</b><br />
		/// Db4o.configure().addAlias(<br />
		/// &#160;&#160;new WildcardAlias(<br />
		/// &#160;&#160;&#160;&#160;"Tutorial.F1.*, Tutorial",<br />
		/// &#160;&#160;&#160;&#160;"com.f1.*"));<br />
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
		/// or connecting to a server.<br /><br />
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

		/// <summary>sets the storage data blocksize for new ObjectContainers.</summary>
		/// <remarks>
		/// sets the storage data blocksize for new ObjectContainers.
		/// <br /><br />The standard setting is 1 allowing for a maximum
		/// database file size of 2GB. This value can be increased
		/// to allow larger database files, although some space will
		/// be lost to padding because the size of some stored objects
		/// will not be an exact multiple of the block size. A
		/// recommended setting for large database files is 8, since
		/// internal pointers have this length.<br /><br />
		/// This setting is only effective when the database is first created, in
		/// client-server environment in most cases it means that the setting
		/// should be used on the server side.
		/// </remarks>
		/// <param name="bytes">the size in bytes from 1 to 127</param>
		int BlockSize
		{
			set;
		}

		/// <summary>
		/// configures the size database files should grow in bytes, when no
		/// free slot is found within.
		/// </summary>
		/// <remarks>
		/// configures the size database files should grow in bytes, when no
		/// free slot is found within.
		/// <br /><br />Tuning setting.
		/// <br /><br />Whenever no free slot of sufficient length can be found
		/// within the current database file, the database file's length
		/// is extended. This configuration setting configures by how much
		/// it should be extended, in bytes.<br /><br />
		/// This configuration setting is intended to reduce fragmentation.
		/// Higher values will produce bigger database files and less
		/// fragmentation.<br /><br />
		/// To extend the database file, a single byte array is created
		/// and written to the end of the file in one write operation. Be
		/// aware that a high setting will require allocating memory for
		/// this byte array.
		/// </remarks>
		/// <param name="bytes">amount of bytes</param>
		int DatabaseGrowthSize
		{
			set;
		}

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

		/// <summary>returns the freespace configuration interface.</summary>
		/// <remarks>returns the freespace configuration interface.</remarks>
		IFreespaceConfiguration Freespace
		{
			get;
		}

		/// <summary>configures db4o to generate UUIDs for stored objects.</summary>
		/// <remarks>
		/// configures db4o to generate UUIDs for stored objects.
		/// This setting should be used when the database is first created.<br /><br />
		/// </remarks>
		/// <param name="setting">the scope for UUID generation: disabled, generate for all classes, or configure individually
		/// 	</param>
		ConfigScope GenerateUUIDs
		{
			set;
		}

		/// <summary>configures db4o to generate version numbers for stored objects.</summary>
		/// <remarks>
		/// configures db4o to generate version numbers for stored objects.
		/// This setting should be used when the database is first created.
		/// </remarks>
		/// <param name="setting">the scope for version number generation: disabled, generate for all classes, or configure individually
		/// 	</param>
		ConfigScope GenerateVersionNumbers
		{
			set;
		}

		/// <summary>allows to configure db4o to use a customized byte IO adapter.</summary>
		/// <remarks>
		/// allows to configure db4o to use a customized byte IO adapter.
		/// <br /><br />Derive from the abstract class
		/// <see cref="IoAdapter">IoAdapter</see>
		/// to
		/// write your own. Possible usecases could be improved performance
		/// with a native library, mirrored write to two files, encryption or
		/// read-on-write fail-safety control.<br /><br />An example of a custom
		/// io adapter can be found in xtea_db4o community project:<br />
		/// http://developer.db4o.com/ProjectSpaces/view.aspx/XTEA<br /><br />
		/// In client-server environment this setting should be used on the server
		/// (adapter class must be available)<br /><br />
		/// </remarks>
		/// <param name="adapter">- the IoAdapter</param>
		/// <exception cref="GlobalOnlyConfigException"></exception>
		/// <summary>
		/// returns the configured
		/// <see cref="IoAdapter">IoAdapter</see>
		/// .
		/// </summary>
		/// <returns></returns>
		IoAdapter Io
		{
			get;
			set;
		}

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
		/// In client-server environment this setting should be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="flag"><code>false</code> to turn database file locking off.</param>
		bool LockDatabaseFile
		{
			set;
		}

		/// <summary>tuning feature only: reserves a number of bytes in database files.</summary>
		/// <remarks>
		/// tuning feature only: reserves a number of bytes in database files.
		/// <br /><br />The global setting is used for the creation of new database
		/// files. Continous calls on an ObjectContainer Configuration context
		/// (see
		/// <see cref="IExtObjectContainer.Configure">IExtObjectContainer.Configure</see>
		/// ) will
		/// continually allocate space.
		/// <br /><br />The allocation of a fixed number of bytes at one time
		/// makes it more likely that the database will be stored in one
		/// chunk on the mass storage. Less read/write head movement can result
		/// in improved performance.<br /><br />
		/// <b>Note:</b><br /> Allocated space will be lost on abnormal termination
		/// of the database engine (hardware crash, VM crash). A Defragment run
		/// will recover the lost space. For the best possible performance, this
		/// method should be called before the Defragment run to configure the
		/// allocation of storage space to be slightly greater than the anticipated
		/// database file size.
		/// <br /><br />
		/// In client-server environment this setting should be used on the server side. <br /><br />
		/// Default configuration: 0<br /><br />
		/// </remarks>
		/// <param name="byteCount">the number of bytes to reserve</param>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		long ReserveStorageSpace
		{
			set;
		}

		/// <summary>
		/// configures the path to be used to store and read
		/// Blob data.
		/// </summary>
		/// <remarks>
		/// configures the path to be used to store and read
		/// Blob data.
		/// <br /><br />
		/// In client-server environment this setting should be used on the
		/// server side. <br /><br />
		/// </remarks>
		/// <param name="path">the path to be used</param>
		/// <exception cref="IOException"></exception>
		string BlobPath
		{
			set;
		}

		/// <summary>turns readOnly mode on and off.</summary>
		/// <remarks>
		/// turns readOnly mode on and off.
		/// <br /><br />This method configures the mode in which subsequent calls to
		/// <see cref="Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// will open files.
		/// <br /><br />Readonly mode allows to open an unlimited number of reading
		/// processes on one database file. It is also convenient
		/// for deploying db4o database files on CD-ROM.<br /><br />
		/// In client-server environment this setting should be used on the server side
		/// in embedded mode and ONLY on client side in networked mode.<br /><br />
		/// </remarks>
		/// <param name="flag">
		/// <code>true</code> for configuring readOnly mode for subsequent
		/// calls to
		/// <see cref="Db4oFactory.OpenFile">Db4o.openFile()</see>
		/// .
		/// TODO: this is rather embedded + client than base?
		/// </param>
		bool ReadOnly
		{
			set;
		}
	}
}
