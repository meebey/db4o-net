using System;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	class DatePlatform
	{
		public static readonly long MIN_DATE = DateTime.MinValue.Ticks;

		public static readonly long MAX_DATE = DateTime.MaxValue.Ticks;
	}
}
