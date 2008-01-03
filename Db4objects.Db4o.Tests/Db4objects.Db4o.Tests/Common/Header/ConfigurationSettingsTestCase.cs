/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Header
{
	public class ConfigurationSettingsTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		/// <exception cref="Exception"></exception>
		[System.ObsoleteAttribute]
		public virtual void TestChangingUuidSettings()
		{
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Assert.AreEqual(ConfigScope.Globally, GenerateUUIDs());
			Db().Configure().GenerateUUIDs(-1);
			Assert.AreEqual(ConfigScope.Disabled, GenerateUUIDs());
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Assert.AreEqual(ConfigScope.Globally, GenerateUUIDs());
		}

		private ConfigScope GenerateUUIDs()
		{
			return ((LocalObjectContainer)Db()).Config().GenerateUUIDs();
		}
	}
}
