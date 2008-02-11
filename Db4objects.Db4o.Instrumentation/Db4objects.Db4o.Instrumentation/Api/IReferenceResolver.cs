/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Reflection;
using Db4objects.Db4o.Instrumentation.Api;

namespace Db4objects.Db4o.Instrumentation.Api
{
	public interface IReferenceResolver
	{
		/// <exception cref="InstrumentationException"></exception>
		MethodInfo Resolve(IMethodRef methodRef);
	}
}
