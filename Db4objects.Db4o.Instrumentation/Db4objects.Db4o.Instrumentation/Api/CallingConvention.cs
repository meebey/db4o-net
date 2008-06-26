/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Instrumentation.Api;

namespace Db4objects.Db4o.Instrumentation.Api
{
	public sealed class CallingConvention
	{
		public static readonly CallingConvention Static = new CallingConvention();

		public static readonly CallingConvention Virtual = new CallingConvention();

		public static readonly CallingConvention Interface = new CallingConvention();

		private CallingConvention()
		{
		}
	}
}
