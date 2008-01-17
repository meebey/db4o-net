/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
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

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.Freespace().UseBTreeSystem();
			configuration = config;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestSwitchingBackAndForth()
		{
			ProduceSomeFreeSpace();
			Db().Commit();
			int maximumFreespace = StabilizeFreespaceManagerAlterationEffects();
			for (int i = 0; i < 10; i++)
			{
				AssertFreespaceSmallerThan(maximumFreespace);
				configuration.Freespace().UseRamSystem();
				Reopen();
				AssertFreespaceManagerClass(typeof(RamFreespaceManager));
				AssertFreespaceSmallerThan(maximumFreespace);
				configuration.Freespace().UseBTreeSystem();
				Reopen();
				AssertFreespaceManagerClass(typeof(BTreeFreespaceManager));
			}
		}

		private void AssertFreespaceManagerClass(Type clazz)
		{
			Assert.IsInstanceOf(clazz, CurrentFreespaceManager());
		}

		private void AssertFreespaceSmallerThan(int maximumFreespace)
		{
			Assert.IsSmaller(maximumFreespace, CurrentFreespace());
		}

		private int CurrentFreespace()
		{
			return CurrentFreespaceManager().TotalFreespace();
		}

		/// <exception cref="Exception"></exception>
		private int StabilizeFreespaceManagerAlterationEffects()
		{
			int maximumFreespace = 0;
			for (int i = 0; i < 3; i++)
			{
				configuration.Freespace().UseRamSystem();
				Reopen();
				maximumFreespace = Math.Max(maximumFreespace, CurrentFreespace());
				configuration.Freespace().UseBTreeSystem();
				Reopen();
				maximumFreespace = Math.Max(maximumFreespace, CurrentFreespace());
			}
			return maximumFreespace;
		}
	}
}
