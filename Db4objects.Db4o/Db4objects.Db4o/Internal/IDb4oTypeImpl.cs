namespace Db4objects.Db4o.Internal
{
	/// <summary>marker interface for special db4o datatypes</summary>
	/// <exclude></exclude>
	public interface IDb4oTypeImpl : Db4objects.Db4o.ITransactionAware
	{
		int AdjustReadDepth(int depth);

		bool CanBind();

		object CreateDefault(Db4objects.Db4o.Internal.Transaction trans);

		bool HasClassIndex();

		void ReplicateFrom(object obj);

		void SetObjectReference(Db4objects.Db4o.Internal.ObjectReference @ref);

		object StoredTo(Db4objects.Db4o.Internal.Transaction trans);

		void PreDeactivate();
	}
}
