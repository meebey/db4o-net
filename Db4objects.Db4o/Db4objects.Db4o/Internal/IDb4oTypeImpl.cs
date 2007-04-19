using Db4objects.Db4o;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <summary>marker interface for special db4o datatypes</summary>
	/// <exclude></exclude>
	public interface IDb4oTypeImpl : ITransactionAware
	{
		int AdjustReadDepth(int depth);

		bool CanBind();

		object CreateDefault(Transaction trans);

		bool HasClassIndex();

		void ReplicateFrom(object obj);

		void SetObjectReference(ObjectReference @ref);

		object StoredTo(Transaction trans);

		void PreDeactivate();
	}
}
