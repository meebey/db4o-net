/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Tests.Common.Freespace;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public class FreespaceMigrationTestCase : FreespaceManagerTestCaseBase, IOptOutCS
		, IOptOutDefragSolo
	{
		private IConfiguration configuration;

		public static void Main(string[] args)
		{
			new FreespaceMigrationTestCase().RunSolo();
		}

		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.Freespace().UseBTreeSystem();
			configuration = config;
		}

		public virtual void TestSwitchingBackAndForth()
		{
			ProduceSomeFreeSpace();
			Db().Commit();
			for (int i = 0; i < 5; i++)
			{
				int oldFreespace = CurrentFreespaceManager().TotalFreespace();
				configuration.Freespace().UseRamSystem();
				Reopen();
				Assert.IsInstanceOf(typeof(RamFreespaceManager), CurrentFreespaceManager());
				int newFreespace = CurrentFreespaceManager().TotalFreespace();
				if (i > 2)
				{
					Assert.AreEqual(oldFreespace, newFreespace);
				}
				configuration.Freespace().UseBTreeSystem();
			}
		}
	}
}
