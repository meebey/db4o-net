/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;

namespace Db4oUnit.Extensions
{
	/// <summary>
	/// Configure the test case to run with TransparentActivationSupport
	/// enabled unless the test case implements OptOutTA.
	/// </summary>
	/// <remarks>
	/// Configure the test case to run with TransparentActivationSupport
	/// enabled unless the test case implements OptOutTA.
	/// </remarks>
	public class TAFixtureConfiguration : IFixtureConfiguration
	{
		public virtual void Configure(Type clazz, IConfiguration config)
		{
			if (typeof(IOptOutTA).IsAssignableFrom(clazz))
			{
				return;
			}
			config.Add(new TransparentActivationSupport());
		}

		public virtual string GetLabel()
		{
			return "TA";
		}
	}
}
