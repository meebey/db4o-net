/* Copyright (C) 2004-2007   db4objects Inc.   http://www.db4o.com */

namespace Db4objects.Db4o.Tests.Common.TA.Nested
{
	public partial class OuterClass
	{
		public partial class InnerClass
		{
#if CF
			public InnerClass()
			{
			}
#endif
		}
	}

}
