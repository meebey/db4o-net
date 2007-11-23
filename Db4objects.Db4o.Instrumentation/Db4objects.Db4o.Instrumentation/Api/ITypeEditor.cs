/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Instrumentation.Api;

namespace Db4objects.Db4o.Instrumentation.Api
{
	/// <summary>Cross platform interface for type instrumentation.</summary>
	/// <remarks>Cross platform interface for type instrumentation.</remarks>
	public interface ITypeEditor
	{
		Type ActualType();

		ITypeLoader Loader();

		IReferenceProvider References();

		void AddInterface(Type type);

		IMethodBuilder NewPublicMethod(string methodName, Type returnType, Type[] parameterTypes
			);
	}
}
