using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Tests;
using Db4objects.Db4o.TA.Tests.Collections;
using Sharpen.Util;

namespace Db4objects.Db4o.TA.Tests
{
	public class TransparentActivationTestCase : AbstractDb4oTestCase
	{
		private const int PRIORITY = 42;

		protected override void Configure(IConfiguration config)
		{
			config.Add(new PagedListSupport());
			config.Add(new TransparentActivationSupport());
		}

		protected override void Store()
		{
			Project project = new PrioritizedProject("db4o", PRIORITY);
			project.LogWorkDone(new UnitOfWork("ta kick-off", new Date(1000), new Date(2000))
				);
			Store(project);
		}

		public virtual void Test()
		{
			PrioritizedProject project = (PrioritizedProject)RetrieveOnlyInstance(typeof(Project)
				);
			Assert.AreEqual(PRIORITY, project.GetPriority());
			Assert.AreEqual(1000, project.TotalTimeSpent());
		}
	}
}
