/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Instrumentation.Api
{
	public sealed class CallingConvention
	{
		public static readonly Db4objects.Db4o.Instrumentation.Api.CallingConvention STATIC
			 = new Db4objects.Db4o.Instrumentation.Api.CallingConvention();

		public static readonly Db4objects.Db4o.Instrumentation.Api.CallingConvention VIRTUAL
			 = new Db4objects.Db4o.Instrumentation.Api.CallingConvention();

		public static readonly Db4objects.Db4o.Instrumentation.Api.CallingConvention INTERFACE
			 = new Db4objects.Db4o.Instrumentation.Api.CallingConvention();

		private CallingConvention()
		{
		}
	}
}
