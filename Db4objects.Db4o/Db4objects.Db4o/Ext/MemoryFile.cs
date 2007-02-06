namespace Db4objects.Db4o.Ext
{
	/// <summary>carries in-memory data for db4o in-memory operation.</summary>
	/// <remarks>
	/// carries in-memory data for db4o in-memory operation.
	/// <br /><br />In-memory ObjectContainers are useful for maximum performance
	/// on small databases, for swapping objects or for storing db4o format data
	/// to other media or other databases.<br /><br />Be aware of the danger of running
	/// into OutOfMemory problems or complete loss of all data, in case of hardware
	/// or JVM failures.
	/// <br /><br />
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Ext.ExtDb4oFactory.OpenMemoryFile">Db4objects.Db4o.Ext.ExtDb4oFactory.OpenMemoryFile
	/// 	</seealso>
	public class MemoryFile
	{
		private byte[] i_bytes;

		private const int INITIAL_SIZE_AND_INC = 1024 * 64;

		private int i_initialSize = INITIAL_SIZE_AND_INC;

		private int i_incrementSizeBy = INITIAL_SIZE_AND_INC;

		public MemoryFile()
		{
		}

		public MemoryFile(byte[] bytes)
		{
			i_bytes = bytes;
		}

		/// <summary>returns the raw byte data.</summary>
		/// <remarks>
		/// returns the raw byte data.
		/// <br /><br />Use this method to get the byte data from the MemoryFile
		/// to store it to other media or databases, for backup purposes or
		/// to create other MemoryFile sessions.
		/// <br /><br />The byte data from a MemoryFile should only be used
		/// after it is closed.<br /><br />
		/// </remarks>
		/// <returns>bytes the raw byte data.</returns>
		public virtual byte[] GetBytes()
		{
			if (i_bytes == null)
			{
				return new byte[0];
			}
			return i_bytes;
		}

		/// <summary>
		/// returns the size the MemoryFile is to be enlarged, if it grows beyond
		/// the current size.
		/// </summary>
		/// <remarks>
		/// returns the size the MemoryFile is to be enlarged, if it grows beyond
		/// the current size.
		/// </remarks>
		/// <returns>size in bytes</returns>
		public virtual int GetIncrementSizeBy()
		{
			return i_incrementSizeBy;
		}

		/// <summary>returns the initial size of the MemoryFile.</summary>
		/// <remarks>returns the initial size of the MemoryFile.</remarks>
		/// <returns>size in bytes</returns>
		public virtual int GetInitialSize()
		{
			return i_initialSize;
		}

		/// <summary>sets the raw byte data.</summary>
		/// <remarks>
		/// sets the raw byte data.
		/// <br /><br /><b>Caution!</b><br />Calling this method during a running
		/// Memory File session may produce unpreditable results.
		/// </remarks>
		/// <param name="bytes">the raw byte data.</param>
		public virtual void SetBytes(byte[] bytes)
		{
			i_bytes = bytes;
		}

		/// <summary>
		/// configures the size the MemoryFile is to be enlarged by, if it grows
		/// beyond the current size.
		/// </summary>
		/// <remarks>
		/// configures the size the MemoryFile is to be enlarged by, if it grows
		/// beyond the current size.
		/// <br /><br />Call this method before passing the MemoryFile to
		/// <see cref="Db4objects.Db4o.Ext.ExtDb4oFactory.OpenMemoryFile">ExtDb4o#openMemoryFile(MemoryFile)
		/// 	</see>
		/// .
		/// <br /><br />
		/// This parameter can be modified to tune the maximum performance of
		/// a MemoryFile for a specific usecase. To produce the best results,
		/// test the speed of your application with real data.<br /><br />
		/// </remarks>
		/// <param name="byteCount">the desired size in bytes</param>
		public virtual void SetIncrementSizeBy(int byteCount)
		{
			i_incrementSizeBy = byteCount;
		}

		/// <summary>configures the initial size of the MemoryFile.</summary>
		/// <remarks>
		/// configures the initial size of the MemoryFile.
		/// <br /><br />Call this method before passing the MemoryFile to
		/// <see cref="Db4objects.Db4o.Ext.ExtDb4oFactory.OpenMemoryFile">ExtDb4o#openMemoryFile(MemoryFile)
		/// 	</see>
		/// .
		/// <br /><br />
		/// This parameter can be modified to tune the maximum performance of
		/// a MemoryFile for a specific usecase. To produce the best results,
		/// test speed and memory consumption of your application with
		/// real data.<br /><br />
		/// </remarks>
		/// <param name="byteCount">the desired size in bytes</param>
		public virtual void SetInitialSize(int byteCount)
		{
			i_initialSize = byteCount;
		}
	}
}
