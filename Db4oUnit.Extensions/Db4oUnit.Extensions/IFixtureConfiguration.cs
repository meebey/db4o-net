/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions
{
	public interface IFixtureConfiguration
	{
		string GetLabel();

		void Configure(Type clazz, IConfiguration config);
	}
}
