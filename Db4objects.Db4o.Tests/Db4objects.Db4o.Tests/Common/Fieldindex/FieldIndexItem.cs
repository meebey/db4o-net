namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class FieldIndexItem : Db4objects.Db4o.Tests.Common.Fieldindex.IHasFoo
	{
		public int foo;

		public FieldIndexItem()
		{
		}

		public FieldIndexItem(int foo_)
		{
			foo = foo_;
		}

		public virtual int GetFoo()
		{
			return foo;
		}
	}
}
