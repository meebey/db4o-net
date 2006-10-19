namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface ITransactionParticipant
	{
		void Commit(Db4objects.Db4o.Transaction transaction);

		void Rollback(Db4objects.Db4o.Transaction transaction);

		void Dispose(Db4objects.Db4o.Transaction transaction);
	}
}
