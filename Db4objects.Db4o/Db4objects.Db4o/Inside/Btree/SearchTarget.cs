namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public sealed class SearchTarget
	{
		public static readonly Db4objects.Db4o.Inside.Btree.SearchTarget LOWEST = new Db4objects.Db4o.Inside.Btree.SearchTarget
			("Lowest");

		public static readonly Db4objects.Db4o.Inside.Btree.SearchTarget ANY = new Db4objects.Db4o.Inside.Btree.SearchTarget
			("Any");

		public static readonly Db4objects.Db4o.Inside.Btree.SearchTarget HIGHEST = new Db4objects.Db4o.Inside.Btree.SearchTarget
			("Highest");

		private readonly string _target;

		public SearchTarget(string target)
		{
			_target = target;
		}

		public override string ToString()
		{
			return _target;
		}
	}
}
