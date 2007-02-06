namespace Db4objects.Db4o.Internal
{
	/// <summary>marker interface for special db4o datatypes</summary>
	/// <exclude></exclude>
	public interface IDb4oTypeImpl : Db4objects.Db4o.ITransactionAware
	{
		int AdjustReadDepth(int a_depth);

		bool CanBind();

		object CreateDefault(Db4objects.Db4o.Internal.Transaction a_trans);

		bool HasClassIndex();

		void ReplicateFrom(object obj);

		void SetYapObject(Db4objects.Db4o.Internal.ObjectReference a_yapObject);

		object StoredTo(Db4objects.Db4o.Internal.Transaction a_trans);

		void PreDeactivate();
	}
}
