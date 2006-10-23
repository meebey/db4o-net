namespace Db4objects.Db4o
{
	internal interface IOrderable
	{
		int CompareTo(object obj);

		void HintOrder(int a_order, bool a_major);

		bool HasDuplicates();
	}
}
