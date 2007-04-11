using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Collections;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Tests;
using Sharpen.Util;

namespace Db4objects.Db4o.TA.Tests
{
	public class TransparentActivationTestCase : AbstractDb4oTestCase
	{
		protected override void Configure(IConfiguration config)
		{
			config.Add(new PagedListSupport());
			config.Add(new TransparentActivationSupport());
		}

		protected override void Store()
		{
			Project project = new Project("db4o");
			project.LogWorkDone(new UnitOfWork("ta kick-off", new Date(1000), new Date(2000))
				);
			Store(project);
		}

		public virtual void Test()
		{
			Project project = (Project)RetrieveOnlyInstance(typeof(Project));
			Assert.AreEqual(1000, project.TotalTimeSpent());
		}
	}
}
