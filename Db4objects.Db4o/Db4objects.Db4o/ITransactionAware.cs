using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface ITransactionAware
	{
		void SetTrans(Transaction a_trans);
	}
}
