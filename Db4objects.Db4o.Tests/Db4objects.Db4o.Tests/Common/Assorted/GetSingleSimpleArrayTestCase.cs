namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class GetSingleSimpleArrayTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public virtual void Test()
		{
			Db4objects.Db4o.IObjectSet result = Db().Get(new double[] { 0.6, 0.4 });
			Db4oUnit.Assert.IsFalse(result.HasNext());
			Db4oUnit.Assert.IsFalse(result.HasNext());
			Db4oUnit.Assert.Expect(typeof(System.InvalidOperationException), new _AnonymousInnerClass17
				(this, result));
		}

		private sealed class _AnonymousInnerClass17 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass17(GetSingleSimpleArrayTestCase _enclosing, Db4objects.Db4o.IObjectSet
				 result)
			{
				this._enclosing = _enclosing;
				this.result = result;
			}

			public void Run()
			{
				result.Next();
			}

			private readonly GetSingleSimpleArrayTestCase _enclosing;

			private readonly Db4objects.Db4o.IObjectSet result;
		}
	}
}
