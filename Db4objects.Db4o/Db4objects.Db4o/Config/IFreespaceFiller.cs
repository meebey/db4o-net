namespace Db4objects.Db4o.Config
{
	/// <summary>Callback hook for overwriting freed space in the database file.</summary>
	/// <remarks>Callback hook for overwriting freed space in the database file.</remarks>
	public interface IFreespaceFiller
	{
		/// <summary>Called to overwrite freed space in the database file.</summary>
		/// <remarks>Called to overwrite freed space in the database file.</remarks>
		/// <param name="io">Handle for the freed slot</param>
		void Fill(Db4objects.Db4o.IO.IoAdapterWindow io);
	}
}
