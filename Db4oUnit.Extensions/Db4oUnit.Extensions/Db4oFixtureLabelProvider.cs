/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4oUnit.Extensions
{
	public sealed class Db4oFixtureLabelProvider
	{
		private sealed class _ILabelProvider_8 : ILabelProvider
		{
			public _ILabelProvider_8()
			{
			}

			public string GetLabel(TestMethod method)
			{
				return "[" + this.FixtureLabel(method) + "] " + TestMethod.DefaultLabelProvider.GetLabel
					(method);
			}

			private string FixtureLabel(TestMethod method)
			{
				return ((AbstractDb4oTestCase)method.GetSubject()).Fixture().GetLabel();
			}
		}

		public static readonly ILabelProvider Default = new _ILabelProvider_8();
	}
}
