using Db4oUnit.Extensions;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class EventsTestCaseBase : AbstractDb4oTestCase
	{
		public sealed class Item
		{
		}

		protected sealed class EventLog
		{
			public bool xing;

			public bool xed;
		}

		protected override void Store()
		{
			Store(new EventsTestCaseBase.Item());
		}

		protected virtual IEventRegistry EventRegistry()
		{
			return EventRegistryFactory.ForObjectContainer(Db());
		}
	}
}
