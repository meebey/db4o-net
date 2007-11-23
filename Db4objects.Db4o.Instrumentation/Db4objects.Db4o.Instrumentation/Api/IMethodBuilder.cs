/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using System.Reflection;
using Db4objects.Db4o.Instrumentation.Api;

namespace Db4objects.Db4o.Instrumentation.Api
{
	/// <summary>Cross platform interface for bytecode emission.</summary>
	/// <remarks>Cross platform interface for bytecode emission.</remarks>
	public interface IMethodBuilder
	{
		IReferenceProvider References();

		void Ldc(object value);

		void LoadArgument(int index);

		void Pop();

		void LoadArrayElement(Type elementType);

		void Add(Type operandType);

		void Subtract(Type operandType);

		void Multiply(Type operandType);

		void Divide(Type operandType);

		void Invoke(IMethodRef method);

		void Invoke(MethodInfo method);

		void LoadField(IFieldRef fieldRef);

		void LoadStaticField(IFieldRef fieldRef);

		void Box(Type boxedType);

		void EndMethod();

		void Print(TextWriter @out);
	}
}
