namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public interface IQuickSortable4
	{
		int Size();

		int Compare(int leftIndex, int rightIndex);

		void Swap(int leftIndex, int rightIndex);
	}
}
