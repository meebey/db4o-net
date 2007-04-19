using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class GetSingleSimpleArrayTestCase : AbstractDb4oTestCase
	{
		public virtual void Test()
		{
			IObjectSet result = Db().Get(new double[] { 0.6, 0.4 });
			Assert.IsFalse(result.HasNext());
			Assert.IsFalse(result.HasNext());
			Assert.Expect(typeof(InvalidOperationException), new _AnonymousInnerClass17(this, 
				result));
		}

		private sealed class _AnonymousInnerClass17 : ICodeBlock
		{
			public _AnonymousInnerClass17(GetSingleSimpleArrayTestCase _enclosing, IObjectSet
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

			private readonly IObjectSet result;
		}
	}
}
