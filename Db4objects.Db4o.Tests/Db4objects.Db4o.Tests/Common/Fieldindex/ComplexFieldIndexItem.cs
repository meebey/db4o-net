namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class ComplexFieldIndexItem : Db4objects.Db4o.Tests.Common.Fieldindex.IHasFoo
	{
		public int foo;

		public int bar;

		public Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem child;

		public ComplexFieldIndexItem()
		{
		}

		public ComplexFieldIndexItem(int foo_, int bar_, Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem
			 child_)
		{
			foo = foo_;
			bar = bar_;
			child = child_;
		}

		public virtual int GetFoo()
		{
			return foo;
		}
	}
}
