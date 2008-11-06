/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect.Core
{
	/// <exclude></exclude>
	public class ReflectorUtils
	{
		public static IReflectClass ReflectClassFor(IReflector reflector, object clazz)
		{
			if (clazz is IReflectClass)
			{
				return (IReflectClass)clazz;
			}
			if (clazz is Type)
			{
				return reflector.ForClass((Type)clazz);
			}
			if (clazz is string)
			{
				return reflector.ForName((string)clazz);
			}
			return reflector.ForObject(clazz);
		}
	}
}
