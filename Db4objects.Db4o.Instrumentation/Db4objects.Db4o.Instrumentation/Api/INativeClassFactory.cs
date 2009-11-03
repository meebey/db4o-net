/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Instrumentation.Api
{
	/// <exclude></exclude>
	public interface INativeClassFactory
	{
		/// <exception cref="System.TypeLoadException"></exception>
		Type ForName(string className);
	}
}
