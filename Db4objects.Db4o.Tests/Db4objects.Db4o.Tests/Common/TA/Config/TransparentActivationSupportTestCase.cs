/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.Config;

namespace Db4objects.Db4o.Tests.Common.TA.Config
{
	public class TransparentActivationSupportTestCase : TransparentActivationTestCaseBase
	{
		public static void Main(string[] args)
		{
			new TransparentActivationSupportTestCase().RunAll();
		}

		public virtual void TestActivationDepth()
		{
			Assert.IsInstanceOf(typeof(TransparentActivationDepthProvider), Stream().ConfigImpl
				().ActivationDepthProvider());
		}
	}
}
