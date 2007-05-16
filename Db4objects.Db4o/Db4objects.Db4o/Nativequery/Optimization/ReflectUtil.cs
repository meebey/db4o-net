/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Nativequery.Optimization
{
	public class ReflectUtil
	{
		public static MethodInfo MethodFor(Type clazz, string methodName, Type[] paramTypes
			)
		{
			Type curclazz = clazz;
			while (curclazz != null)
			{
				try
				{
					MethodInfo method = Sharpen.Runtime.GetDeclaredMethod(curclazz, methodName, paramTypes
						);
					Platform4.SetAccessible(method);
					return method;
				}
				catch (Exception)
				{
				}
				curclazz = curclazz.BaseType;
			}
			return null;
		}

		public static FieldInfo FieldFor(Type clazz, string name)
		{
			Type curclazz = clazz;
			while (curclazz != null)
			{
				try
				{
					FieldInfo field = Sharpen.Runtime.GetDeclaredField(curclazz, name);
					Platform4.SetAccessible(field);
					return field;
				}
				catch (Exception)
				{
				}
				curclazz = curclazz.BaseType;
			}
			return null;
		}
	}
}
