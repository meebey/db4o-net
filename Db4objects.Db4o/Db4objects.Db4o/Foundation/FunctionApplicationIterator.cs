namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class FunctionApplicationIterator : Db4objects.Db4o.Foundation.MappingIterator
	{
		private readonly Db4objects.Db4o.Foundation.IFunction4 _function;

		public FunctionApplicationIterator(System.Collections.IEnumerator iterator, Db4objects.Db4o.Foundation.IFunction4
			 function) : base(iterator)
		{
			if (null == function)
			{
				throw new System.ArgumentNullException();
			}
			_function = function;
		}

		protected override object Map(object current)
		{
			return _function.Apply(current);
		}
	}
}
