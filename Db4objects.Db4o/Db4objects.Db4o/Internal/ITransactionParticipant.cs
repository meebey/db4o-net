using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ITransactionParticipant
	{
		void Commit(Transaction transaction);

		void Rollback(Transaction transaction);

		void Dispose(Transaction transaction);
	}
}
