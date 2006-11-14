namespace Db4objects.Db4o.Defragment
{
	/// <summary>Filter for StoredClass instances.</summary>
	/// <remarks>Filter for StoredClass instances.</remarks>
	public interface IStoredClassFilter
	{
		/// <param name="storedClass">StoredClass instance to be checked</param>
		/// <returns>true, if the given StoredClass instance should be accepted, false otherwise.
		/// 	</returns>
		bool Accept(Db4objects.Db4o.Ext.IStoredClass storedClass);
	}
}
