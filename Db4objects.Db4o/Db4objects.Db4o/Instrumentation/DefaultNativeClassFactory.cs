/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Instrumentation;

namespace Db4objects.Db4o.Instrumentation
{
	public class DefaultNativeClassFactory : INativeClassFactory
	{
		/// <exception cref="TypeLoadException"></exception>
		public virtual Type ForName(string className)
		{
			return Sharpen.Runtime.GetType(className);
		}
	}
}
