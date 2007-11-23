/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Instrumentation.Api;

namespace Db4objects.Db4o.Instrumentation.Api
{
	public interface IReferenceProvider
	{
		IMethodRef ForMethod(Type declaringType, string methodName, Type[] parameterTypes
			, Type returnType);

		IFieldRef ForField(Type declaringType, Type fieldType, string fieldName);
	}
}
