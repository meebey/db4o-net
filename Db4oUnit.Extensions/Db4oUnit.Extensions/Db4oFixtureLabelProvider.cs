/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4oUnit.Extensions
{
	public sealed class Db4oFixtureLabelProvider
	{
		private sealed class _AnonymousInnerClass8 : ILabelProvider
		{
			public _AnonymousInnerClass8()
			{
			}

			public string GetLabel(TestMethod method)
			{
				return "[" + this.FixtureLabel(method) + "] " + TestMethod.DEFAULT_LABEL_PROVIDER
					.GetLabel(method);
			}

			private string FixtureLabel(TestMethod method)
			{
				return ((AbstractDb4oTestCase)method.GetSubject()).Fixture().GetLabel();
			}
		}

		public static readonly ILabelProvider DEFAULT = new _AnonymousInnerClass8();
	}
}
