namespace Db4objects.Db4o
{
	/// <summary>
	/// allows registration with a transaction to be notified of
	/// commit and rollback
	/// </summary>
	/// <exclude></exclude>
	public interface ITransactionListener
	{
		void PreCommit();

		void PostRollback();
	}
}
