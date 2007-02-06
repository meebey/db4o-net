namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ITransactionParticipant
	{
		void Commit(Db4objects.Db4o.Internal.Transaction transaction);

		void Rollback(Db4objects.Db4o.Internal.Transaction transaction);

		void Dispose(Db4objects.Db4o.Internal.Transaction transaction);
	}
}
