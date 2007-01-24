namespace Db4objects.Db4o.Defragment
{
	/// <summary>A message from the defragmentation process.</summary>
	/// <remarks>
	/// A message from the defragmentation process. This is a stub only
	/// and will be refined.
	/// Currently instances of these class will only be created and sent
	/// to registered listeners when invalid IDs are encountered during
	/// the defragmentation process. These probably are harmless and the
	/// result of a user-initiated delete operation.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public class DefragmentInfo
	{
		private string _msg;

		public DefragmentInfo(string msg)
		{
			_msg = msg;
		}

		public override string ToString()
		{
			return _msg;
		}
	}
}
