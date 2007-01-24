namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class TreeObject : Db4objects.Db4o.Foundation.Tree
	{
		private readonly object _object;

		private readonly Db4objects.Db4o.Foundation.IComparison4 _function;

		public TreeObject(object @object, Db4objects.Db4o.Foundation.IComparison4 function
			)
		{
			_object = @object;
			_function = function;
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree tree)
		{
			return _function.Compare(_object, tree.Key());
		}

		public override object Key()
		{
			return _object;
		}
	}
}
