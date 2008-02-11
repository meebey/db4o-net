/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Config
{
	/// <summary>interface to configure the freespace system to be used.</summary>
	/// <remarks>
	/// interface to configure the freespace system to be used.
	/// &lt;br&gt;&lt;br&gt;All methods should be called before opening database files.
	/// If db4o is instructed to exchange the system
	/// (
	/// <see cref="IFreespaceConfiguration.UseIndexSystem">IFreespaceConfiguration.UseIndexSystem
	/// 	</see>
	/// ,
	/// <see cref="IFreespaceConfiguration.UseRamSystem">IFreespaceConfiguration.UseRamSystem
	/// 	</see>
	/// )
	/// this will happen on opening the database file.&lt;br&gt;&lt;br&gt;
	/// By default the index-based system will be used.
	/// </remarks>
	public interface IFreespaceConfiguration
	{
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
		void DiscardSmallerThan(int byteCount);

		/// <summary>
		/// Configure a way to overwrite freed space in the database file with custom
		/// (for example: random) bytes.
		/// </summary>
		/// <remarks>
		/// Configure a way to overwrite freed space in the database file with custom
		/// (for example: random) bytes. Will slow down I/O operation.
		/// The value of this setting may be cached internally and can thus not be
		/// reliably set after an object container has been opened.
		/// </remarks>
		/// <param name="freespaceFiller">The freespace overwriting callback to use</param>
		void FreespaceFiller(IFreespaceFiller freespaceFiller);

		/// <summary>configures db4o to use a BTree-based freespace system.</summary>
		/// <remarks>
		/// configures db4o to use a BTree-based freespace system.
		/// &lt;br&gt;&lt;br&gt;&lt;b&gt;Advantages&lt;/b&gt;&lt;br&gt;
		/// - ACID, no freespace is lost on abnormal system termination&lt;br&gt;
		/// - low memory consumption&lt;br&gt;
		/// &lt;br&gt;&lt;b&gt;Disadvantages&lt;/b&gt;&lt;br&gt;
		/// - slower than the RAM-based system, since freespace information
		/// is written during every commit&lt;br&gt;
		/// </remarks>
		void UseBTreeSystem();

		/// <summary>discontinued freespace system, only available before db4o 7.0.</summary>
		/// <remarks>discontinued freespace system, only available before db4o 7.0.</remarks>
		[System.ObsoleteAttribute(@"Please use the BTree freespace system instead by calling"
			)]
		void UseIndexSystem();

		/// <summary>configures db4o to use a RAM-based freespace system.</summary>
		/// <remarks>
		/// configures db4o to use a RAM-based freespace system.
		/// &lt;br&gt;&lt;br&gt;&lt;b&gt;Advantages&lt;/b&gt;&lt;br&gt;
		/// - best performance&lt;br&gt;
		/// &lt;br&gt;&lt;b&gt;Disadvantages&lt;/b&gt;&lt;br&gt;
		/// - upon abnormal system termination all freespace is lost&lt;br&gt;
		/// - memory consumption&lt;br&gt;
		/// </remarks>
		void UseRamSystem();
	}
}
