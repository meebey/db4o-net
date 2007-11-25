/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Instrumentation.Api
{
	/// <exclude></exclude>
	public interface INativeClassFactory
	{
		/// <exception cref="TypeLoadException"></exception>
		Type ForName(string className);
	}
}
