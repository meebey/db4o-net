namespace Db4objects.Db4o.Internal.Btree
{
	public interface IBTreeRange
	{
		/// <summary>
		/// Iterates through all the valid pointers in
		/// this range.
		/// </summary>
		/// <remarks>
		/// Iterates through all the valid pointers in
		/// this range.
		/// </remarks>
		/// <returns>an Iterator4 over BTreePointer value</returns>
		System.Collections.IEnumerator Pointers();

		System.Collections.IEnumerator Keys();

		int Size();

		Db4objects.Db4o.Internal.Btree.IBTreeRange Greater();

		Db4objects.Db4o.Internal.Btree.IBTreeRange Union(Db4objects.Db4o.Internal.Btree.IBTreeRange
			 other);

		Db4objects.Db4o.Internal.Btree.IBTreeRange ExtendToLast();

		Db4objects.Db4o.Internal.Btree.IBTreeRange Smaller();

		Db4objects.Db4o.Internal.Btree.IBTreeRange ExtendToFirst();

		Db4objects.Db4o.Internal.Btree.IBTreeRange Intersect(Db4objects.Db4o.Internal.Btree.IBTreeRange
			 range);

		Db4objects.Db4o.Internal.Btree.IBTreeRange ExtendToLastOf(Db4objects.Db4o.Internal.Btree.IBTreeRange
			 upperRange);

		bool IsEmpty();

		void Accept(Db4objects.Db4o.Internal.Btree.IBTreeRangeVisitor visitor);

		Db4objects.Db4o.Internal.Btree.BTreePointer LastPointer();
	}
}
