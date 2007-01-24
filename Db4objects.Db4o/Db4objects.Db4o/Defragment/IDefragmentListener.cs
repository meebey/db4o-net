namespace Db4objects.Db4o.Defragment
{
	/// <summary>Listener for defragmentation process messages.</summary>
	/// <remarks>Listener for defragmentation process messages.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public interface IDefragmentListener
	{
		/// <summary>
		/// This method will be called when the defragment process encounters
		/// file layout anomalies during the defragmentation process.
		/// </summary>
		/// <remarks>
		/// This method will be called when the defragment process encounters
		/// file layout anomalies during the defragmentation process.
		/// </remarks>
		/// <param name="info">The message from the defragmentation process.</param>
		void NotifyDefragmentInfo(Db4objects.Db4o.Defragment.DefragmentInfo info);
	}
}
