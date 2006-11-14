namespace Db4objects.Db4o.Ext
{
	/// <summary>extended factory class with static methods to open special db4o sessions.
	/// 	</summary>
	/// <remarks>extended factory class with static methods to open special db4o sessions.
	/// 	</remarks>
	public sealed class ExtDb4oFactory : Db4objects.Db4o.Db4oFactory
	{
		/// <summary>
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectContainer">Db4objects.Db4o.IObjectContainer</see>
		/// for in-memory use .
		/// <br /><br />In-memory ObjectContainers are useful for maximum performance
		/// on small databases, for swapping objects or for storing db4o format data
		/// to other media or other databases.<br /><br />Be aware of the danger of running
		/// into OutOfMemory problems or complete loss of all data, in case of hardware
		/// or JVM failures.<br /><br />
		/// </summary>
		/// <param name="memoryFile">
		/// a
		/// <see cref="Db4objects.Db4o.Ext.MemoryFile">MemoryFile</see>
		/// 
		/// to store the raw byte data.
		/// </param>
		/// <returns>
		/// an open
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Ext.MemoryFile">Db4objects.Db4o.Ext.MemoryFile</seealso>
		public static Db4objects.Db4o.IObjectContainer OpenMemoryFile(Db4objects.Db4o.Ext.MemoryFile
			 memoryFile)
		{
			return OpenMemoryFile1(Db4objects.Db4o.Db4oFactory.NewConfiguration(), memoryFile
				);
		}

		public static Db4objects.Db4o.IObjectContainer OpenMemoryFile(Db4objects.Db4o.Config.IConfiguration
			 config, Db4objects.Db4o.Ext.MemoryFile memoryFile)
		{
			return OpenMemoryFile1(config, memoryFile);
		}
	}
}
