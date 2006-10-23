namespace Db4objects.Db4o.Foundation
{
	/// <summary>Iterator primitives (cat, map, reduce, filter, etc...).</summary>
	/// <remarks>Iterator primitives (cat, map, reduce, filter, etc...).</remarks>
	/// <exclude></exclude>
	public class Iterators
	{
		internal static readonly object NO_ELEMENT = new object();

		public static System.Collections.IEnumerator Concat(System.Collections.IEnumerator
			 iterators)
		{
			return new Db4objects.Db4o.Foundation.CompositeIterator4(iterators);
		}

		public static System.Collections.IEnumerator Map(System.Collections.IEnumerator iterator
			, Db4objects.Db4o.Foundation.IFunction4 function)
		{
			return new Db4objects.Db4o.Foundation.FunctionApplicationIterator(iterator, function
				);
		}

		public static System.Collections.IEnumerator Map(object[] array, Db4objects.Db4o.Foundation.IFunction4
			 function)
		{
			return Map(new Db4objects.Db4o.Foundation.ArrayIterator4(array), function);
		}
	}
}
