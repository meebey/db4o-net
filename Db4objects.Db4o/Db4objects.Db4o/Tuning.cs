namespace Db4objects.Db4o
{
	/// <summary>Tuning switches for customized versions.</summary>
	/// <remarks>Tuning switches for customized versions.</remarks>
	/// <exclude></exclude>
	public class Tuning
	{
		/// <deprecated>Use Db4o.configure().io(new com.db4o.io.SymbianIoAdapter()) instead</deprecated>
		public const bool symbianSeek = false;

		internal const bool readableMessages = true;
	}
}
