/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Handlers.AllTests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ArrayHandlerTestCase), typeof(BooleanHandlerTestCase), 
				typeof(ByteHandlerTestCase), typeof(CharHandlerTestCase), typeof(ClassHandlerTestCase
				), typeof(ClassMetadataTypehandlerTestCase), typeof(CustomTypeHandlerTestCase), 
				typeof(DoubleHandlerTestCase), typeof(FloatHandlerTestCase), typeof(IntHandlerTestCase
				), typeof(LongHandlerTestCase), typeof(MultiDimensionalArrayHandlerTestCase), typeof(
				StringBufferTypeHandlerTestCase), typeof(StringHandlerTestCase), typeof(ShortHandlerTestCase
				), typeof(UntypedHandlerTestCase) };
		}
	}
}
