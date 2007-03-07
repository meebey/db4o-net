namespace Db4objects.Db4o.Foundation
{
	public class FilteredIterator : Db4objects.Db4o.Foundation.MappingIterator
	{
		private readonly Db4objects.Db4o.Foundation.IPredicate4 _filter;

		public FilteredIterator(System.Collections.IEnumerator iterator, Db4objects.Db4o.Foundation.IPredicate4
			 filter) : base(iterator)
		{
			_filter = filter;
		}

		protected override object Map(object current)
		{
			return _filter.Match(current) ? current : Db4objects.Db4o.Foundation.MappingIterator
				.SKIP;
		}
	}
}
