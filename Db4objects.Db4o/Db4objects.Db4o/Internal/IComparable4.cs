using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	/// <renameto>com.db4o.internal.Comparable4</renameto>
	public interface IComparable4
	{
		IComparable4 PrepareComparison(object obj);

		int CompareTo(object obj);

		bool IsEqual(object obj);

		bool IsGreater(object obj);

		bool IsSmaller(object obj);

		object Current();
	}
}
