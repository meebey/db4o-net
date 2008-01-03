/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
			IObjectSet result = Db().QueryByExample(new double[] { 0.6, 0.4 });
			Assert.IsFalse(result.HasNext());
			Assert.IsFalse(result.HasNext());
			Assert.Expect(typeof(InvalidOperationException), new _ICodeBlock_17(this, result)
				);
		}

		private sealed class _ICodeBlock_17 : ICodeBlock
		{
			public _ICodeBlock_17(GetSingleSimpleArrayTestCase _enclosing, IObjectSet result)
			{
				this._enclosing = _enclosing;
				this.result = result;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				result.Next();
			}

			private readonly GetSingleSimpleArrayTestCase _enclosing;

			private readonly IObjectSet result;
		}
	}
}
